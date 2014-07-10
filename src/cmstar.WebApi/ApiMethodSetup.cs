using System;

namespace cmstar.WebApi
{
    /// <summary>
    /// 提供API方法注册的便捷入口。
    /// </summary>
    public class ApiMethodSetup
    {
        private readonly ApiMethodInfo _apiMethodInfo;
        private readonly ApiSetup _setup;

        /// <summary>
        /// 初始化<see cref="ApiMethodSetup"/>的新实例。
        /// </summary>
        /// <param name="setup">调用此构造函数的<see cref="ApiSetup"/>实例。</param>
        /// <param name="apiMethodInfo">注册的API方法的有关信息。</param>
        public ApiMethodSetup(ApiSetup setup, ApiMethodInfo apiMethodInfo)
        {
            ArgAssert.NotNull(apiMethodInfo, "apiMethodInfo");
            _setup = setup;
            _apiMethodInfo = apiMethodInfo;
        }

        /// <summary>
        /// 设置API方法的名称。若未使用此方法设置名称，将使用默认的名称（一般同注册的方法名）。
        /// </summary>
        /// <param name="name">API方法的名称，可以是任意非空字符串。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup Name(string name)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(name, "name");
            _apiMethodInfo.MethodName = name;
            return this;
        }

        /// <summary>
        /// 开启API方法的缓存。
        /// 可以使用参数默认值以便套用<see cref="ApiSetup"/>中设置的缓存参数，也可以单独指定。
        /// </summary>
        /// <param name="expiration">
        /// 缓存的超时时间。若不指定，则使用<see cref="ApiSetup"/>中设置的时间。
        /// </param>
        /// <param name="cacheProvider">
        /// 缓存提供器。若不指定，则使用<see cref="ApiSetup"/>中设置的提供器。
        /// </param>
        /// <param name="cacheNamespace">
        /// 次方法所使用的缓存键的命名空间，若不指定，则使用<see cref="ApiSetup"/>中设置的命名空间。
        /// </param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup EnableCache(
            TimeSpan? expiration = null,
            IApiCacheProvider cacheProvider = null,
            string cacheNamespace = null)
        {
            var provider = cacheProvider ?? _setup.CacheProvider;
            if (provider == null)
            {
                throw new ArgumentException(
                    "The cache provider must be specified if the base provider is not set.", "cacheProvider");
            }

            _apiMethodInfo.CacheProvider = provider;
            _apiMethodInfo.CacheNamespace = cacheNamespace ?? _setup.CacheNamespace;
            _apiMethodInfo.CacheExpiration = expiration.HasValue ? expiration.Value : _setup.CacheExpiration;

            return this;
        }
    }
}