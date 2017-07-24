using System;
using System.Collections.Generic;
using System.Reflection;
using cmstar.RapidReflection.Emit;
using cmstar.Util;

#if NET35
using cmstar.WebApi.NetFuture;
#else
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
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
            if (StringUtils.IsNullOrWhiteSpace(_setting.MethodName))
#else
            if (string.IsNullOrWhiteSpace(_setting.MethodName))
#endif
            {
                _setting.MethodName = methodInfo.Name;
            }

#if !NET35
            // 只要返回Task就认为是异步方法
            IsAsyncMethod = ReflectionUtils.IsOrIsSubClassOf(methodInfo.ReturnType, typeof(Task));
            if (IsAsyncMethod)
            {
                _taskResultGetter = BuildTaskResultGetter(methodInfo.ReturnType);
            }
#endif
        }

        /// <summary>
        /// 获取WebAPI方法的参数信息。
        /// </summary>
        public ApiMethodParamInfoMap ParamInfoMap { get; }

        /// <summary>
        /// 获取当前注册中的关联的<see cref="MethodInfo"/>。
        /// </summary>
        public MethodInfo Method => ParamInfoMap.Method;

        /// <summary>
        /// 获取当前实例所关联的Web API方法的设置信息。
        /// </summary>
        public ApiMethodSetting Setting => _setting;

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
            ArgAssert.NotNull(param, nameof(param));

            object result;
            if (_isStaticMethod)
            {
                result = _invoker(null, param);
            }
            else
            {
                var providerInstance = _provider?.Invoke();
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

#if !NET35
        /// <summary>
        /// true若方法为异步方法（返回<see cref="Task"/>或<see cref="Task{T}"/>）；否则返回false。
        /// </summary>
        public bool IsAsyncMethod { get; }

        /// <summary>
        /// 用于从异步的方法的返回值<see cref="Task{T}"/>中，将<see cref="Task{T}.Result"/>的值取出来。
        /// 如果异步方法仅返回非泛型的Task，则不需要取值，此时该字段值为null。
        /// </summary>
        private readonly Func<Task, object> _taskResultGetter;

        /// <summary>
        /// 执行一次WebAPI中绑定的异步方法（返回Task）。
        /// <see cref="IsAsyncMethod"/>必须为true才可以使用。
        /// </summary>
        /// <param name="paramValueMap">
        /// 方法的参数字典，包含各参数的名称及值。
        /// 若有参数未在字典中给出，该参数将保持默认值（对于引用类型为null，值类型为0）。
        /// 若字典本身为null，则所有的参数都保持默认值。
        /// </param>
        /// <returns>方法的返回值。</returns>
        /// <exception cref="InvalidOperationException">当<see cref="IsAsyncMethod"/>为false。</exception>
        public async Task<object> InvokeAsync(IDictionary<string, object> paramValueMap)
        {
            // 仅支持异步方法
            if (!IsAsyncMethod)
                throw new InvalidOperationException("The method is not an async method.");

            // 若方法不返回Task，就不是异步方法，直接返回结果
            var taskResult = (Task)Invoke(paramValueMap);

            // 在异步方法返回的Task上执行await以达到异步目的
            await taskResult;

            // 若异步方法返回Task<T>，则获取其Result，否则返回null
            return _taskResultGetter?.Invoke(taskResult);
        }

        private Func<Task, object> BuildTaskResultGetter(Type taskType)
        {
            Contract.Assert(ReflectionUtils.IsOrIsSubClassOf(taskType, typeof(Task)));

            // 对于没有返回值的Task，不需要创建getter方法
            if (!taskType.IsGenericType)
                return null;

            // 嗯……利用 nameof “安全”地拿到 Result 属性
            Task<object> t;
            var resultProp = taskType.GetProperty(nameof(t.Result));
            var getter = PropertyAccessorGenerator.CreateGetter(resultProp);
            return getter;
        }
#endif
    }
}