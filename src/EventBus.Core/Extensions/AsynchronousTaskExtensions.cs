using System.Threading;
using System.Threading.Tasks;

namespace EventBus.Core.Extensions
{
    public static class AsynchronousTaskExtensions
    {
        internal static TaskFactory GetTaskFactory() => new TaskFactory(CancellationToken.None,
            TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        public static void Synchronize(this Task task)
        {
            var factory = GetTaskFactory();
            factory.StartNew(() => task).Unwrap().GetAwaiter().GetResult();
        }

        public static TResult Synchronize<TResult>(this Task<TResult> task)
        {
            var factory = GetTaskFactory();
            return factory.StartNew(() => task).Unwrap().GetAwaiter().GetResult();
        }
    }
}
