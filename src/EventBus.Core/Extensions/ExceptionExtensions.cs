using System;
using System.Collections.Generic;

namespace EventBus.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static IEnumerable<string> GetMessages(this Exception e)
        {
            var messages = new List<string>();
            Exception ex = e;
            while(ex != null)
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            return messages;
        }
    }
}
