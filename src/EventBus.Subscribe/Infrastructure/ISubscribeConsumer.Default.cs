using EventBus.Core;
using EventBus.Core.Extensions;
using EventBus.Core.Infrastructure;
using EventBus.Subscribe.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Core.Internal.Model;

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

        public TaskFactory Factory { get; }

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

            Factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning);
        }

        public void Start()
        {
            var clients = GetClients();
            foreach (var client in clients)
            {
                client.Listening();
            }
        }

        private IList<ISubscribeClient> GetClients()
        {
            var subscribeInfos = _subscribeOptions.SubscribeInfos;
            var groupedInfos = subscribeInfos.GroupBy(x => x.Group)
                .ToDictionary(x => x.Key, x => x.GroupBy(y => y.Exchange)
                    .ToDictionary(y => y.Key, y => y.ToArray()));

            _logger.LogInformation($"subscribe infos {subscribeInfos.ToJson()}");

            var clients = new List<ISubscribeClient>();
            var connectfactoryAccessor = _serviceProvider.GetRequiredService<IConnectionFactoryAccessor>();
            var rabbitOption = _serviceProvider.GetRequiredService<RabbitOptions>();

            foreach (var queueGroupedItems in groupedInfos)
            {
                foreach (var exchangeGroupedItems in queueGroupedItems.Value)
                {
                    for (var i = 0; i < _subscribeOptions.ConsumerClientCount; ++i)
                    {
                        var exchange = string.IsNullOrEmpty(exchangeGroupedItems.Key) ? rabbitOption.DefaultExchangeName : exchangeGroupedItems.Key;

                        var client = new DefaultSubscribeClient(_serviceProvider
                            , connectfactoryAccessor
                            , rabbitOption
                            , queueGroupedItems.Key
                            , exchange);

                        _disposables.Add(client);
                        clients.Add(client);
                        RegisterClient(client);

                        var topics = exchangeGroupedItems.Value.Select(x => x.Topic).ToArray();
                        client.Subscribe(topics);
                    }
                }
            }

            return clients;
        }

        private async Task MessageHandle(object ctx)
        {
            var context = (MessageContext)ctx;
            ReceivedMessage msg = null;

            try
            {
                msg = _messageDecoder.Decode(context);
            }
            catch (Exception ex)
            {
                _logger.DecodeError(context, ex);

                HandleError(_serviceProvider, new FailContext
                {
                    Raw = Encoding.UTF8.GetString(context.Content),
                    ExceptionMessage = ex.Message
                });

                return;
            }

            try
            {
                await _receivedEventPersistenter.InsertAsync(msg);
            }
            catch (Exception e)
            {
                _logger.ReceivedEventPersistenterInsert(msg, e);

                HandleError(_serviceProvider, new FailContext
                {
                    ExceptionMessage = e.Message,
                    Raw = Encoding.UTF8.GetString(context.Content),
                });

                return;
            }

            var result = false;

            try
            {
                var invoker = new DefaultConsumerInvoker(_serviceProvider, context);
                result = await invoker.InvokeAsync();

                _logger.LogInformation($"invoke result: {result} message: {msg.ToJson()}");
            }
            catch (Exception ex)
            {
                _logger.InvokeConsumer(context, msg, ex);
            }
            finally
            {
                if (result)
                {
                    try
                    {
                        await _receivedEventPersistenter.ChangeStateAsync(msg.MessageId, msg.TransactId, msg.Group, Core.State.MessageState.Succeeded);
                    }
                    catch (Exception ex)
                    {
                        _logger.UpdateReceivedMessage(msg, ex);
                    }
                    _logger.LogInformation($"message invoke true {msg.ToJson()}");
                }
                else
                {
                    _logger.LogInformation($"invoke false {msg.ToJson()}");

                    try
                    {
                        await _receivedEventPersistenter.ChangeStateAsync(msg.MessageId, msg.TransactId, msg.Group, Core.State.MessageState.Failed);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(110, ex, $"fail to update received message[ignore]: {msg.ToJson()}");
                    }

                    HandleError(_serviceProvider, new FailContext
                    {
                        ExceptionMessage = "",
                        State = msg,
                    });
                }
            }

        }

        private void RegisterClient(ISubscribeClient client)
        {
            client.OnReceive = (MessageContext context) =>
            {
                Factory.StartNew(MessageHandle, context);
            };
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        private static void HandleError(IServiceProvider serviceProvider, FailContext context)
        {
            ThreadPool.QueueUserWorkItem((state) =>
            {
                try
                {
                    var handlers = serviceProvider.GetServices<ISubFailureHandler>();
                    foreach (var handler in handlers)
                    {
                        handler.HandleAsync(context);
                    }
                }
                catch
                {
                    // ignored
                }
            });
        }

        class InvokeState
        {
            public IServiceProvider ServiceProvider { get; set; }

            public MessageContext MessageContext { get; set; }
        }
    }
}
