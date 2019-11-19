using System;
using System.Collections.Generic;
using System.IO;

#if NETCORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// <see cref="IRequestDecoder"/>的实现。
    /// 将请求的HTTP参数以一一对应的关系映射到注册的.net方法参数上。
    /// </summary>
    /// <remarks>
    /// 若请求的HTTP参数为 a=123&amp;b=abc&amp;c=1.1
    /// 对应注册的.net方法为 M(int a, string b, float c)，
    /// 则HTTP参数中的a、b、c将分别映射到.net方法的a、b、c参数。
    /// </remarks>
    public class InlineParamHttpParamDecoder : IRequestDecoder
    {
        private readonly ApiMethodParamInfoMap _paramInfoMap;
        private readonly string _streamParamName;
        private readonly string _httpFileCollectionParamName;

        /// <summary>
        /// 初始化<see cref="InlineParamHttpParamDecoder"/>的新实例。
        /// </summary>
        /// <param name="paramInfoMap">注册WebAPI的方法的参数的有关信息。</param>
        public InlineParamHttpParamDecoder(ApiMethodParamInfoMap paramInfoMap)
        {
            ArgAssert.NotNull(paramInfoMap, "paramInfoMap");

            foreach (var kv in paramInfoMap.ParamInfos)
            {
                var paramInfo = kv.Value;
                var paramType = paramInfo.Type;

                if (paramType == typeof(Stream))
                {
                    if (_streamParamName != null)
                    {
                        throw FileParamConflictError(paramInfoMap.Method.Name, "paramInfoMap");
                    }

                    _streamParamName = paramInfo.Name;
                    continue;
                }

#if NETCORE
                if (paramType == typeof(IFormFileCollection))
#else
                if (paramType == typeof(HttpFileCollection))
#endif
                {
                    if (_streamParamName != null || _httpFileCollectionParamName != null)
                    {
                        throw FileParamConflictError(paramInfoMap.Method.Name, "paramInfoMap");
                    }

                    _httpFileCollectionParamName = paramInfo.Name;
                    continue;
                }

                if (!TypeHelper.CanConvertFromString(paramType))
                {
                    var msg = string.Format(
                        "The values for parameter '{0}' ({1}) of method '{2}' cannot be converted from the query string.",
                        paramInfo.Name, paramType, paramInfoMap.Method.Name);
                    throw new ArgumentException(msg, nameof(paramInfoMap));
                }
            }

            _paramInfoMap = paramInfoMap;
        }

        /// <summary>
        /// 解析<see cref="HttpRequest"/>并创建该请求所对应要调用方法的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="request">HTTP请求。</param>
        /// <param name="state">包含用于参数解析的有关数据。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        public IDictionary<string, object> DecodeParam(HttpRequest request, object state)
        {
            if (_paramInfoMap.ParamCount == 0)
                return new Dictionary<string, object>(0);

            var paramValueMap = new Dictionary<string, object>();
            if (_streamParamName != null)
            {
                var inputStream = request.RequestInputStream();
                paramValueMap.Add(_streamParamName, inputStream);
            }

            if (_httpFileCollectionParamName != null)
            {
#if NETCORE
                paramValueMap.Add(_httpFileCollectionParamName, request.FormFiles());
#else
                paramValueMap.Add(_httpFileCollectionParamName, request.Files);
#endif
            }

            var requestParam = HttpParamDecoderHelper.AllParam(request, state);
            foreach (var p in requestParam)
            {
                var key = p.Key;
                if (key == null || key == _streamParamName || key == _httpFileCollectionParamName)
                    continue;

                ApiParamInfo paramInfo;
                if (!_paramInfoMap.TryGetParamInfo(key, out paramInfo))
                    continue;

                var paramValue = p.Value;
                object value;
                try
                {
                    value = paramInfo.IsGenericCollection
                        ? TypeHelper.ConvertToCollection(paramValue, paramInfo.Type)
                        : TypeHelper.ConvertString(paramValue, paramInfo.Type);
                }
                catch (Exception ex)
                {
                    var msg = $"Parameter '{key}' - failed on converting value '{paramValue}' to type {paramInfo.Type}.";
                    throw new InvalidCastException(msg, ex);
                }

                paramValueMap.Add(paramInfo.Name, value);
            }

            return paramValueMap;
        }

        private Exception FileParamConflictError(string methodName, string parameterName)
        {
            var msg = $"There can be only one parameter declared as Stream or HttpFileCollection on the method '{methodName}'.";
            return new ArgumentException(msg, parameterName);
        }
    }
}
