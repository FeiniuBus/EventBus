namespace EventBus.Core.State
{
    public sealed class StateChangedArgs
    {
        public StateChangedArgs(MessageState oldState, MessageState newState)
        {
            OldState = oldState;
            NewState = newState;
        }

        public MessageState OldState { get; }
        public MessageState NewState { get; }
    }
}
