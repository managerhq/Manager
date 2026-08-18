using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer
{
    public sealed class TypePartitionedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, Dictionary<TKey, TValue>> outerDictionary = [];
        private Dictionary<Type, Dictionary<TKey, TValue>> innerDictionaries = [];

        public Dictionary<Type, Dictionary<TKey, TValue>>.ValueCollection Values => innerDictionaries.Values;

        public TValue this[TKey key]
        {
            get
            {
                if (outerDictionary.TryGetValue(key, out Dictionary<TKey, TValue> innerDictionary))
                {
                    if (innerDictionary.TryGetValue(key, out TValue value))
                    {
                        return value;
                    }
                }
                return default;
            }
            set
            {
                if (outerDictionary.TryGetValue(key, out var oldInnerDictionary))
                {
                    oldInnerDictionary.Remove(key);
                }

                var type = value.GetType();
                var innerDictionary = innerDictionaries.GetValueOrDefault(type);
                if (innerDictionary == null)
                {
                    innerDictionary = [];
                    innerDictionaries.Add(type, innerDictionary);
                }
                innerDictionary[key] = value;
                outerDictionary[key] = innerDictionary;
            }
        }

        public int Count => outerDictionary.Count;
        public bool ContainsKey(TKey key) => outerDictionary.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (outerDictionary.TryGetValue(key, out var innerDictionary))
            {
                if (innerDictionary.TryGetValue(key, out var innerDictionaryValue))
                {
                    value = innerDictionaryValue;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            var type = value.GetType();
            var innerDictionary = innerDictionaries.GetValueOrDefault(type);
            if (innerDictionary == null)
            {
                innerDictionary = new Dictionary<TKey, TValue>();
                innerDictionaries.Add(type, innerDictionary);
            }
            innerDictionary.TryAdd(key, value); // TryAdd because Key is string and there could be duplicates for some reason. Remove when key is byte[]
            outerDictionary.TryAdd(key, innerDictionary);
        }

        public int GetCount<T>()
        {
            if (typeof(T).IsSealed)
            {
                return innerDictionaries.GetValueOrDefault(typeof(T))?.Count ?? 0;
            }
            else
            {
                return innerDictionaries
                    .Where(x => x.Key.IsAssignableTo(typeof(T)))
                    .Sum(x => x.Value.Count);
            }
        }

        public IEnumerable<T> OfType<T>()
        {
            return OfType(typeof(T)).Cast<T>();
        }

        public IEnumerable<TValue> OfType(Type type)
        {
            if (type.IsSealed)
            {
                return innerDictionaries.GetValueOrDefault(type)?.Values.Cast<TValue>() ?? [];
            }
            else
            {
                return innerDictionaries
                    .Where(x => x.Key.IsAssignableTo(type))
                    .SelectMany(x => x.Value.Values);
            }
        }

        public bool Remove(TKey key)
        {
            var innerDictionary = outerDictionary.GetValueOrDefault(key);
            if (innerDictionary != null) innerDictionary.Remove(key);
            return outerDictionary.Remove(key);
        }
    }
}
