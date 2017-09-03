using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace EventBus.Core.Infrastructure
{
    public class DefaultPubMessageValidator : IPubMessageValidator
    {
        public void Validate(PubMessageValidateContext context)
        {
            if (context.MessageType.GetTypeInfo().GetCustomAttribute<EventAttribute>() == null)
            {
                context.Result.AddError("events must be marked with EventAttribute");
            }
        }
    }
}
