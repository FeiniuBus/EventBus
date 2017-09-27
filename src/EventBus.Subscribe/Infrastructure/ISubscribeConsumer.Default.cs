using EventBus.Core;
using EventBus.Core.Extensions;
using EventBus.Core.Infrastructure;
using EventBus.Subscribe.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Subscribe.Infrastructure
{
    public class DefaultSubscribeConsumer : IConsumer
    {
        private readonly IList<IDisposable> _disposables;
        private readonly IServiceProvider _serviceProvider;
        private readonly SubscribeOptions _subscribeOptions;
        private readonly IReceivedEventPersistenter _receivedEventPersistenter;
        private readonly IMessageDecoder _messageDecoder;
        private readonly ILogger<DefaultSubscribeConsumer> _logger;

        public DefaultSubscribeConsumer(IServiceProvider serviceProvider
            , IReceivedEventPersistenter receivedEventPersistenter
            , IMessageDecoder messageDecoder
            , SubscribeOptions options
            , ILogger<DefaultSubscribeConsumer> logger)
        {
            _disposables = new List<IDisposable>();
            _serviceProvider = serviceProvider;
            _subscribeOptions = options;
            _receivedEventPersistenter = receivedEventPersistenter;
            _messageDecoder = messageDecoder;
            _logger = logger;
        }

        public void Start()
        {
            var clients = GetClients();
            foreach(var client in clients)
            {
                client.Listening();
            }
        }

        private ISubscribeClient[] GetClients()
        {
            var subscribeInfos = _subscribeOptions.SubscribeInfos;
            var groupedInfos = subscribeInfos.GroupBy(x => x.Group)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Exchange)
                    .ToDictionary(y => y.Key, y => y.ToArray()));

            _logger.LogInformation($"subscribe infos {subscribeInfos.ToJson()}");

            var clients = new List<ISubscribeClient>();
            var connectfactoryAccessor = _serviceProvider.GetRequiredService<IConnectionFactoryAccessor>();
            var rabbitOption = _serviceProvider.GetRequiredService<IOptions<RabbitOptions>>().Value;

            foreach(var queueGroupedItems in groupedInfos)
            {
                foreach(var exchangeGroupedItems in queueGroupedItems.Value)
                {
                    for(var i = 0; i < _subscribeOptions.ConsumerClientCount; ++i)
                    {
                        var exchange = string.IsNullOrEmpty(exchangeGroupedItems.Key) ? rabbitOption.DefaultExchangeName : exchangeGroupedItems.Key;

                        var client = new DefaultSubscribeClient(_serviceProvider
                            , connectfactoryAccessor
                            , rabbitOption
                            , queueGroupedItems.Key
                            , exchange);

                        _disposables.Add(client);
                        RegisterClient(client);

                        var topics = exchangeGroupedItems.Value.Select(x => x.Topic).ToArray();
                        client.Subscribe(topics);

                        client.Listening();
                    }
                }
            }

            return clients.ToArray();
        }

        private void RegisterClient(ISubscribeClient client)
        {
            client.OnReceive = (MessageContext context) =>
            {
                Core.Internal.Model.ReceivedMessage msg = null;

                try
                {
                    msg = _messageDecoder.Decode(context);
                }
                catch(Exception ex)
                {
                    context.Reject(true);
                    _logger.DecodeError(context, ex);
                    return;
                }

                try
                {
                    _receivedEventPersistenter.InsertAsync(msg);
                }
                catch(Exception e)
                {
                    context.Reject(true);
                    _logger.ReceivedEventPersistenterInsert(msg, e);
                    return;
                }

                bool result = false;
                DefaultConsumerInvoker invoker = null;
                try
                {
                    invoker = new DefaultConsumerInvoker(_serviceProvider, context);
                }
                catch(Exception e)
                {
                    _logger.CreateDefaultConsumerInvoker(context, e);
                }
                try
                {
                    result = invoker.InvokeAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    _logger.LogInformation($"invoke result: {result} message: {msg.ToJson()}");

                    try
                    {
                        _receivedEventPersistenter.ChangeStateAsync(msg.MessageId, msg.TransactId, msg.Group, Core.State.MessageState.Succeeded);
                    }
                    catch(Exception ex)
                    {
                        _logger.UpdateReceivedMessage(msg, ex);
                    }
                }
                catch(Exception ex)
                {
                    _logger.InvokeConsumer(context, msg, ex);
                }
                finally
                {
                    if (result)
                    {
                        context.Ack();
                        _logger.LogInformation($"ack message {msg.ToJson()}");
                    }
                    else
                    {
                        context.Reject();
                        _logger.LogInformation($"reject message {msg.ToJson()}");

                        try
                        {
                            _receivedEventPersistenter.ChangeStateAsync(msg.MessageId, msg.TransactId, msg.Group, Core.State.MessageState.Failed);
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(110, ex, $"fail to update received message[ignore]: {msg.ToJson()}");
                        }

                        try
                        {
                            var subFailureHandlers = _serviceProvider.GetServices<ISubFailureHandler>().ToArray();
                            if (subFailureHandlers.Any())
                            {
                                Task.WhenAll(subFailureHandlers.Select(x => x.HandleAsync(context))).ConfigureAwait(false).GetAwaiter().GetResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(110, ex, $"SubFailureHandler调用失败");
                        }
                    }
                }
            };
        }

        public void Dispose()
        {
            foreach(var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
