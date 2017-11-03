namespace EventBus.Core.Infrastructure
{
    public class DefaultFailureContextAccessor : IFailureContextAccessor
    {
        public ConsumeFailureContext FailureContext { get ; set ; }
    }
}
