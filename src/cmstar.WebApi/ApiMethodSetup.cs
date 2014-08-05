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
        /// 为当前注册的方法单独指定缓存提供器。
        /// </summary>
        /// <param name="cacheProvider">缓存提供器。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup CacheProvider(IApiCacheProvider cacheProvider)
        {
            ArgAssert.NotNull(cacheProvider, "cacheProvider");
            _apiMethodInfo.CacheProvider = cacheProvider;
            return this;
        }

        /// <summary>
        /// 为当前注册的方法单独指定缓存超时时间。
        /// </summary>
        /// <param name="expiration"></param>
        /// <returns>当前<see cref="ApiMethodSetup"/>超时时间。</returns>
        public ApiMethodSetup CacheExpiration(TimeSpan expiration)
        {
            if (expiration.Ticks <= 0)
                throw new ArgumentOutOfRangeException(
                    "expiration", "The expiration time must be greater than zero.");

            _apiMethodInfo.CacheExpiration = expiration;
            return this;
        }

        /// <summary>
        /// 为当前注册的方法单独指定缓存命名空间。
        /// </summary>
        /// <param name="ns">缓存命名空间。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup CacheNamespace(string ns)
        {
            _apiMethodInfo.CacheNamespace = ns ?? string.Empty;
            return this;
        }

        /// <summary>
        /// 开启API方法的自动缓存。
        /// 没有被指定的缓存配置将套用<see cref="ApiSetup"/>中设置的缓存配置。
        /// </summary>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup EnableAutoCache()
        {
            if (_apiMethodInfo.CacheProvider == null)
            {
                _apiMethodInfo.CacheProvider = _setup.CacheProvider;
            }

            if (_apiMethodInfo.CacheProvider == null)
            {
                throw new InvalidOperationException(
                    "The cache provider must be specified if the base provider is not set.");
            }

            if (_apiMethodInfo.CacheExpiration == TimeSpan.Zero)
            {
                _apiMethodInfo.CacheExpiration = _setup.CacheExpiration;
            }

            // 在没有显式指定命名空间时，优先使用承载方法的类型的名称
            if (_apiMethodInfo.CacheNamespace == null)
            {
                _apiMethodInfo.CacheNamespace = _apiMethodInfo.Method.DeclaringType != null
                    ? _apiMethodInfo.Method.DeclaringType.FullName
                    : _setup.CallerType.FullName;
            }

            _apiMethodInfo.AutoCacheEnabled = true;
            return this;
        }
    }
}