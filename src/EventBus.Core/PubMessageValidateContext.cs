using System;

namespace EventBus.Core
{
    public class PubMessageValidateContext
    {
        public PubMessageValidateContext()
        {
            Result = new PubMessageValidateResult();
        }
        public Type MessageType { get; set; }

        public object Message { get; set; }

        public PubMessageValidateResult Result { get; }
    }
}
