using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Machine.Specifications;

namespace DocaLabs.Testing.Common.MSpec
{
    public static class CollectionExtensions
    {
        public static KeyValuePair<TKey, TValue> ShouldContainKeyValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            var pair = new KeyValuePair<TKey, TValue>(key, value);
            if (!dictionary.Contains(pair))
                throw new SpecificationException(string.Format("The dictionary should contain key {0} and value {1}", key, value));

            return pair;
        }

        public static void ShouldContainOnly(this NameValueCollection collection, params KeyValuePair<string, string> [] valuePairs)
        {
            if( collection.Count != valuePairs.Length)
                throw new SpecificationException(string.Format("The collection should contain {0} items but was {1}", valuePairs.Length, collection.Count));

            foreach (var keyValuePair in valuePairs)
            {
                if(!Equals(keyValuePair.Value, collection[keyValuePair.Key]))
                    throw new SpecificationException(string.Format("The collection should contain key {0} and value {1} but the value was {2}.", keyValuePair.Key, keyValuePair.Value, collection[keyValuePair.Key]));
            }
        }

        public static void ActionOnEachElement<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var element in sequence)
            {
                action(element);
            }
        }

        public static T[] GenerateSequence<T>(Func<int,T> getInstance, int count)
        {
            var sequence = new T[count];

            for (var i = 0; i < count; i++)
            {
                sequence[i] = getInstance(i);
            }

            return sequence;
        }
    }
}
