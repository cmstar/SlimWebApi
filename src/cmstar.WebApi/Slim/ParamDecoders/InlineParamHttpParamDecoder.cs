using System;
using System.Collections.Generic;
using System.Web;

namespace cmstar.WebApi.Slim.ParamDecoders
{
    public class InlineParamHttpParamDecoder : IRequestDecoder
    {
        private readonly ApiMethodParamInfoMap _paramInfoMap;

        public InlineParamHttpParamDecoder(ApiMethodParamInfoMap paramInfoMap)
        {
            ArgAssert.NotNull(paramInfoMap, "paramInfoMap");

            foreach (var kv in paramInfoMap.ParamInfos)
            {
                var paramInfo = kv.Value;
                if (paramInfo.Type.IsSubclassOf(typeof(IConvertible)))
                {
                    var msg = string.Format(
                        "The parameter \"{0}\" (type {1}) of method {2} cannot be convert from the query string.",
                        paramInfo.Name, paramInfo.Type, paramInfoMap.Method.Name);
                    throw new ArgumentException(msg, "paramInfoMap");
                }
            }

            _paramInfoMap = paramInfoMap;
        }

        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            if (_paramInfoMap.ParamCount == 0)
                return new Dictionary<string, object>(0);

            var keys = request.Params.AllKeys;
            var paramValueMap = new Dictionary<string, object>(keys.Length);

            foreach (var key in keys)
            {
                if (key == null)
                    continue;

                ApiParamInfo paramInfo;
                if (!_paramInfoMap.TryGetParamInfo(key, out paramInfo))
                    continue;

                var value = TypeHelper.ConvertString(request.Params[key], paramInfo.Type);
                var name = paramInfo.Name;

                paramValueMap.Add(name, value);
            }

            return paramValueMap;
        }
    }
}
