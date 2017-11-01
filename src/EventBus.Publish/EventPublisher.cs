using EventBus.Core;
using EventBus.Core.Extensions;
using EventBus.Core.Infrastructure;
using EventBus.Core.State;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Publish
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IServiceScope _serviceScope;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IPublishedEventPersistenter _publishedEventPersistenter;
        private readonly IMessageQueueTransaction _messageQueueTransaction;
        private readonly IMessageSerializer _messageSerializer;
        private readonly ILogger _logger;
        public EventPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _identityGenerator = serviceProvider.GetRequiredService< IIdentityGenerator>();
            _publishedEventPersistenter = serviceProvider.GetRequiredService< IPublishedEventPersistenter>();
            _messageQueueTransaction = serviceProvider.GetRequiredService< IMessageQueueTransaction>();
            _messageSerializer = serviceProvider.GetRequiredService< IMessageSerializer>();
            _logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<EventPublisher>();
            TransactID = _identityGenerator.NextIdentity();
        }

        private EventPublisher(IServiceScope serviceScope) : this(serviceScope.ServiceProvider)
        {
            _serviceScope = serviceScope;
        }

        public long TransactID { get; }

        public async Task ConfirmAsync()
        {
            await _messageQueueTransaction.CommitAsync();
        }

        public IEventPublisher CreateScope()
        {
            return new EventPublisher(_serviceProvider.CreateScope());
        }

        public void Dispose()
        {
            _serviceScope?.Dispose();
            _messageQueueTransaction?.Dispose();
        }

        public async Task PrepareAsync(EventDescriptor descriptor)
        {
            var options = _serviceProvider.GetRequiredService<EventBusMySQLOptions>();
            if (options.DbContextType == null) throw new ArgumentException($"Field `DbContextType` should been configured before using PrepareAsync(EventDescriptor).");
            var dbContext = _serviceProvider.GetRequiredService(options.DbContextType) as DbContext;
            var dbContextTransaction = dbContext.Database.CurrentTransaction;
            if (dbContextTransaction == null) throw new ArgumentException("Could not access a transaction before it has began.");
            var dbConnection = dbContext.Database.GetDbConnection();
            var dbTransaction = dbContextTransaction.GetDbTransaction();
            await PrepareAsync(descriptor, dbConnection, dbTransaction);
        }

        public async Task PrepareAsync(EventDescriptor descriptor, IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            var message = new Internal.Model.Message
            {
                Id = _identityGenerator.NextIdentity(),
                MessageId = _identityGenerator.NextIdentity(),
                TransactId = TransactID,
                Content = descriptor.Message.GetTransferJson(),
                CreationDate = DateTime.Now,
                State = MessageState.Processing,
                Type = MessageType.Published,
                Exchange = descriptor.Exchange,
                RouteKey = descriptor.RouteKey
            };
            var metaData = new MetaData();
            metaData.Set("ContentType", descriptor.ContentType);
            metaData.Set("TransactID", message.TransactId.ToString());
            metaData.Set("MessageID", message.MessageId.ToString());
            metaData.Contact(descriptor.Message.MetaData);
           
            message.MetaData = metaData.ToJson();

            await _publishedEventPersistenter.InsertAsync(message, dbConnection, dbTransaction);
            var jsonTransafer = FeiniuBus.AspNetCore.Json.FeiniuBusJsonConvert.SerializeObject(new { MetaData = metaData.GetDictionary(), Content = descriptor.Message.Content });
            await _messageQueueTransaction.PublishAsync(descriptor.Exchange, descriptor.RouteKey, Encoding.UTF8.GetBytes(jsonTransafer));
        }

        public async Task RollbackAsync()
        {
            await _messageQueueTransaction.RollbackAsync();
        }
    }
}
