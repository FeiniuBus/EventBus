using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBus.Core
{
    public class PubMessageValidateResult
    {
        public IList<string> Errors { get; }

        public PubMessageValidateResult()
        {
            Errors = new List<string>();
        }

        public void AddError(string error) => Errors.Add(error);

        public bool Failed => Errors.Any();
    }
}
