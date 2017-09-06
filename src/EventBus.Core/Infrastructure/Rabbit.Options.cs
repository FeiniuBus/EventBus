using RabbitMQ.Client;

namespace EventBus.Core.Infrastructure
{
    public class RabbitOptions
    {
        public string UserName { get; set; } = ConnectionFactory.DefaultUser;

        public string Password { get; set; } = ConnectionFactory.DefaultPass;

        public string VirtualHost { get; set; } = ConnectionFactory.DefaultVHost;

        public string HostName { get; set; }

        public int Port { get; set; } = AmqpTcpEndpoint.UseDefaultPort;

        public string DefaultExchangeName { get; set; } = "default.exchange@feiniubus";

        public int QueueMessageExpires { get; set; } = 864000000;

        public string DefaultDeadLetterExchange { get; set; } = "deadletter.exchange@feiniubus";
    }
}
