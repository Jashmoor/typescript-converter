using System;
using System.Collections;
using System.Collections.Generic;

namespace TypescriptClassConverter.Collections
{
    public class TypeConversions : IReadOnlyDictionary<Type, string>
    {
        private IDictionary<Type, string> _Collection;

        public TypeConversions()
        {
            _Collection = new Dictionary<Type, string>();
        }

        public void Add(Type type, string definition)
        {
            _Collection.Add(type, definition);
        }

        public IEnumerator<KeyValuePair<Type, string>> GetEnumerator()
            => _Collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int Count => _Collection.Count;

        public bool ContainsKey(Type key)
            => _Collection.ContainsKey(key);

        public bool TryGetValue(Type key, out string value)
            => _Collection.TryGetValue(key, out value);

        public string this[Type key]
        {
            get => _Collection[key];
            set => _Collection[key] = value;
        }

        public IEnumerable<Type> Keys => _Collection.Keys;
        public IEnumerable<string> Values => _Collection.Values;
    }
}
