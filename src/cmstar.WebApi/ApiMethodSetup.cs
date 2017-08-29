using System;
using System.Collections.Generic;
using Common.Logging;

namespace cmstar.WebApi
{
    /// <summary>
    /// 提供API方法注册的便捷入口。
    /// </summary>
    public class ApiMethodSetup
    {
        private readonly ApiMethodInfo _apiMethodInfo;

        /// <summary>
        /// 初始化<see cref="ApiMethodSetup"/>的新实例。
        /// </summary>
        /// <param name="setup">调用此构造函数的<see cref="ApiSetup"/>实例。</param>
        /// <param name="apiMethodInfo">注册的API方法的有关信息。</param>
        public ApiMethodSetup(ApiSetup setup, ApiMethodInfo apiMethodInfo)
        {
            ArgAssert.NotNull(setup, "setup");
            ArgAssert.NotNull(apiMethodInfo, "apiMethodInfo");

            apiMethodInfo.Setting.CacheProvider = setup.CacheProvider;

            if (setup.CacheExpiration.Ticks > 0)
            {
                apiMethodInfo.Setting.CacheExpiration = setup.CacheExpiration;
            }

            // 缓存命名空间优先使用承载方法的类型的名称
            apiMethodInfo.Setting.CacheNamespace = apiMethodInfo.Method.DeclaringType != null
                ? apiMethodInfo.Method.DeclaringType.FullName
                : setup.CallerType.FullName;

            _apiMethodInfo = apiMethodInfo;
        }

        /// <summary>
        /// 设置API方法的名称。
        /// </summary>
        /// <returns>API方法的名称。</returns>
        public string Name()
        {
            return _apiMethodInfo.Setting.MethodName;
        }

        /// <summary>
        /// 设置API方法的名称。若未使用此方法设置名称，将使用默认的名称（一般同注册的方法名）。
        /// </summary>
        /// <param name="name">API方法的名称，可以是任意非空字符串。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup Name(string name)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(name, "name");
            _apiMethodInfo.Setting.MethodName = name;
            return this;
        }

        /// <summary>
        /// 设置当前WebAPI成功执行后输出日志信息所使用的日志级别。
        /// </summary>
        public ApiMethodSetup SuccessLogLevel(LogLevel logLevel)
        {
            _apiMethodInfo.Setting.SuccessLogLevel = logLevel;
            return this;
        }

#if !NET35
        /// <summary>
        /// 获取异步的WebAPI方法的超时时间，单位为秒。
        /// 0表示不会超时，-1（或其他负数）表示使用默认的超时（<see cref="ApiEnvironment.AsyncTimeout"/>），若未设置，默认值为 -1。
        /// 若异步调用超过了此时间，HTTP请求以500状态码返回。
        /// 异步方法本身并不会因此结束，需要从相关的日志与调试信息中获取此情况的有关信息以便进行优化。 
        /// </summary>
        /// <param name="timeoutSeconds">异步的WebAPI方法的超时时间，单位为秒。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup AsyncTimeout(int timeoutSeconds)
        {
            _apiMethodInfo.Setting.AsyncTimeout = timeoutSeconds;
            return this;
        }
#endif

        /// <summary>
        /// 获取输出结果所使用的压缩方式。
        /// </summary>
        /// <returns>输出结果所使用的压缩方式。</returns>
        public ApiCompressionMethods Compression()
        {
            return _apiMethodInfo.Setting.CompressionMethods;
        }

        /// <summary>
        /// 指定输出结果所使用的压缩方式。
        /// </summary>
        /// <param name="compressionMethod">API数据输出时的压缩方式。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup Compression(ApiCompressionMethods compressionMethod)
        {
            _apiMethodInfo.Setting.CompressionMethods = compressionMethod;
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
            _apiMethodInfo.Setting.CacheProvider = cacheProvider;
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
                    nameof(expiration), "The expiration time must be greater than zero.");

            _apiMethodInfo.Setting.CacheExpiration = expiration;
            return this;
        }

        /// <summary>
        /// 为当前注册的方法单独指定缓存命名空间。
        /// </summary>
        /// <param name="ns">缓存命名空间。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup CacheNamespace(string ns)
        {
            _apiMethodInfo.Setting.CacheNamespace = ns ?? string.Empty;
            return this;
        }

        /// <summary>
        /// 开启API方法的自动缓存。
        /// 没有被指定的缓存配置将套用<see cref="ApiSetup"/>中设置的缓存配置。
        /// </summary>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup EnableAutoCache()
        {
            if (_apiMethodInfo.Setting.CacheProvider == null)
            {
                throw new InvalidOperationException(
                    "The cache provider must be specified if the base provider is not set.");
            }

            _apiMethodInfo.Setting.AutoCacheEnabled = true;
            return this;
        }

        /// <summary>
        /// 设置一个方法，在当前WebAPI被调用前执行此方法。
        /// 此方法的第一个参数指向当前被调用的WebAPI方法的注册信息；
        /// 第二个参数为当前被调用的WebAPI方法的参数表。
        /// </summary>
        /// <param name="callback">回调方法，若为null，则不会被执行。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup MethodInvoking(
            Action<ApiMethodInfo, IDictionary<string, object>> callback)
        {
            _apiMethodInfo.Setting.MethodInvoking = callback;
            return this;
        }

        /// <summary>
        /// 设置一个方法，在当前WebAPI被调用后（即便调用期间出现异常）执行此方法。
        /// 此方法的第一个参数指向当前被调用的WebAPI方法的注册信息；
        /// 第二个参数为当前被调用的WebAPI方法的参数表；
        /// 第三个参数为当前被调用的WebAPI方法的返回值，若方法没有返回值，或调用期间出现异常，则为null；
        /// 第四个参数为WebAPI方法调用过程中的异常，若没有异常，则为null。
        /// </summary>
        /// <param name="callback">回调方法，若为null，则不会被执行。</param>
        /// <returns>当前<see cref="ApiMethodSetup"/>实例。</returns>
        public ApiMethodSetup MethodInvoked(
            Action<ApiMethodInfo, IDictionary<string, object>, object, Exception> callback)
        {
            _apiMethodInfo.Setting.MethodInvoked = callback;
            return this;
        }
    }
}