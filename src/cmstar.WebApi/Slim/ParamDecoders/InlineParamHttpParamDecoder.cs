using System;
using System.Collections.Generic;
using System.Web;

namespace cmstar.WebApi.Slim.ParamDecoders
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

        /// <summary>
        /// 解析<see cref="HttpRequest"/>并创建该请求所对应要调用方法的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="request">HTTP请求。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            if (_paramInfoMap.ParamCount == 0)
                return new Dictionary<string, object>(0);

            var keys = request.ExplicicParamKeys();
            var paramValueMap = new Dictionary<string, object>();

            foreach (var key in keys)
            {
                if (key == null)
                    continue;

                ApiParamInfo paramInfo;
                if (!_paramInfoMap.TryGetParamInfo(key, out paramInfo))
                    continue;

                var paramValue = request.ExplicicParam(key);
                var value = paramInfo.IsGenericCollection
                    ? TypeHelper.ConvertToCollection(paramValue, paramInfo.Type)
                    : TypeHelper.ConvertString(paramValue, paramInfo.Type);

                paramValueMap.Add(paramInfo.Name, value);
            }

            return paramValueMap;
        }
    }
}
