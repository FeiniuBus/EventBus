using System.Collections;
using System.Collections.Generic;

namespace EventBus.Core.Infrastructure
{
    public interface IMetaData
    {
        string this[string name] { get; set; }
        void Set(string name, string value);
        void Remove(string name);
        string Get(string name);
        void Unoin(IMetaData metaData);
        IEnumerator<KeyValuePair<string, string>> GetEnumerator();
    }
}
