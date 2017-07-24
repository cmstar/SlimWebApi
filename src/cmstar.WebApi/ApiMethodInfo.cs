using System;
using System.Collections.Generic;
using System.Reflection;
using cmstar.RapidReflection.Emit;
#if NET35
using cmstar.WebApi.NetFuture;
#endif

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含WebAPI方法的注册信息和有关操作。
    /// </summary>
    public class ApiMethodInfo
    {
        private readonly bool _isStaticMethod;
        private readonly Func<object> _provider;
        private readonly Func<object, object[], object> _invoker;
        private readonly ApiMethodSetting _setting;

        /// <summary>
        /// 初始化<see cref="ApiMethodInfo"/>的新实例。
        /// </summary>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodInfo">被注册为WebAPI的方法。</param>
        /// <param name="methodSetting">用于获取或保存此WebAPI方法的设置信息。若为null，则套用默认的设置。</param>
        /// <exception cref="ArgumentNullException">当<paramref name="methodInfo"/>为null。</exception>
        public ApiMethodInfo(Func<object> provider, MethodInfo methodInfo, ApiMethodSetting methodSetting)
        {
            ArgAssert.NotNull(methodInfo, "methodInfo");

            _isStaticMethod = methodInfo.IsStatic;
            if (!_isStaticMethod)
            {
                ArgAssert.NotNull(provider, "provider");
                _provider = provider;
            }

            ParamInfoMap = new ApiMethodParamInfoMap(methodInfo);

            _invoker = MethodInvokerGenerator.CreateDelegate(methodInfo);
            _setting = methodSetting ?? new ApiMethodSetting();

            // 若没有指定方法入口的名称，套用方法自身的名称
#if NET35
            if (!_setting.MethodName.IsNullOrWhiteSpace())
#else
            if (!string.IsNullOrWhiteSpace(_setting.MethodName))
#endif
            {
                _setting.MethodName = methodInfo.Name;
            }
        }

        /// <summary>
        /// 获取WebAPI方法的参数信息。
        /// </summary>
        public ApiMethodParamInfoMap ParamInfoMap { get; private set; }

        /// <summary>
        /// 获取当前注册中的关联的<see cref="MethodInfo"/>。
        /// </summary>
        public MethodInfo Method
        {
            get { return ParamInfoMap.Method; }
        }

        /// <summary>
        /// 获取当前实例所关联的Web API方法的设置信息。
        /// </summary>
        public ApiMethodSetting Setting
        {
            get { return _setting; }
        }

        /// <summary>
        /// 执行一次WebAPI中绑定的方法。
        /// </summary>
        /// <param name="paramValueMap">
        /// 方法的参数字典，包含各参数的名称及值。
        /// 若有参数未在字典中给出，该参数将保持默认值（对于引用类型为null，值类型为0）。
        /// 若字典本身为null，则所有的参数都保持默认值。
        /// </param>
        /// <returns>方法的返回值。</returns>
        public object Invoke(IDictionary<string, object> paramValueMap)
        {
            var paramArray = BuildParamArray(paramValueMap);
            return Invoke(paramArray);
        }

        /// <summary>
        /// 执行一次WebAPI中绑定的方法。
        /// </summary>
        /// <param name="param">与方法参数签名顺序和长度一致的参数数组。</param>
        /// <returns>方法的返回值。</returns>
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

        /// <summary>
        /// 获得与方法参数签名顺序和长度一致的参数数组。该数据可用于直接调用方法。
        /// </summary>
        /// <param name="paramValueMap">
        /// 方法的参数字典，包含各参数的名称及值。
        /// 若有参数未在字典中给出，该参数将保持默认值（对于引用类型为null，值类型为0）。
        /// 若字典本身为null，则所有的参数都保持默认值。
        /// </param>
        /// <returns>与方法参数签名顺序和长度一致的参数数组。</returns>
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
                // 虽然这里使用emit创建会更快，但还需要做一层用于创建对象的委托的缓存，
                // 处理缓存也需要时间和空间开销，整体并不优化，这里就直接使用Activator
                value = Activator.CreateInstance(valueType);
                paramValues[index] = value;
            }

            return paramValues;
        }
    }
}