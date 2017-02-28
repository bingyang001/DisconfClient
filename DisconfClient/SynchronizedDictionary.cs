using System;
using System.Collections;
using System.Collections.Generic;

namespace DisconfClient
{
    [Serializable]
    public class SynchronizedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;
        private readonly object _syncRoot = new object();
        public SynchronizedDictionary(IEqualityComparer<TKey> comparer = null)
        {
            _dictionary = new Dictionary<TKey, TValue>(comparer);
        }
        public SynchronizedDictionary(IDictionary<TKey, TValue> dictionaries)
        {
            _dictionary = dictionaries;
        }

        #region IDictionary<TKey,TValue> Members

        public object SyncRoot
        {
            get
            {
                return _syncRoot;
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (_syncRoot)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _dictionary.Count;
                }
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_syncRoot)
                {
                    TValue value;
                    _dictionary.TryGetValue(key, out value);
                    return value;
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    _dictionary[key] = value;
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (_syncRoot)
                {
                    return _dictionary.Keys;
                }
            }
        }


        public void Add(TKey key, TValue value)
        {
            if (_dictionary.ContainsKey(key)) return;
            lock (_syncRoot)
            {
                if (!_dictionary.ContainsKey(key))
                {
                    _dictionary.Add(key, value);
                }
            }
        }

        public TValue GetOrSet(TKey key, Func<TValue> func = null)
        {
            TValue value;
            bool flag = _dictionary.TryGetValue(key, out value);
            if (flag)
                return value;
            if (func == null)
                return value;
            lock (_syncRoot)
            {
                value = func();
                _dictionary[key] = value;
            }
            return value;
        }

        public bool Remove(TKey key)
        {
            lock (_syncRoot)
            {
                return _dictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_syncRoot)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }

        public TValue GetWithNoLock(TKey key)
        {
            TValue value;
            _dictionary.TryGetValue(key, out value);
            return value;
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (_syncRoot)
                {
                    return _dictionary.Values;
                }
            }
        }
        #endregion


        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.ContainsKey(item.Key)) return;
            lock (_syncRoot)
            {
                if (!_dictionary.ContainsKey(item.Key))
                {
                    _dictionary.Add(item);
                }
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _dictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (_syncRoot)
            {
                return _dictionary.Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (_syncRoot)
            {
                this._dictionary.CopyTo(array, arrayIndex);
            }
        }


        public bool IsReadOnly
        {
            get
            {
                return _dictionary.IsReadOnly;
            }
        }


        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (_dictionary.ContainsKey(item.Key)) return false;
            lock (_syncRoot)
            {
                if (!_dictionary.ContainsKey(item.Key))
                {
                    return _dictionary.Remove(item);
                }
            }
            return false;
        }

        #endregion


        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _dictionary.GetEnumerator();
            }
        }

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_syncRoot)
            {
                return _dictionary.GetEnumerator();
            }
        }

        public void Foreach(Action<KeyValuePair<TKey, TValue>> action)
        {
            if (action == null)
                return;
            TKey[] keys = new TKey[this.Keys.Count];
            this.Keys.CopyTo(keys, 0);
            foreach (TKey key in keys)
            {
                TValue value;
                bool flag = this.TryGetValue(key, out value);
                if (!flag) continue;
                KeyValuePair<TKey, TValue> pair = new KeyValuePair<TKey, TValue>(key, value);
                action(pair);
            }
        }
        #endregion

    }
}
