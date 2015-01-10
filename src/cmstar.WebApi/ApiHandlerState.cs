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
    }
}