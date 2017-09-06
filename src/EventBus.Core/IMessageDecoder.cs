using EventBus.Core.Internal.Model;

namespace EventBus.Core
{
    public interface IMessageDecoder
    {
        ReceivedMessage Decode(MessageContext context);
    }
}
