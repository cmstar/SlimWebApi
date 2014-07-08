using System;
using System.Collections.Generic;
using System.Reflection;
using cmstar.RapidReflection.Emit;

namespace cmstar.WebApi
{
    public class ApiMethodInfo
    {
        private readonly bool _isStaticMethod;
        private readonly Func<object> _provider;
        private readonly Func<object, object[], object> _invoker;
        private string _methodName;
        private TimeSpan _cacheExpiration = TimeSpan.Zero;

        public ApiMethodInfo(Func<object> provider, MethodInfo methodInfo)
        {
            ArgAssert.NotNull(methodInfo, "methodInfo");

            _isStaticMethod = methodInfo.IsStatic;
            if (!_isStaticMethod)
            {
                ArgAssert.NotNull(provider, "provider");
            }

            ParamInfoMap = new ApiMethodParamInfoMap(methodInfo);
            _provider = provider;
            _methodName = methodInfo.Name;
            _invoker = MethodInvokerGenerator.CreateDelegate(methodInfo);
        }

        public ApiMethodParamInfoMap ParamInfoMap { get; private set; }

        public MethodInfo Method
        {
            get { return ParamInfoMap.Method; }
        }

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

        public TimeSpan CacheExpiration
        {
            get { return _cacheExpiration; }
            set
            {
                if (value.Ticks <= 0)
                    throw new ArgumentException("The expiration must be greater than zero.", "value");

                _cacheExpiration = value;
            }
        }

        public IApiCacheProvider CacheProvider { get; set; }

        public string CacheNamespace { get; set; }

        public object Invoke(IDictionary<string, object> paramValueMap)
        {
            var paramArray = BuildParamArray(paramValueMap);
            return Invoke(paramArray);
        }

        public object Invoke(object[] param)
        {
            ArgAssert.NotNull(param, "param");

            object result;
            if (_isStaticMethod)
            {
                result = _invoker(null, param);
            }
            else
            {
                var providerInstance = _provider == null ? null : _provider();
                result = _invoker(providerInstance, param);
            }
            return result;
        }

        public object[] BuildParamArray(IDictionary<string, object> paramValueMap)
        {
            var paramValues = new object[ParamInfoMap.ParamCount];

            foreach (var kv in ParamInfoMap.ParamInfos)
            {
                var index = kv.Value.Index;

                object value;
                if (paramValueMap != null && paramValueMap.TryGetValue(kv.Key, out value))
                {
                    paramValues[index] = value;
                    continue;
                }

                // 没有给定值的情况，设定默认值
                // 对于引用类型，保持初始化的null
                var valueType = kv.Value.Type;
                if (!valueType.IsValueType)
                    continue;

                // 对于值类型，需要初始化其值
                value = Activator.CreateInstance(valueType);
                paramValues[index] = value;
            }

            return paramValues;
        }
    }
}