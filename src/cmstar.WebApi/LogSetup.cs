using System;
using System.Web;
using Common.Logging;

namespace cmstar.WebApi
{
    /// <summary>
    /// API日志的相关配置。
    /// </summary>
    public class LogSetup : ICloneable
    {
        /// <summary>
        /// API请求处理成功时的默认日志级别。
        /// <see cref="Default"/>方法创建的实例使用此日志级别。
        /// </summary>
        public const LogLevel DefaultSuccessLogLevel = LogLevel.Debug;

        /// <summary>
        /// 获取一个新的<see cref="LogSetup"/>实例，其中的各项配置已初始化为预定义的值。
        /// </summary>
        /// <returns></returns>
        public static LogSetup Default()
        {
            return new LogSetup
            {
                LoggerName = null,
                SuccessLogLevel = LogLevel.Debug,
                Code400LogLevel = LogLevel.Warn
            };
        }

        private LogSetup() { }

        /// <summary>
        /// 指定日志名称。默认为null，此时自动套用API提供类，即<see cref="ApiHttpHandlerBase"/>的派生类的名称。
        /// </summary>
        public string LoggerName;

        /// <summary>
        /// 指定API请求处理成功时的日志级别。默认为 DEBUG 。
        /// </summary>
        public LogLevel SuccessLogLevel;

        /// <summary>
        /// 指定API请求处理结果状态码为400时的日志级别。默认为 WARN 。
        /// </summary>
        public LogLevel Code400LogLevel;

        /// <inheritdoc />
        public object Clone()
        {
            // 都是简单类型，直接用浅表拷贝即可
            return MemberwiseClone();
        }
    }
}
