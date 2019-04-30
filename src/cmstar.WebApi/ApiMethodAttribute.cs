using System;
using Common.Logging;

#if  NET35
using cmstar.WebApi.NetFuture;
#endif

namespace cmstar.WebApi
{
    /// <summary>
    /// 标记一个方法在<see cref="ApiSetup"/>中被自动注册为WebAPI方法，
    /// 并提供方法的行为配置所需的入口。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ApiMethodAttribute : Attribute
    {
        private readonly ApiMethodSetting _setting = new ApiMethodSetting();

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        public ApiMethodAttribute()
        {
        }

        /// <summary>
        /// 初始化类型的新实例并指定API方法的名称。
        /// </summary>
        /// <param name="name">API方法的名称。</param>
        public ApiMethodAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 获取或设置API方法的名称。若设置为<c>null</c>则表示使用默认的名称。
        /// </summary>
        public string Name
        {
            get
            {
                return _setting.MethodName;
            }
            set
            {
#if NET35
                if (!StringUtils.IsNullOrWhiteSpace(value))
#else
                if (!string.IsNullOrWhiteSpace(value))
#endif
                {
                    _setting.MethodName = value;
                }
            }
        }

        /// <summary>
        /// 指定对于此API方法使用何种压缩方式输出结果。
        /// 默认值为<see cref="ApiCompressionMethods.None"/>。
        /// </summary>
        public ApiCompressionMethods CompressionMethods
        {
            get { return _setting.CompressionMethods; }
            set { _setting.CompressionMethods = value; }
        }

        /// <summary>
        /// 指定此API方法成功执行后输出日志信息所使用的日志级别。
        /// </summary>
        public LogLevel SuccessLogLevel
        {
            get { return _setting.SuccessLogLevel; }
            set { _setting.SuccessLogLevel = value; }
        }

#if !NET35
        /// <summary>
        /// 获取或设置异步的WebAPI方法的超时时间，单位为秒。
        /// 0表示不会超时，-1（或其他负数）表示使用默认的超时（<see cref="ApiEnvironment.AsyncTimeout"/>），默认值为 -1。
        /// 若异步调用超过了此时间，HTTP请求以500状态码返回。
        /// 异步方法本身并不会因此结束，需要从相关的日志与调试信息中获取此情况的有关信息以便进行优化。 
        /// </summary>
        public int AsyncTimeout
        {
            get { return _setting.AsyncTimeout; }
            set { _setting.AsyncTimeout = value; }
        }
#endif

        /// <summary>
        /// 获取方法的配置信息。
        /// </summary>
        /// <returns>方法的配置信息。</returns>
        public ApiMethodSetting GetUnderlyingSetting()
        {
            return _setting;
        }
    }
}
