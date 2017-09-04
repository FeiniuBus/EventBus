using Microsoft.Extensions.Logging;

namespace EventBus.MySQL.Internal
{
    public static class LoggerExtensions
    {
        public static void MessagePersitenterNotUsingTransaction(this ILogger logger, long transactionId)
        {
            logger.LogWarning($"Not using any transaction during message persistence.Trace Info:[TransactionId]{transactionId}.");
        }
    }
}
