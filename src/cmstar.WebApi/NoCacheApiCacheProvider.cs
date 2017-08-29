using System;
using System.Collections.Generic;

namespace cmstar.WebApi
{
    /// <summary>
    /// 一个空的缓存提供器，实际上不提供任何缓存实现。
    /// </summary>
    public class NoCacheApiCacheProvider : IEnumerableApiCacheProvider
    {
        /// <inheritdoc />
        public object Get(string key)
        {
            return null;
        }

        /// <inheritdoc />
        public void Add(string key, object value, TimeSpan expiration)
        {
        }

        /// <inheritdoc />
        public object Set(string key, object value, TimeSpan expiration)
        {
            return null;
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, object>> KeyValues(string prefix)
        {
            return new KeyValuePair<string, object>[0];
        }

        /// <inheritdoc />
        public void Clear(string prefix)
        {
        }
    }
}
