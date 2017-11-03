namespace EventBus.Core
{
    public interface IFailureContextAccessor
    {
        ConsumeFailureContext FailureContext { get; set; }
    }
}
