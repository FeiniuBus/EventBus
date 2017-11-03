using System;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBus.Core
{
    public class MessageContext
    {
        public string Exchange { get; set; }

        public string Topic { get; set; }

        public string Queue { get; set; }

        public ulong DeliveryTag { get; set; }

        [JsonIgnore]
        public IModel Channel { get; set; }

        public byte[] Content { get; set; }

        [JsonIgnore]
        public BasicDeliverEventArgs Args { get; set; }

        public void Ack()
        {
            Channel.BasicAck(DeliveryTag, false);
        }

        public void Reject(bool requeue = false)
        {
            Channel.BasicReject(DeliveryTag, requeue);
        }
    }
}
