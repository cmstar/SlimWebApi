using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using cmstar.RapidReflection.Emit;
using cmstar.WebApi.Slim;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含获取方法缓存键相关的方法。
    /// </summary>
    internal static class CacheKeyHelper
    {
        private static readonly Dictionary<ApiMethodInfo, ICacheKeyBuilder> KnownCacheKeyBuilders
            = new Dictionary<ApiMethodInfo, ICacheKeyBuilder>();

        /// <summary>
        /// 获取指定WebAPI方法所使用的缓存键的前缀。
        /// 此前缀可用于查找、清除特定方法的缓存。
        /// </summary>
        /// <param name="apiMethodInfo">WebAPI方法的注册信息。</param>
        /// <returns>缓存键的前缀。</returns>
        public static string GetCacheKeyPrefix(ApiMethodInfo apiMethodInfo)
        {
            return apiMethodInfo.Setting.CacheNamespace + apiMethodInfo.Setting.MethodName;
        }

        /// <summary>
        /// 获取缓存的键。
        /// </summary>
        /// <param name="apiMethodInfo">WebAPI方法的注册信息。</param>
        /// <param name="paramValueMap">方法的参数字典，包含各参数的名称及值。</param>
        /// <returns>缓存的键。</returns>
        public static string GetCacheKey(ApiMethodInfo apiMethodInfo, IDictionary<string, object> paramValueMap)
        {
            var cacheKeyBuilder = GetCacheKeyBuilder(apiMethodInfo);
            var cacheKey = cacheKeyBuilder.BuildCacheKey(apiMethodInfo, paramValueMap);
            return cacheKey;
        }

        private static ICacheKeyBuilder GetCacheKeyBuilder(ApiMethodInfo apiMethodInfo)
        {
            ICacheKeyBuilder cacheKeyBuilder;
            if (KnownCacheKeyBuilders.TryGetValue(apiMethodInfo, out cacheKeyBuilder))
                return cacheKeyBuilder;

            lock (KnownCacheKeyBuilders)
            {
                if (KnownCacheKeyBuilders.TryGetValue(apiMethodInfo, out cacheKeyBuilder))
                    return cacheKeyBuilder;

                cacheKeyBuilder = ResolveCacheKeyBuilder(apiMethodInfo);
                KnownCacheKeyBuilders.Add(apiMethodInfo, cacheKeyBuilder);
            }

            return cacheKeyBuilder;
        }

        private static ICacheKeyBuilder ResolveCacheKeyBuilder(ApiMethodInfo apiMethodInfo)
        {
            var method = apiMethodInfo.Method;
            var methodParamStat = TypeHelper.GetMethodParamStat(method);

            if (methodParamStat.HasFileInput)
                return NotSupportedCacheKeyBuilder.Instance;

            if (methodParamStat.IsPurePlain)
                return new PlainMethodCacheKeyBuilder();

            var ps = method.GetParameters();
            if (ps.Length == 1)
            {
                var paramType = ps[0].ParameterType;
                var paramTypeStat = TypeHelper.GetTypeMemberStat(paramType);

                if (paramTypeStat.HasFileInput)
                    return NotSupportedCacheKeyBuilder.Instance;

                if (paramTypeStat.IsPurePlain)
                    return new SinglePlainTypeCacheKeyBuilder(paramType);
            }

            return new ComplexTypeCacheKeyBuilder();
        }

        private interface ICacheKeyBuilder
        {
            string BuildCacheKey(ApiMethodInfo apiMethodInfo, IDictionary<string, object> paramValueMap);
        }

        private class NotSupportedCacheKeyBuilder : ICacheKeyBuilder
        {
            public static readonly NotSupportedCacheKeyBuilder Instance = new NotSupportedCacheKeyBuilder();

            private NotSupportedCacheKeyBuilder() { }

            public string BuildCacheKey(ApiMethodInfo apiMethodInfo, IDictionary<string, object> paramValueMap)
            {
                return null;
            }
        }

        private class PlainMethodCacheKeyBuilder : ICacheKeyBuilder
        {
            public string BuildCacheKey(ApiMethodInfo apiMethodInfo, IDictionary<string, object> paramValueMap)
            {
                var paramArray = apiMethodInfo.BuildParamArray(paramValueMap);

                var prefix = GetCacheKeyPrefix(apiMethodInfo);
                var sb = new StringBuilder(prefix, 40);

                foreach (var v in paramArray)
                {
                    sb.Append('_').Append(v);
                }

                return sb.ToString();
            }
        }

        private class SinglePlainTypeCacheKeyBuilder : ICacheKeyBuilder
        {
            private readonly List<Func<object, object>> _getters;

            public SinglePlainTypeCacheKeyBuilder(Type type)
            {
                _getters = new List<Func<object, object>>();

                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var prop in props)
                {
                    var getter = PropertyAccessorGenerator.CreateGetter(prop);
                    _getters.Add(getter);
                }

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    var getter = FieldAccessorGenerator.CreateGetter(field);
                    _getters.Add(getter);
                }
            }

            public string BuildCacheKey(ApiMethodInfo apiMethodInfo, IDictionary<string, object> paramValueMap)
            {
                var paramArray = apiMethodInfo.BuildParamArray(paramValueMap);
                var param = paramArray[0];

                var prefix = GetCacheKeyPrefix(apiMethodInfo);
                var sb = new StringBuilder(prefix, 40);

                if (param != null)
                {
                    foreach (var getter in _getters)
                    {
                        var v = getter(param);
                        sb.Append('_').Append(v);
                    }
                }

                return sb.ToString();
            }
        }

        // 复杂的类型现在直接用json
        private class ComplexTypeCacheKeyBuilder : ICacheKeyBuilder
        {
            public string BuildCacheKey(ApiMethodInfo apiMethodInfo, IDictionary<string, object> paramValueMap)
            {
                var paramArray = apiMethodInfo.BuildParamArray(paramValueMap);

                var prefix = GetCacheKeyPrefix(apiMethodInfo);
                var sb = new StringBuilder(prefix, 80);

                foreach (var v in paramArray)
                {
                    var json = JsonHelper.Serialize(v);
                    sb.Append('_').Append(json);
                }

                return sb.ToString();
            }
        }
    }
}
