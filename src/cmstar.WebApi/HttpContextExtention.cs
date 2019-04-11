using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace cmstar.WebApi
{
    internal static class HttpContextExtention
    {
        /// <summary>
        /// 从路由信息中获取指定的参数值。
        /// </summary>
        /// <param name="routeData">路由信息。</param>
        /// <param name="key">参数的名称。</param>
        /// <returns>参数的值。无此参数时返回null。</returns>
        public static string Param(this RouteData routeData, string key)
        {
            var res = routeData.Values[key];
            return res == null ? null : res.ToString();
        }

        /// <summary>
        /// 按照先<see cref="HttpRequest.QueryString"/>后<see cref="HttpRequest.Form"/>的
        /// 顺序查询指定名称的HTTP请求参数。此方法不会查询cookie及服务器变量。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="key">参数的名称。</param>
        /// <returns>参数的值。无此参数时返回null。</returns>
        public static string ExplicicParam(this HttpRequest request, string key)
        {
            return request.QueryString[key] ?? request.Form[key];
        }

        /// <summary>
        /// 按照先<see cref="HttpRequest.QueryString"/>后<see cref="HttpRequest.Form"/>的
        /// 顺序逐一给出HTTP请求参数的名称。此方法不会查询cookie及服务器变量。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>HTTP请求参数的名称的序列。</returns>
        public static IEnumerable<string> ExplicicParamKeys(this HttpRequest request)
        {
            foreach (var key in request.QueryString.AllKeys)
            {
                yield return key;
            }

            foreach (var key in request.Form.AllKeys)
            {
                yield return key;
            }
        }

        /// <summary>
        /// 以文本形式从请求的HTTP body中读取所有的内容。
        /// </summary>
        public static string TextBody(this HttpRequest request, Encoding encoding)
        {
            using (var reader = new StreamReader(request.InputStream, encoding))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
