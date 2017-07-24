using System;

namespace cmstar.WebApi
{
    public class AttributedServiceProvider
    {
        private int _value;

        // 标记API方法，并没有开启缓存
        [ApiMethod]
        public int Zero()
        {
            return 0;
        }

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