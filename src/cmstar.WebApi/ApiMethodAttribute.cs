using System;
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
                if (!value.IsNullOrWhiteSpace())
#else
                if (!string.IsNullOrWhiteSpace(value))
#endif
                {
                    _setting.MethodName = value;
                }
            }
        }

        /// <summary>
        /// 指示缓存是否启用。
        /// </summary>
        public bool AutoCacheEnabled
        {
            get { return _setting.AutoCacheEnabled; }
            set { _setting.AutoCacheEnabled = value; }
        }

        /// <summary>
        /// 获取或设置当前API方法的缓存超时时间，单位为秒。
        /// 使用非正数表示不指定缓存超时时间，在此情况下通常将套用全局的设置。
        /// </summary>
        public int CacheExpiration
        {
            get
            {
                return (int)_setting.CacheExpiration.TotalSeconds;
            }
            set
            {
                if (value >= 0)
                {
                    _setting.CacheExpiration = TimeSpan.FromSeconds(value);
                }
            }
        }

        /// <summary>
        /// 获取或设置当前API方法的缓存命名空间。
        /// 使用<c>null</c>表示不指定，在此情况下通常将套用全局的设置。
        /// </summary>
        public string CacheNamespace
        {
            get
            {
                return _setting.CacheNamespace;
            }
            set
            {
#if NET35
                if (!value.IsNullOrWhiteSpace())
#else
                if (!string.IsNullOrWhiteSpace(value))
#endif
                {
                    _setting.CacheNamespace = value;
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
        /// 获取方法的配置信息。
        /// </summary>
        /// <returns>方法的配置信息。</returns>
        public ApiMethodSetting GetUnderlyingSetting()
        {
            return _setting;
        }
    }
}
