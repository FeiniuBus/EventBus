using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.MySQL.Internal.Model
{
    internal class Message
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public long TransactId { get; set; }
        public string MetaData { get; set; }
        public string Content { get; set; }
        public short Type { get; set; }
        public short State { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
