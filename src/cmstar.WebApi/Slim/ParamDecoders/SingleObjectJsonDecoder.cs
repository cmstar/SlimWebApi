using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace cmstar.WebApi.Slim.ParamDecoders
{
    public class SingleObjectJsonDecoder : IRequestDecoder
    {
        private readonly string _paramName;
        private readonly Type _paramType;

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
            var value = SlimApiEnvironment.JsonSerializer.Deserialize(textReader, _paramType);
            
            return new Dictionary<string, object>(1) { { _paramName, value } };
        }
    }
}
