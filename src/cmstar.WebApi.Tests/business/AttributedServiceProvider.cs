using System;
using Common.Logging;

namespace cmstar.WebApi
{
    public class AttributedServiceProvider
    {
        [ApiMethod]
        public int Zero()
        {
            // 可以通过 ApiMethodContext.Current.Raw 直接访问当前请求的 HttpContext。
            // 当然，也可以直接通过 HttpContext.Current。
            ApiMethodContext.Current.Raw.Response.Headers.Add(
                "Custom-Head",
                $"raw {ApiMethodContext.Current.Raw != null}");

            return 0;
        }

        // 单独指定方法成功执行时输出日志的日志级别
        [ApiMethod(SuccessLogLevel = LogLevel.Fatal)]
        public void FatalLog() { }

        [ApiMethod(SuccessLogLevel = LogLevel.Trace)]
        public void TraceLog() { }

        [ApiMethod]
        public DateTime Now()
        {
            return DateTime.Now;
        }

        [ApiMethod(CompressionMethods = ApiCompressionMethods.GZip)]
        public string ForceGzipString()
        {
            return Guid.NewGuid().ToString();
        }

        [ApiMethod(CompressionMethods = ApiCompressionMethods.Defalte)]
        public string ForceDeflateString()
        {
            return Guid.NewGuid().ToString();
        }

        [ApiMethod(CompressionMethods = ApiCompressionMethods.Auto)]
        public string AutoCompressionString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}