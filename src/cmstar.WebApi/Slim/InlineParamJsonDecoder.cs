using System.Collections.Generic;
using System.IO;
using cmstar.Serialization.Json;

#if NETCORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// <see cref="IRequestDecoder"/>的实现。
    /// 将HTTP请求的body中的JSON的各属性以一一对应的关系映射到注册的.net方法参数上。
    /// </summary>
    /// <remarks>
    /// 若请求中的JSON为 { a: 123, b: "abc", c: 1.1}
    /// 对应注册的.net方法为 M(int a, string b, float c)，
    /// 则JSON中的a、b、c属性将分别映射到.net方法的a、b、c参数。
    /// </remarks>
    public class InlineParamJsonDecoder : IRequestDecoder
    {
        private readonly MethodParamContract _contract;

        /// <summary>
        /// 初始化<see cref="InlineParamJsonDecoder"/>的新实例。
        /// </summary>
        /// <param name="paramInfoMap">注册WebAPI的方法的参数的有关信息。</param>
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
                    var paramType = kv.Value.Type;

                    var contract = contractResolver.ResolveContract(paramType);
                    contractMap.Add(paramName, contract);
                }

                _contract = new MethodParamContract(contractMap);
            }
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
            if (_contract == null)
                return new Dictionary<string, object>(0);

            TextReader textReader;
            if (state is IJsonRequestState jsonState)
            {
                textReader = new StringReader(jsonState.RequestJson);
            }
            else
            {
                // 不调用StreamReader.Dispose以保持InputStream不被关掉
                textReader = new StreamReader(request.RequestInputStream());
            }

            var jsonReader = new JsonReader(textReader);
            var paramValueMap = _contract.Read(jsonReader, new JsonDeserializingState());

            return (Dictionary<string, object>)paramValueMap;
        }
    }
}
