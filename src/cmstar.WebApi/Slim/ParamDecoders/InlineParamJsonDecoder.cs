using System.Collections.Generic;
using System.IO;
using System.Web;
using cmstar.Serialization.Json;

namespace cmstar.WebApi.Slim.ParamDecoders
{
    public class InlineParamJsonDecoder : IRequestDecoder
    {
        private readonly MethodParamContract _contract;

        public InlineParamJsonDecoder(ApiMethodParamInfoMap paramInfoMap)
        {
            ArgAssert.NotNull(paramInfoMap, "paramInfoMap");

            if (paramInfoMap.ParamCount > 0)
            {
                var contractResolver = JsonHelper.GetSerializer().ContractResolver;
                var contractMap = new Dictionary<string, JsonContract>(paramInfoMap.ParamCount);

                foreach (var kv in paramInfoMap.ParamInfos)
                {
                    var paramName = kv.Key;
                    var paramType = kv.Value;

                    var contract = contractResolver.ResolveContract(paramType);
                    contractMap.Add(paramName, contract);
                }

                _contract = new MethodParamContract(contractMap);
            }
        }

        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            if (_contract == null)
                return new Dictionary<string, object>(0);

            var textReader = new StreamReader(request.InputStream);
            var jsonReader = new JsonReader(textReader);
            var paramValueMap = _contract.Read(jsonReader, new JsonDeserializingState());

            return (Dictionary<string, object>)paramValueMap;
        }
    }
}
