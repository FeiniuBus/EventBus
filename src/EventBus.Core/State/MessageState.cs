namespace EventBus.Core.State
{
    public enum MessageState : short
    {
        Processing = 1,
        Succeeded = 2,
        Failed = 3
    }
}
