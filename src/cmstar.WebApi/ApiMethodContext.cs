using System;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含一次API方法调用期间的有关信息。
    /// </summary>
    public class ApiMethodContext
    {
        private readonly static ApiMethodContext EmptyContext;

        [ThreadStatic]
        private static ApiMethodContext _currentContext;

        static ApiMethodContext()
        {
            EmptyContext = new ApiMethodContext();
        }

        private string _cacheKey;

        /// <summary>
        /// 获取或设置本次API方法调用所关联的<see cref="ApiMethodContext"/>。
        /// </summary>
        public static ApiMethodContext Current
        {
            get
            {
                return _currentContext ?? EmptyContext;
            }
            set
            {
                _currentContext = value;
            }
        }

        /// <summary>
        /// 获取当前被调用方法所关联的缓存值。
        /// 若没有被缓存的值，返回<c>null</c>。
        /// </summary>
        public object GetCachedResult()
        {
            if (CacheProvider == null || CacheKeyProvider == null)
                return null;

            var cacheKey = GetCacheKey();
            return CacheProvider.Get(cacheKey);
        }

        /// <summary>
        /// 设置当前被调用方法所关联的缓存值。
        /// </summary>
        /// <param name="obj">需缓存的值。</param>
        public void SetCachedResult(object obj)
        {
            if (CacheProvider == null || CacheKeyProvider == null)
                return;

            var cacheKey = GetCacheKey();
            CacheProvider.Set(cacheKey, obj, CacheExpiration);
        }

        internal Func<string> CacheKeyProvider { get; set; }

        internal TimeSpan CacheExpiration { get; set; }

        internal IApiCacheProvider CacheProvider { get; set; }

        private string GetCacheKey()
        {
            return _cacheKey ?? (_cacheKey = CacheKeyProvider());
        }
    }
}
