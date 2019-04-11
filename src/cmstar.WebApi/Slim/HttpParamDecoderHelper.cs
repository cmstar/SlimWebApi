using System.Collections.Generic;
using System.Text;
using System.Web;

namespace cmstar.WebApi.Slim
{
    internal static class HttpParamDecoderHelper
    {
        /// <summary>
        /// 获取所有请求参数。
        /// </summary>
        public static Dictionary<string, string> AllParam(HttpRequest request, object state)
        {
            var result = new Dictionary<string, string>();
            var keys = request.ExplicicParamKeys();

            foreach (var key in keys)
            {
                if (key != null)
                {
                    result[key] = request.ExplicicParam(key);
                }
            }

            TryAppendFormData(result, request, state);
            return result;
        }

        // 目前支持非标准的HTTP协议，允许调用者通过一个参数来指定使用表单传参，而请求本身可以不携带 Content-Type。
        // Content-Type 不对的情况下 ASP.net 并不会解析表单信息，需要单独处理。
        // 只支持 application/x-www-form-urlencoded 型的表单；不支持 multipart/form-data 。
        // 如果需要单独处理，则返回解析到的参数；否则返回 null 。
        private static void TryAppendFormData(
            Dictionary<string, string> result, HttpRequest request, object state)
        {
            // 同时满足下述情况则需要单独解析：
            // 1. 请求指定了一个特殊参数；
            // 2. ASP.net 本身不解析表单。
            if (!(state is SlimApiRequestState slimApiRequestState)
                || slimApiRequestState.RequestFormat != SlimApiHttpHandler.MetaRequestFormatPost // case 1
                || request.Form.Count > 0) // case 2
            {
                return;
            }

            var encoding = GuessRequestEncoding(request);
            var body = request.TextBody(encoding);
            if (string.IsNullOrEmpty(body))
                return;

            var param = HttpUtility.ParseQueryString(body);
            if (param.Count >= 0)
            {
                foreach (var key in param.AllKeys)
                {
                    result[key] = param[key];
                }
            }
        }

        private static Encoding GuessRequestEncoding(HttpRequest request)
        {
            // 如果直接使用 request.ContentEncoding，在请求没有指定 charset 的情况下，会使用默认字符集或配置文件里的字符集。
            // 在中文系统下默认编码一般是 GB2312，而当前的实现并不标准，也就是不能靠这个配置，故不能直接使用 ContentEncoding 。
            if (request.ContentType.Contains("charset="))
                return request.ContentEncoding;

            return SlimApiHttpHandler.DefaultEncoding;
        }
    }
}
