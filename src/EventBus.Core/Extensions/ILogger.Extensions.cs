using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Core.Extensions
{
    public static class ILoggerExtensions
    {
        public static void DecodeError(this ILogger logger, MessageContext context, Exception e)
        {
            LogJson(logger, $"An exception has been thrown when decode message from context.", new
            {
                InnerExceptionMessages = GetErrorMessages(e),
                MessageContext = context
            });
        }

        public static void ReceivedEventPersistenterInsert(this ILogger logger, object message, Exception e)
        {
            LogJson(logger, $"An exception has been thrown when insert received message[requeue]:.", new
            {
                InnerExceptionMessages = GetErrorMessages(e),
                Message = message
            });
        }

        public static void CreateDefaultConsumerInvoker(this ILogger logger, MessageContext context, Exception e)
        {
            LogJson(logger, $"An exception has been thrown when create DefaultConsumerInvoker.", new
            {
                InnerExceptionMessages = GetErrorMessages(e),
                MessageContext = context
            });
        }

        public static void InvokeConsumer(this ILogger logger, MessageContext context,object msg, Exception e)
        {
            LogJson(logger, $"An exception has been thrown when invoke consumer callback handler.", new
            {
                InnerExceptionMessages = GetErrorMessages(e),
                Message = msg,
                MessageContext = context
            });
        }

        public static void UpdateReceivedMessage(this ILogger logger, object msg, Exception e)
        {
            LogJson(logger, $"An exception has been thrown when update received message[ignore].", new
            {
                InnerExceptionMessages = GetErrorMessages(e),
                Message = msg
            });
        }

        private static void LogJson(ILogger logger, string message, object data)
        {
            if (logger == null) return;
            logger.LogError($"{message}---->{data.ToJson()}");
        }

        private static IEnumerable<string> GetErrorMessages(Exception e)
        {
            var ex = e;
            var messages = new List<String>();
            while(ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return messages;
        }
    }
}
