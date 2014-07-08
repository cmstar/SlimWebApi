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
        private readonly Cache _cache = HttpRuntime.Cache;

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public void Add(string key, object value, TimeSpan expiration)
        {
            Set(key, value, expiration);
        }

        public object Set(string key, object value, TimeSpan expiration)
        {
            var absoluteExpiration = DateTime.Now.Add(expiration);
            return _cache.Add(key, value, null, absoluteExpiration,
                Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

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
                        yield return new KeyValuePair<string, object>(key, e.Value);
                    }
                }
            }
            finally
            {
                var disposable = e as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        public void Clear(string prefix)
        {
            foreach (var kv in KeyValues(prefix))
            {
                _cache.Remove(kv.Key);
            }
        }
    }
}
