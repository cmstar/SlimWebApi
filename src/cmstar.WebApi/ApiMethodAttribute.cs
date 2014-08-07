using System;

namespace cmstar.WebApi
{
    /// <summary>
    /// 标记一个方法在<see cref="ApiSetup"/>中被自动注册为WebAPI方法。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ApiMethodAttribute : Attribute
    {
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
        public string Name { get; set; }

        /// <summary>
        /// 指示缓存是否启用。
        /// </summary>
        public bool AutoCacheEnabled { get; set; }

        /// <summary>
        /// 获取或设置当前API方法的缓存超时时间，单位为秒。
        /// 使用非正数表示不指定缓存超时时间，在此情况下通常将套用全局的设置。
        /// </summary>
        public int CacheExpiration { get; set; }

        /// <summary>
        /// 获取或设置当前API方法的缓存命名空间。
        /// 使用<c>null</c>表示不指定，在此情况下通常将套用全局的设置。
        /// </summary>
        public string CacheNamespace { get; set; }
    }
}
