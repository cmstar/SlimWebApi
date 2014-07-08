using System;
using System.Collections.Generic;

namespace cmstar.WebApi
{
    /// <summary>
    /// 定义API缓存提供器。
    /// </summary>
    public interface IApiCacheProvider
    {
        /// <summary>
        /// 获取缓存中具有指定键的对象。若该键不存在，返回<c>null</c>。
        /// </summary>
        /// <param name="key">缓存对象的键。</param>
        /// <returns>缓存的对象。</returns>
        object Get(string key);

        /// <summary>
        /// 向缓存中添加对象。若存在具有相同键的缓存对象，则替换之。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <param name="value">缓存的对象。</param>
        /// <param name="expiration">缓存的超时时间。</param>
        void Add(string key, object value, TimeSpan expiration);

        /// <summary>
        /// 设置缓存对象，若存在具有相同键的缓存对象不存在，则添加改对象缓存，否则替换之。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <param name="value">缓存的对象。</param>
        /// <param name="expiration">缓存的超时时间。</param>
        /// <returns>若缓存对象被替换，返回被替换的对象；否则返回<c>null</c>。</returns>
        object Set(string key, object value, TimeSpan expiration);

        /// <summary>
        /// 移除具有指定键的缓存对象。
        /// </summary>
        /// <param name="key">缓存对象的键。</param>
        void Remove(string key);
    }

    /// <summary>
    /// 定义可通过前缀匹配枚举键值的缓存提供器。
    /// </summary>
    public interface IEnumerableApiCacheProvider : IApiCacheProvider
    {
        /// <summary>
        /// 根据缓存对象的键的前缀枚举缓存的键值。
        /// </summary>
        /// <param name="prefix">键前缀。</param>
        /// <returns>具有指定键前缀的缓存的键值序列。</returns>
        IEnumerable<KeyValuePair<string, object>> KeyValues(string prefix);

        /// <summary>
        /// 根据缓存对象的键的前缀批量移除缓存对象。
        /// </summary>
        /// <param name="prefix">键前缀。</param>
        void Clear(string prefix);
    }
}
