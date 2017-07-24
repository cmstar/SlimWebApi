using System;
using System.Collections;
using System.Collections.Generic;

namespace cmstar.WebApi
{
    /// <summary>
    /// 至多包含一个元素的<see cref="IDictionary{K, V}"/>实现。
    /// </summary>
    public class SingleEntryDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IEqualityComparer<TKey> _comparer;
        private readonly bool _readOnly;
        private bool _hasKey;
        private TKey _key;
        private TValue _value;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="equalityComparer">指定元素的比较方式。</param>
        /// <param name="readOnly">若为true，则集合在创建后不允许修改。</param>
        public SingleEntryDictionary(IEqualityComparer<TKey> equalityComparer = null, bool readOnly = false)
        {
            _readOnly = readOnly;
            _comparer = equalityComparer ?? EqualityComparer<TKey>.Default;
        }

        /// <summary>
        /// 初始化类型得新实例。
        /// </summary>
        /// <param name="key">指定元素的键。</param>
        /// <param name="value">指定元素的值。</param>
        /// <param name="equalityComparer">指定元素的比较方式。</param>
        /// <param name="readOnly">若为true，则集合在创建后不允许修改。</param>
        public SingleEntryDictionary(
            TKey key, TValue value, IEqualityComparer<TKey> equalityComparer = null, bool readOnly = false)
        {
            ArgAssert.NotNull(key, "key");

            _hasKey = true;
            _key = key;
            _value = value;

            _readOnly = readOnly;
            _comparer = equalityComparer ?? EqualityComparer<TKey>.Default;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            ((IDictionary<TKey, TValue>)this).Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (_readOnly)
                throw new InvalidOperationException("The collection is readonly.");

            _value = default(TValue);
            _key = default(TKey);
            _hasKey = false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!_hasKey || !_comparer.Equals(_key, item.Key))
                return false;

            if (_value == null && item.Value == null)
                return true;

            if (_value == null || item.Value == null)
                return false;

            return _value.Equals(item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            if (_hasKey)
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(_key, _value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return _hasKey ? 0 : 1; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return _readOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return _hasKey && _comparer.Equals(_key, key);
        }

        public void Add(TKey key, TValue value)
        {
            if (_readOnly)
                throw new InvalidOperationException("The collection is readonly.");

            if (_hasKey)
                throw new InvalidOperationException("There can not be more than one key in the collection.");

            if (key == null)
                throw new ArgumentNullException("key");

            _hasKey = true;
            _key = key;
            _value = value;
        }

        public bool Remove(TKey key)
        {
            if (_readOnly)
                throw new InvalidOperationException("The collection is readonly.");

            if (!ContainsKey(key))
                return false;

            Clear();
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (!_hasKey || !_comparer.Equals(_key, key))
            {
                value = default(TValue);
                return false;
            }

            value = _value;
            return true;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (ContainsKey(key))
                    return _value;

                throw new KeyNotFoundException();
            }
            set
            {
                Add(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _hasKey ? new[] { _key } : new TKey[0]; }
        }

        public ICollection<TValue> Values
        {
            get { return _hasKey ? new[] { _value } : new TValue[0]; }
        }

        private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            private readonly SingleEntryDictionary<TKey, TValue> _parent;
            private bool _moved;

            public Enumerator(SingleEntryDictionary<TKey, TValue> parent)
            {
                _parent = parent;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_moved || !_parent._hasKey)
                    return false;

                _moved = true;
                return true;
            }

            public void Reset()
            {
                _moved = false;
            }

            public KeyValuePair<TKey, TValue> Current
            {
                get { return new KeyValuePair<TKey, TValue>(_parent._key, _parent._value); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}
