using System;
using System.Collections.Generic;

namespace EventBus.Core.Infrastructure
{
    public class MetaData : IMetaData
    {
        private readonly IDictionary<string, string> _source;

        public MetaData()
        {
            _source = new Dictionary<string, string>();
        }
        public string this[string name]
        {
            get
            {
                return Get(name);
            }
            set
            {
                Set(name, value);
            }
        }

        public void Set(string name, string value)
        {
            _source[name] = value;
        }

        public string Get(string name)
        {
            if (_source.TryGetValue(name, out string value)) return value;
            else throw new IndexOutOfRangeException($"MetaData do net have element named '{name}'.");
        }

        public void Remove(string name)
        {
            _source.Remove(name);
        }

        public void Unoin(IMetaData metaData)
        {
            var iterator = GetEnumerator();
            while (iterator.MoveNext())
            {
                string key = iterator.Current.Key;
                if (_source.ContainsKey(iterator.Current.Key))
                {
                    key = GetNewKey(iterator.Current.Key);
                }
                _source.Add(key, iterator.Current.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _source.GetEnumerator();

        private string GetNewKey(string key)
        {
            int index = 1;
            var newKey = $"{key}_{index}";
            while (_source.ContainsKey(newKey))
            {
                index = index + 1;
                newKey = $"{key}_{index}";
            }
            return newKey;
        }
    }
}
