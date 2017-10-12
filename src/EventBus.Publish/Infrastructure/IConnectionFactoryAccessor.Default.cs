using EventBus.Core;
using EventBus.Core.Infrastructure;
using RabbitMQ.Client;

namespace EventBus.Publish.Infrastructure
{
    public class DefaultConnectionFactoryAccessor : IConnectionFactoryAccessor
    {
        RabbitOptions RabbitOptions;
        public IConnectionFactory ConnectionFactory { get; }

        public DefaultConnectionFactoryAccessor(RabbitOptions optionsAcssesor)
        {
            RabbitOptions = optionsAcssesor;
            ConnectionFactory = new ConnectionFactory()
            {
                UserName = RabbitOptions.UserName,
                Password = RabbitOptions.Password,
                VirtualHost = RabbitOptions.VirtualHost,
                HostName = RabbitOptions.HostName,
                Port = RabbitOptions.Port,
            };
        }
    }
}
