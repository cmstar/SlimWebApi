using System;
using Common.Logging;

namespace cmstar.WebApi
{
    public class AttributedServiceProvider
    {
        private int _value;

        // 标记API方法，并没有开启缓存
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

        // 在特性上配置缓存超时并开启自动缓存
        [ApiMethod(AutoCacheEnabled = true, CacheExpiration = 3)]
        public DateTime Now()
        {
            return DateTime.Now;
        }

        // 使用ApiMethodContext手工管理缓存，以应对特殊的场景（比如API内同时有读写）
        [ApiMethod(CacheExpiration = 5)]
        public int ManualCache()
        {
            // 此方法使用ApiMethodContext手工管理缓存
            var value = ApiMethodContext.Current.GetCachedResult();
            if (value != null)
                return (int)value;

            _value++;
            ApiMethodContext.Current.SetCachedResult(_value);
            return _value;
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