using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if NETCORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// <see cref="IRequestDecoder"/>的实现。
    /// 解析HTTP请求body中的JSON，并将该JSON映射到只有一个参数的方法的唯一参数。
    /// </summary>
    public class SingleObjectJsonDecoder : IRequestDecoder
    {
        private readonly string _paramName;
        private readonly Type _paramType;

        /// <summary>
        /// 初始化<see cref="SingleObjectJsonDecoder"/>的新实例。
        /// </summary>
        /// <param name="paramInfoMap">包含方法参数相关的信息。</param>
        public SingleObjectJsonDecoder(ApiMethodParamInfoMap paramInfoMap)
        {
            ArgAssert.NotNull(paramInfoMap, "paramInfoMap");

            var paramCount = paramInfoMap.ParamCount;
            if (paramCount == 0)
                return;

            if (paramCount != 1)
            {
                var msg = $"The method {paramInfoMap.Method} should not have more than one parameter.";
                throw new ArgumentException(msg, nameof(paramInfoMap));
            }

            var paramInfo = paramInfoMap.ParamInfos.First().Value;
            _paramName = paramInfo.Name;
            _paramType = paramInfo.Type;
        }

        /// <summary>
        /// 解析<see cref="HttpRequest"/>并创建该请求所对应要调用方法的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="request">HTTP请求。</param>
        /// <param name="state">
        /// 若为<see cref="IJsonRequestState"/>，则JSON将从<see cref="IJsonRequestState.RequestJson"/>读取；
        /// 否则从请求的BODY中获取。
        /// </param>
        /// <returns>记录参数名称和对应的值。</returns>
        public IDictionary<string, object> DecodeParam(HttpRequest request, object state)
        {
            if (_paramName == null)
                return new Dictionary<string, object>(0);

            object value;
            if (state is IJsonRequestState jsonState)
            {
                value = JsonHelper.Deserialize(jsonState.RequestJson, _paramType);
            }
            else
            {
                var body = request.RequestInputStream();

                // BODY可能被重读，设置 leaveOpen=true 以保持其不被关掉。
                using (var textReader = new StreamReader(body, SlimApiHttpHandler.DefaultEncoding, false, 1024, true))
                {
                    value = JsonHelper.Deserialize(textReader, _paramType, true);
                }
            }

            return new Dictionary<string, object>(1) { { _paramName, value } };
        }
    }
}
