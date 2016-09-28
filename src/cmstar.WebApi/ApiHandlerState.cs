using System;
using System.Collections.Generic;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含API方法的有关信息。
    /// </summary>
    public class ApiHandlerState
    {
        private const string Delimiter = "$`l%";

        private readonly Dictionary<string, ApiMethodInfo> _methods
            = new Dictionary<string, ApiMethodInfo>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, IRequestDecoder> _decoders
            = new Dictionary<string, IRequestDecoder>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 在每个API方法执行前触发此事件。
        /// </summary>
        public event Action<ApiMethodInfo, IDictionary<string, object>> MethodInvoking;

        /// <summary>
        /// 在每个API方法执行后触发此事件。
        /// </summary>
        public event Action<ApiMethodInfo, IDictionary<string, object>, object, Exception> MethodInvoked;

        /// <summary>
        /// 当前API方法的相关信息是否已经初始化。
        /// </summary>
        public bool Initialized { get; set; }

        /// <summary>
        /// 获取或设置日志想的配置信息。
        /// </summary>
        public LogSetup LogSetup { get; set; }

        /// <summary>
        /// 添加一个API方法注册信息。
        /// </summary>
        /// <param name="methodName">API方法的名称。</param>
        /// <param name="apiMethodInfo">API方法的注册信息。</param>
        public void AddMethod(string methodName, ApiMethodInfo apiMethodInfo)
        {
            _methods[methodName] = apiMethodInfo;
        }

        /// <summary>
        /// 添加一个API方法所关联的参数解析器。
        /// </summary>
        /// <param name="methodName">API方法的名称。</param>
        /// <param name="decoderName">参数解析器的名称。</param>
        /// <param name="decoder">参数解析器的实例。</param>
        public void AddDecoder(string methodName, string decoderName, IRequestDecoder decoder)
        {
            var key = string.Concat(methodName, Delimiter, decoderName);
            _decoders[key] = decoder;
        }

        /// <summary>
        /// 获取具有指定名称的API方法的注册信息。若指定的名称不存在，返回null。
        /// </summary>
        /// <param name="methodName">API方法的名称。</param>
        /// <returns>API方法的注册信息。若指定的名称不存在，返回null。</returns>
        public ApiMethodInfo GetMethod(string methodName)
        {
            if (methodName == null)
                return null;

            ApiMethodInfo method;
            return _methods.TryGetValue(methodName, out method) ? method : null;
        }

        /// <summary>
        /// 获取指定API方法所关联的具有指定名称的参数解析器。若相关名称不存在，返回null。
        /// </summary>
        /// <param name="methodName">API方法的名称。</param>
        /// <param name="decoderName">参数解析器的名称。</param>
        /// <returns>参数解析器的实例。若相关名称不存在，返回null。</returns>
        public IRequestDecoder GetDecoder(string methodName, string decoderName)
        {
            var key = string.Concat(methodName, Delimiter, decoderName);

            IRequestDecoder decoder;
            return _decoders.TryGetValue(key, out decoder) ? decoder : null;
        }

        /// <summary>
        /// 触发<see cref="MethodInvoking"/>事件。
        /// </summary>
        /// <param name="apieMethodInfo">调用的API方法的信息。</param>
        /// <param name="param">调用的API方法的输入参数。</param>
        public void OnMethodInvoking(ApiMethodInfo apieMethodInfo, IDictionary<string, object> param)
        {
            if (MethodInvoking != null)
            {
                MethodInvoking(apieMethodInfo, param);
            }
        }

        /// <summary>
        /// 触发<see cref="MethodInvoked"/>事件。
        /// </summary>
        /// <param name="apieMethodInfo">调用的API方法的信息。</param>
        /// <param name="param">调用的API方法的输入参数。</param>
        /// <param name="result">调用的方法的返回值。若方法没有返回值，或调用过程中出现异常，为null。</param>
        /// <param name="ex">调用的方法所抛出的异常。若无异常，为null。</param>
        public void OnMethodInvoked(
            ApiMethodInfo apieMethodInfo, IDictionary<string, object> param, object result, Exception ex)
        {
            if (MethodInvoked != null)
            {
                MethodInvoked(apieMethodInfo, param, result, ex);
            }
        }
    }
}