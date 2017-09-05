using RabbitMQ.Client;

namespace EventBus.Core
{
    public class RabbitOptions
    {
        public string UserName { get; set; } = ConnectionFactory.DefaultUser;

        public string Password { get; set; } = ConnectionFactory.DefaultPass;

        public string VirtualHost { get; set; } = ConnectionFactory.DefaultVHost;

        public string HostName { get; set; }

        public int Port { get; set; } = AmqpTcpEndpoint.UseDefaultPort;

        public string DefaultExchangeName => "default.exchange@feiniubus";
<<<<<<< HEAD:src/EventBus.Core/RabbitOptions.cs
<<<<<<< HEAD:src/EventBus.Core/RabbitOptions.cs

        public int QueueMessageExpires { get; set; } = 864000000;
=======
>>>>>>> parent of 32b76d2... add tx:src/EventBus.Core/RabbitOptions.cs
=======
>>>>>>> parent of 32b76d2... add tx:src/EventBus.Core/RabbitOptions.cs
    }
}
