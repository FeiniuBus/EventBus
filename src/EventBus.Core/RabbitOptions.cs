using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Core
{
    public class RabbitOptions
    {
        public string UserName { get; set; } = ConnectionFactory.DefaultUser;

        public string Password { get; set; } = ConnectionFactory.DefaultPass;

        public string VirtualHost { get; set; } = ConnectionFactory.DefaultVHost;

        public string Host { get; set; }

        public int Port { get; set; } = AmqpTcpEndpoint.UseDefaultPort;
    }
}
