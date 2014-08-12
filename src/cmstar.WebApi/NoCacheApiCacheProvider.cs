using System;
using System.Collections.Generic;

namespace cmstar.WebApi
{
    /// <summary>
    /// 一个空的缓存提供器，实际上不提供任何缓存实现。
    /// </summary>
    public class NoCacheApiCacheProvider : IEnumerableApiCacheProvider
    {
        public object Get(string key)
        {
            return null;
        }

        public void Add(string key, object value, TimeSpan expiration)
        {
        }

        public object Set(string key, object value, TimeSpan expiration)
        {
            return null;
        }

        public void Remove(string key)
        {
        }

        public IEnumerable<KeyValuePair<string, object>> KeyValues(string prefix)
        {
            return new KeyValuePair<string, object>[0];
        }

        public void Clear(string prefix)
        {
        }
    }
}
