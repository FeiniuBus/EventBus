namespace EventBus.Core
{
    public interface IFailureContextAccessor
    {
        FailureContext FailureContext { get; set; }
    }
}
