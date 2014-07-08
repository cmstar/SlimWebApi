using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var paramTypeMap = paramInfoMap.ParamInfos
                    .Select(x => x.Value)
                    .ToDictionary(x => x.Name, x => x.Type);
                _contract = new MethodParamContract(paramTypeMap, SlimApiEnvironment.JsonSerializer.ContractResolver);
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
