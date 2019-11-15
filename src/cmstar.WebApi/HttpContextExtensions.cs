using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Threading.Tasks;

#if NETCORE
using System.Linq;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http.Extensions;
#else
using System.Web.Routing;
#endif

namespace cmstar.WebApi
{
    internal static class HttpContextExtensions
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
            return res?.ToString();
        }

        /// <summary>
        /// 获取<see cref="HttpRequest.QueryString"/>中的所有参数名称。
        /// </summary>
        public static string[] QueryStringKeys(this HttpRequest request)
        {
#if NETCORE
            var keys = request.Query.Keys;
            var values = new string[keys.Count];
            keys.CopyTo(values, 0);
            return values;
#else
            return request.QueryString.AllKeys;
#endif
        }

        /// <summary>
        /// 从<see cref="HttpRequest.QueryString"/>中获取指定参数的值。
        /// 如果参数出现了多次，多个值按出现的顺序以逗号（,）拼接。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="key">参数的名称。可以使用null获取没有key的参数值，如从“?a&amp;foo=bar”得到“a”。</param>
        /// <returns>参数的值。无此参数时返回null。</returns>
        /// <remarks>
        /// 在传统ASP.net中，“?a”解析为一个 key 为 null，值为“a”的参数；而.net Core中，则解析为 key 为“a”值为“”的参数。
        /// 此方法沿用旧版的逻辑。但在 ASP.net Core 上没有将“a”参数移除，如果使用“a”获取参数值，仍会获取到一个值“”，
        /// 在使用过程中应避免这种情况。
        /// </remarks>
        public static string LegacyQueryString(this HttpRequest request, string key)
        {
#if NETCORE
            if (key == null)
                return ReadKeylessQueryString(request.QueryString.Value);

            var values = request.Query[key];
            switch (values.Count)
            {
                case 0: return null;
                case 1: return values[0];
                default: return string.Join(',', values);
            }
#else
            return request.QueryString[key];
#endif
        }

        /// <summary>
        /// 从<see cref="HttpRequest.QueryString"/>中获取指定参数的值。
        /// 如果参数出现了多次，多个值按出现的顺序以逗号（,）拼接。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="key">参数的名称。</param>
        /// <returns>参数的值。无此参数时返回null。</returns>
        public static string LegacyForm(this HttpRequest request, string key)
        {
#if NETCORE
            if (!request.HasFormContentType)
                return null;

            var values = request.Form[key];
            switch (values.Count)
            {
                case 0: return null;
                case 1: return values[0];
                default: return string.Join(',', values);
            }
#else
            return request.Form[key];
#endif
        }

        /// <summary>
        /// 按照先<see cref="HttpRequest.QueryString"/>后<see cref="HttpRequest.Form"/>的
        /// 顺序查询指定名称的HTTP请求参数。此方法不会查询cookie及服务器变量。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="key">参数的名称。</param>
        /// <returns>参数的值。无此参数时返回null。</returns>
        public static string ExplicitParam(this HttpRequest request, string key)
        {
#if NETCORE
            return LegacyQueryString(request, key) ?? LegacyForm(request, key);
#else
            return request.QueryString[key] ?? request.Form[key];
#endif
        }

        /// <summary>
        /// 按照先<see cref="HttpRequest.QueryString"/>后<see cref="HttpRequest.Form"/>的
        /// 顺序逐一给出HTTP请求参数的名称。此方法不会查询cookie及服务器变量。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>HTTP请求参数的名称的序列。</returns>
        public static IEnumerable<string> ExplicitParamKeys(this HttpRequest request)
        {
#if NETCORE
            var keys = request.Query.Keys;
            return request.HasFormContentType ? keys.Union(request.Form.Keys) : keys;
#else
            foreach (var key in request.QueryString.AllKeys)
            {
                yield return key;
            }

            foreach (var key in request.Form.AllKeys)
            {
                yield return key;
            }
#endif
        }

        /// <summary>
        /// 获取一个支持支持<see cref="Stream.Seek"/>的流，用于读取请求的BODY部分。
        /// </summary>
        /// <remarks>
        /// BODY将被缓存，以支持<see cref="Stream.Seek"/>。BODY越大，占用内存越多。
        /// </remarks>
        public static Stream RequestInputStream(this HttpRequest request)
        {
#if NETCORE
            // 当前框架需要BODY部分可以被重复读取。目前ASP.net Core对于表单类型的请求会
            // 自动缓存BODY，其他类型则不会，需要调用 EnableRewind 开启缓存，支持 .net Core 2+ 。
            if (!request.Body.CanSeek)
            {
                request.EnableRewind();
            }

            return request.Body;
#else
            return request.InputStream;
#endif
        }

        /// <summary>
        /// 以文本形式从请求的HTTP body中读取所有的内容。
        /// </summary>
        public static string TextBody(this HttpRequest request, Encoding encoding)
        {
            using (var reader = new StreamReader(request.RequestInputStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 从请求的HTTP body中读取所有的数据。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>HTTP body中的数据。</returns>
        public static byte[] BinaryReadToEnd(this HttpRequest request)
        {
            const int bufferLength = 4096;

            var inputStream = request.RequestInputStream();
            var data = new byte[inputStream.Length];
            var pos = 0;

            int len;
            while ((len = inputStream.Read(data, pos, bufferLength)) > 0)
            {
                pos += len;
            }

            return data;
        }

        /// <summary>
        /// 获取请求的完整的URL，包含协议、域名、参数。
        /// </summary>
        public static string FullUrl(this HttpRequest request)
        {
#if NETCORE
            return request.GetEncodedUrl();
#else
            return request.Url.OriginalString;
#endif
        }

        /// <summary>
        /// 获取请求方的IP地址。
        /// </summary>
        public static string UserHostAddress(this HttpRequest request)
        {
#if NETCORE
            return request.HttpContext.Connection.RemoteIpAddress?.ToString();
#else
            return request.UserHostAddress;
#endif
        }

        /// <summary>
        /// Writes a string of binary characters to the HTTP output stream.
        /// </summary>
        public static Task BinaryWriteAsync(this HttpResponse response, byte[] buffer)
        {
#if NETCORE
            return response.Body.WriteAsync(buffer, 0, buffer.Length);
#else
            response.BinaryWrite(buffer);
            return Task.CompletedTask;
#endif
        }

#if NETCORE
        /// <summary>
        /// 获取表单请求所携带的文件集合<see cref="IFormFileCollection"/>。
        /// 对于非表单类型的请求，返回空集。
        /// </summary>
        public static IFormFileCollection FormFiles(this HttpRequest request)
        {
            if (!request.HasFormContentType)
                return new FormFileCollection();

            return request.Form.Files;
        }

        private static string ReadKeylessQueryString(string queryString)
        {
            if (string.IsNullOrEmpty(queryString) || queryString == "?")
                return null;

            var idxLeft = 0;
            if (queryString[0] == '?')
            {
                idxLeft = 1;
            }

            var length = queryString.Length;
            string result = null;
            int idxRight;
            for (; idxLeft < length; idxLeft = idxRight + 1)
            {
                idxRight = queryString.IndexOf('&', idxLeft);
                if (idxRight == -1)
                {
                    idxRight = length;
                }

                var idxEq = queryString.IndexOf('=', idxLeft, idxRight - idxLeft);
                if (idxEq >= 0)
                    continue;

                var value = HttpUtility.UrlDecode(queryString.Substring(idxLeft, idxRight - idxLeft));
                if (result == null)
                {
                    result = value;
                }
                else
                {
                    result += "," + value;
                }

                // queryString ends with '&'
                if (idxRight == length - 1)
                {
                    result += ",";
                    break;
                }
            }

            return result;
        }
#else
        /// <summary>
        /// Writes a string to an HTTP response stream.
        /// </summary>
        public static Task WriteAsync(this HttpResponse response, string s)
        {
            response.Write(s);
            return Task.CompletedTask;
        }
#endif
    }
}
