using System;
using System.Collections;
using System.Collections.Generic;

namespace cmstar.WebApi.NetFuture
{
    /// <summary>
    /// Represents a thread-safe collection of key-value pairs that can be accessed by multiple threads concurrently.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly IEqualityComparer<TValue> _valueComparer;

        /// <summary>
        /// Initializes a new instance of the class that is empty, has the default concurrency level, 
        /// has the default initial capacity, and uses the default comparer for the key type.
        /// </summary>
        public ConcurrentDictionary()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class that is empty, has the default concurrency level 
        /// and capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="equalityComparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.
        /// </param>
        public ConcurrentDictionary(IEqualityComparer<TKey> equalityComparer)
        {
            _dictionary = new Dictionary<TKey, TValue>(equalityComparer);
            _valueComparer = EqualityComparer<TValue>.Default;
        }

        /// <summary>
        /// Gets a value that indicates whether the dictionary is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                lock (_dictionary)
                {
                    return _dictionary.Count == 0;
                }
            }
        }

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be a null reference for reference types.
        /// </param>
        /// <returns>
        /// true if the key/value pair was added successfully. If the key already exists, returns false.
        /// </returns>
        public bool TryAdd(TKey key, TValue value)
        {
            lock (_dictionary)
            {
                if (_dictionary.ContainsKey(key))
                    return false;

                _dictionary.Add(key, value);
                return true;
            }
        }

        /// <summary>
        /// Attempts to remove and return the value with the specified key from the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to remove and return.</param>
        /// <param name="value">
        /// When this method returns, value contains the object removed from the dictionary 
        /// or the default value of if the operation failed.</param>
        /// <returns>true if an object was removed successfully; otherwise, false.</returns>
        public bool TryRemove(TKey key, out TValue value)
        {
            lock (_dictionary)
            {
                if (!_dictionary.TryGetValue(key, out value))
                    return false;

                _dictionary.Remove(key);
                return true;
            }
        }

        /// <summary>
        /// Compares the existing value for the specified key with a specified value, 
        /// and if they are equal, updates the key with a third value.
        /// </summary>
        /// <param name="key">The key whose value is compared with comparisonValue and possibly replaced.</param>
        /// <param name="newValue">
        /// The value that replaces the value of the element with key if the comparison results in equality.
        /// </param>
        /// <param name="comparisonValue">The value that is compared to the value of the element with key.</param>
        /// <returns>
        /// true if the value with key was equal to comparisonValue and replaced with newValue; otherwise, false.
        /// </returns>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            lock (_dictionary)
            {
                TValue oldValue;
                if (!_dictionary.TryGetValue(key, out oldValue))
                    return false;

                if (!_valueComparer.Equals(oldValue, comparisonValue))
                    return false;

                _dictionary[key] = newValue;
                return true;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the dictionary if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">the value to be added, if the key does not already exist.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key 
        /// if the key is already in the dictionary, or the new value if the key was not in the dictionary.
        /// </returns>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            lock (_dictionary)
            {
                TValue oldValue;
                if (_dictionary.TryGetValue(key, out oldValue))
                    return oldValue;

                _dictionary.Add(key, value);
                return value;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the dictionary if the key does not already exist.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">The function used to generate a value for the key.</param>
        /// <returns>
        /// The value for the key. This will be either the existing value for the key if the key 
        /// is already in the dictionary, or the new value for the key as returned by valueFactory 
        /// if the key was not in the dictionary.
        /// </returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            lock (_dictionary)
            {
                TValue oldValue;
                if (_dictionary.TryGetValue(key, out oldValue))
                    return oldValue;

                var newValue = valueFactory(key);

                // the factory method may set the value, so...
                // recheck after the invokation of the factory method
                if (_dictionary.TryGetValue(key, out oldValue))
                    return oldValue;

                _dictionary.Add(key, newValue);
                return newValue;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the dictionary if the key does not already exist, 
        /// or updates a key/value pair in the dictionary if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated.</param>
        /// <param name="addValue">The value to be added for an absent key.</param>
        /// <param name="updateValueFactory">
        /// The function used to generate a new value for an existing key based on the key's existing value.
        /// </param>
        /// <returns>
        /// The new value for the key. This will be either be addValue (if the key was absent) or the result 
        /// of updateValueFactory (if the key was present).
        /// </returns>
        public TValue AddOrUpdate(TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            lock (_dictionary)
            {
                TValue oldValue;
                if (!_dictionary.TryGetValue(key, out oldValue))
                {
                    _dictionary.Add(key, addValue);
                    return addValue;
                }

                var newValue = updateValueFactory(key, oldValue);
                _dictionary[key] = newValue;
                return newValue;
            }
        }

        /// <summary>
        /// Adds a key/value pair to the dictionary if the key does not already exist, 
        /// or updates a key/value pair in the dictionary if the key already exists.
        /// </summary>
        /// <param name="key">The key to be added or whose value should be updated.</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
        /// <param name="updateValueFactory">
        /// The function used to generate a new value for an existing key based on the key's existing value.
        /// </param>
        /// <returns>
        /// The new value for the key. This will be either be the result of addValueFactory (if the key was absent) 
        /// or the result of updateValueFactory (if the key was present).
        /// </returns>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            lock (_dictionary)
            {
                TValue oldValue, newValue;
                if (!_dictionary.TryGetValue(key, out oldValue))
                {
                    newValue = addValueFactory(key);

                    // the factory method may set the value, so...
                    // recheck after the invokation of the factory method
                    if (_dictionary.TryGetValue(key, out oldValue))
                        goto updateValue;

                    _dictionary.Add(key, newValue);
                    return newValue;
                }

            updateValue:
                newValue = updateValueFactory(key, oldValue);
                _dictionary[key] = newValue;
                return newValue;
            }
        }

        public void Clear()
        {
            lock (_dictionary)
            {
                _dictionary.Clear();
            }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            lock (_dictionary)
            {
                return _dictionary.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_dictionary)
            {
                return _dictionary.GetEnumerator();
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            lock (_dictionary)
            {
                _dictionary.Add(item.Key, item.Value);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue val;
            return TryGetValue(item.Key, out val) && _valueComparer.Equals(item.Value, val);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_dictionary)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (_dictionary)
            {
                TValue val;
                if (!_dictionary.TryGetValue(item.Key, out val))
                    return false;

                if (!_valueComparer.Equals(val, item.Value))
                    return false;

                return _dictionary.Remove(item.Key);
            }
        }

        public int Count => _dictionary.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public bool ContainsKey(TKey key)
        {
            lock (_dictionary)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            lock (_dictionary)
            {
                _dictionary.Add(key, value);
            }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            lock (_dictionary)
            {
                return _dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_dictionary)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_dictionary)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_dictionary)
                {
                    _dictionary[key] = value;
                }
            }
        }

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _dictionary.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _dictionary.Values;
    }
}