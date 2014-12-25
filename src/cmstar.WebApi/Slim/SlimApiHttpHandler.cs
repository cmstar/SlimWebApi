using System.Collections.Generic;
using System.Web;
using cmstar.WebApi.Slim.ParamDecoders;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 提供Slim WebAPI的入口。继承此类以实现API的注册和使用。
    /// 这是一个抽象类。
    /// </summary>
    public abstract class SlimApiHttpHandler : ApiHttpHandlerBase
    {
        /// <summary>
        /// 获取用于当前API调用的<see cref="IApiInvocationHandler"/>实现。
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/>。</param>
        /// <param name="handlerState">包含API方法注册相关的信息。</param>
        /// <returns><see cref="IApiInvocationHandler"/>实现。</returns>
        protected override IApiInvocationHandler CreateInvocationHandler(HttpContext context, ApiHandlerState handlerState)
        {
            return new SlimApiInvocationHandler(context, handlerState.Logger);
        }

        /// <summary>
        /// 获取指定的API方法所对应的参数解析器。
        /// </summary>
        /// <param name="method">包含API方法的有关信息。</param>
        /// <returns>key为解析器的名称，value为解析器实例。</returns>
        protected override IDictionary<string, IRequestDecoder> ResolveDecoders(ApiMethodInfo method)
        {
            var param = method.Method.GetParameters();
            var decoderMap = new Dictionary<string, IRequestDecoder>(4);

            if (param.Length == 0)
            {
                decoderMap[SlimApiEnvironment.MetaRequestFormatGet]
                    = decoderMap[SlimApiEnvironment.MetaRequestFormatPost]
                    = decoderMap[SlimApiEnvironment.MetaRequestFormatJson]
                    = decoderMap[string.Empty]
                    = EmptyParamMethodRequestDecoder.Instance;

                return decoderMap;
            }

            var paramInfoMap = method.ParamInfoMap;

            if (TypeHelper.IsPlainMethod(method.Method, true))
            {
                decoderMap[SlimApiEnvironment.MetaRequestFormatGet]
                    = decoderMap[SlimApiEnvironment.MetaRequestFormatPost]
                    = new InlineParamHttpParamDecoder(paramInfoMap);

                decoderMap[SlimApiEnvironment.MetaRequestFormatJson]
                    = new InlineParamJsonDecoder(paramInfoMap);
            }
            else if (param.Length == 1)
            {
                decoderMap[SlimApiEnvironment.MetaRequestFormatJson]
                    = new SingleObjectJsonDecoder(paramInfoMap);

                var paramType = param[0].ParameterType;
                if (TypeHelper.IsPlainType(paramType, true))
                {
                    decoderMap[SlimApiEnvironment.MetaRequestFormatGet]
                        = decoderMap[SlimApiEnvironment.MetaRequestFormatPost]
                        = new SingleObjectHttpParamDecoder(paramInfoMap);
                }
            }
            else
            {
                decoderMap[SlimApiEnvironment.MetaRequestFormatJson]
                    = new InlineParamJsonDecoder(paramInfoMap);
            }

            // 优先使用HttpParamDecoder作为默认的Decoder
            decoderMap[string.Empty] =
                decoderMap.ContainsKey(SlimApiEnvironment.MetaRequestFormatGet)
                ? decoderMap[SlimApiEnvironment.MetaRequestFormatGet]
                : decoderMap[SlimApiEnvironment.MetaRequestFormatJson];

            return decoderMap;
        }
    }
}
