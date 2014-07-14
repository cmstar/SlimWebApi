using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace cmstar.WebApi.Slim.ParamDecoders
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
                var msg = string.Format("The method {0} should not have more than one parameter.", paramInfoMap.Method);
                throw new ArgumentException(msg, "paramInfoMap");
            }

            var paramInfo = paramInfoMap.ParamInfos.First().Value;
            _paramName = paramInfo.Name;
            _paramType = paramInfo.Type;
        }

        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            if (_paramName == null)
                return new Dictionary<string, object>(0);

            var textReader = new StreamReader(request.InputStream);
            var value = JsonHelper.Deserialize(textReader, _paramType);

            return new Dictionary<string, object>(1) { { _paramName, value } };
        }
    }
}
