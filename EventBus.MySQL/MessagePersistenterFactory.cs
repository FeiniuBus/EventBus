using EventBus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace EventBus.MySQL
{
    public class MessagePersistenterFactory : IMessagePersistenterFactory
    {
        private readonly IIdentityGenerator _identityGenerator;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventBusEFOptions _eventBusEFOptions;
        private readonly ILoggerFactory _loggerFactory;

        public MessagePersistenterFactory(EventBusEFOptions efOptions, IIdentityGenerator identityGenerator, ILoggerFactory loggerFactory,IServiceProvider serviceProvider)
        {
            _eventBusEFOptions = efOptions;
            _identityGenerator = identityGenerator;
            _loggerFactory = loggerFactory;
            _serviceProvider = serviceProvider;
            TransactionID = _identityGenerator.NextIdentity();
        }

        public long TransactionID { get; }

        public IMessagePersistenter<TContent> Create<TContent>()
        {
            return new MessagePersistenter<TContent>(TransactionID, _identityGenerator,_eventBusEFOptions, _loggerFactory.CreateLogger<MessagePersistenter<TContent>>(), _serviceProvider);
        }

        public IMessagePersistenterFactory CreateScope(bool createServiceProviderScope = false)
        {
            return new MessagePersistenterFactory(_eventBusEFOptions, _identityGenerator, _loggerFactory, createServiceProviderScope ? _serviceProvider.CreateScope().ServiceProvider : _serviceProvider);
        }
    }
}
