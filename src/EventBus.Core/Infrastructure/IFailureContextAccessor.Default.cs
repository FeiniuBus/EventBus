namespace EventBus.Core.Infrastructure
{
    public class DefaultFailureContextAccessor : IFailureContextAccessor
    {
        public FailureContext FailureContext { get ; set ; }
    }
}
