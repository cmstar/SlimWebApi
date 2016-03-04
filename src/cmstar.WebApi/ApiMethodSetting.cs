using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含一个Web API方法的设置信息。
    /// </summary>
    public class ApiMethodSetting
    {
        private TimeSpan _cacheExpiration = TimeSpan.Zero;
        private bool _autoCacheEnabled;
        private string _methodName;

        /// <summary>
        /// 获取调用当前WebAPI所使用的名称。
        /// </summary>
        public string MethodName
        {
            get
            {
                return _methodName;
            }
            set
            {
                ArgAssert.NotNullOrEmptyOrWhitespace(value, "value");
                _methodName = value;
            }
        }

        /// <summary>
        /// 获取当前WebAPI所用缓存的超时时间。若未开启缓存，则为<see cref="TimeSpan.Zero"/>。
        /// </summary>
        public TimeSpan CacheExpiration
        {
            get
            {
                return _cacheExpiration;
            }
            set
            {
                if (value.Ticks <= 0)
                    throw new ArgumentException("The expiration time must be greater than zero.", "value");

                _cacheExpiration = value;
            }
        }

        /// <summary>
        /// 获取当前WebAPI所使用的缓存提供器。
        /// </summary>
        public IApiCacheProvider CacheProvider { get; set; }

        /// <summary>
        /// 获取当前WebAPI注册中使用的缓存命名空间。
        /// </summary>
        public string CacheNamespace { get; set; }

        /// <summary>
        /// 是否允许对当前方法进行自动缓存。
        /// </summary>
        public bool AutoCacheEnabled
        {
            get { return _autoCacheEnabled && CacheProvider != null; }
            set { _autoCacheEnabled = value; }
        }
    }
}
