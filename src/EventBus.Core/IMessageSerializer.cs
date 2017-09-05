using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Core
{
    public interface IMessageSerializer
    {
        byte[] Serialize<MessageT>(MessageT message) where MessageT : class;
    }
}
