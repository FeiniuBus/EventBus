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

        public IModel Channel { get; set; }

        public byte[] Content { get; set; }

        public BasicDeliverEventArgs Args { get; set; }

        public void Ack()
        {
            Channel.BasicAck(DeliveryTag, false);
        }

        public void Reject()
        {
            Channel.BasicReject(DeliveryTag, false);
        }
    }
}
