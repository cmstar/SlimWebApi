using System.Collections.Generic;
using System.Web;

namespace cmstar.WebApi
{
    internal static class HttpContextExtention
    {
        /// <summary>
        /// 按照先<see cref="HttpRequest.QueryString"/>后<see cref="HttpRequest.Form"/>的
        /// 顺序查询指定名称的HTTP请求参数。此方法不会查询cookie及服务器变量。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="key">参数的名称。</param>
        /// <returns>参数的值。</returns>
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
    }
}
