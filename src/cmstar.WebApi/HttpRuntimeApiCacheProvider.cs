using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace cmstar.WebApi
{
    /// <summary>
    /// 使用ASP.net缓存的缓存提供器。
    /// </summary>
    public class HttpRuntimeApiCacheProvider : IEnumerableApiCacheProvider
    {
        private static readonly Guid NullValuePlaceholder = Guid.NewGuid();
        private readonly Cache _cache = HttpRuntime.Cache;

        /// <inheritdoc />
        public object Get(string key)
        {
            var value = _cache.Get(key);
            return NullValuePlaceholder.Equals(value) ? null : value;
        }

        /// <inheritdoc />
        public void Add(string key, object value, TimeSpan expiration)
        {
            Set(key, value, expiration);
        }

        /// <inheritdoc />
        public object Set(string key, object value, TimeSpan expiration)
        {
            var absoluteExpiration = DateTime.Now.Add(expiration);
            var oldValue = _cache.Add(key, value ?? NullValuePlaceholder, null, absoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
            return NullValuePlaceholder.Equals(oldValue) ? null : oldValue;
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, object>> KeyValues(string prefix)
        {
            var e = _cache.GetEnumerator();
            try
            {
                while (e.MoveNext())
                {
                    var key = e.Key.ToString();
                    if (string.IsNullOrEmpty(prefix) || key.StartsWith(prefix))
                    {
                        var value = NullValuePlaceholder.Equals(e.Value) ? null : e.Value;
                        yield return new KeyValuePair<string, object>(key, value);
                    }
                }
            }
            finally
            {
                var disposable = e as IDisposable;
                disposable?.Dispose();
            }
        }

        /// <inheritdoc />
        public void Clear(string prefix)
        {
            foreach (var kv in KeyValues(prefix))
            {
                _cache.Remove(kv.Key);
            }
        }
    }
}
