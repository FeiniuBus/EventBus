using RabbitMQ.Client;
using Microsoft.Extensions.Options;

namespace EventBus.Core.Infrastructure
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
                HostName = RabbitOptions.Host,
                Port = RabbitOptions.Port
            };
        }
    }
}
