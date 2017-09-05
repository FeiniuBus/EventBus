using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using EventBus.Core;
using EventBus.Core.Infrastructure;

namespace EventBus.Publish.Infrastructure
{
    public class DefaultConnectionFactoryAccessor : IConnectionFactoryAccessor
    {
        RabbitOptions RabbitOptions;
        public IConnectionFactory ConnectionFactory { get; }

        public DefaultConnectionFactoryAccessor(IOptions<RabbitOptions> optionsAcssesor)
        {
            RabbitOptions = optionsAcssesor.Value;
            ConnectionFactory = new ConnectionFactory()
            {
                UserName = RabbitOptions.UserName,
                Password = RabbitOptions.Password,
                VirtualHost = RabbitOptions.VirtualHost,
                HostName = RabbitOptions.HostName,
                Port = RabbitOptions.Port
            };
        }
    }
}
