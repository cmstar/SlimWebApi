using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Threading.Tasks;

#if !NETFX
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
    /// <summary>
    /// 包含<see cref="HttpContext"/>及其相关类型的扩展方法。
    /// </summary>
    public static class HttpContextExtensions
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
#if !NETFX
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
        /// 此方法用于在 .net Core 上模拟 .net Framework 的 HttpRequest.QueryString 属性。
        /// 在传统ASP.net中，“?a”解析为一个 key 为 null，值为“a”的参数；而.net Core中，则解析为 key 为“a”值为“”的参数。
        /// 此方法沿用旧版的逻辑。但在 ASP.net Core 上没有将“a”参数移除，如果使用“a”获取参数值，仍会获取到一个值“”，
        /// 在使用过程中应避免这种情况。
        /// </remarks>
        public static string LegacyQueryString(this HttpRequest request, string key)
        {
#if !NETFX
            if (key == null)
                return ReadKeylessQueryString(request.QueryString.Value);

            var values = request.Query[key];
            switch (values.Count)
            {
                case 0: return null;
                case 1: return values[0];
                default: return string.Join(",", values);
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
        /// <remarks>
        /// 此方法用于在 .net Core 上模拟 .net Framework 的 HttpRequest.Form 属性。
        /// </remarks>
        public static string LegacyForm(this HttpRequest request, string key)
        {
#if !NETFX
            if (!request.HasFormContentType)
                return null;

            var values = request.Form[key];
            switch (values.Count)
            {
                case 0: return null;
                case 1: return values[0];
                default: return string.Join(",", values);
            }
#else
            return request.Form[key];
#endif
        }

        /// <summary>
        /// 获取具有指定名称的 HTTP 请求头的值。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="name">请求头字段的名称。</param>
        /// <returns>请求头的对应字段的值。没有给定名称的请求头时返回null。</returns>
        /// <remarks>
        /// 此方法用于在 .net Core 上模拟 .net Framework 的 HttpRequest.Headers 属性。
        /// </remarks>
        public static string LegacyHeader(this HttpRequest request, string name)
        {
#if !NETFX
            var values = request.Headers[name];
            return values.Count > 0 ? values[0] : null;
#else
            return request.Headers[name];
#endif
        }

        /// <summary>
        /// 依次尝试从<see cref="HttpRequest.QueryString"/>、<see cref="HttpRequest.Form"/>、
        /// <see cref="HttpRequest.Cookies"/>读取具有指定名称的字段的值。返回第一个读取到的字段。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <param name="name">所需字段的名称。</param>
        /// <returns>字段的值。无此字段时返回null。</returns>
        /// <remarks>
        /// 此方法用于在 .net Core 上模拟 .net Framework 的 HttpRequest.Params 属性。
        /// </remarks>
        public static string LegacyParam(this HttpRequest request, string name)
        {
#if !NETFX
            return LegacyQueryString(request, name) ?? LegacyForm(request, name) ?? request.Cookies[name];
#else
            return request.Params[name];
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
#if !NETFX
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
#if !NETFX
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
        /// 获取一个支持支持<see cref="Stream.Seek"/>的流，且当前位置位于流的开头，用于读取请求的BODY部分。
        /// </summary>
        public static Stream RequestInputStream(this HttpRequest request)
        {
#if !NETFX
            var body = request.Body;
#else
            var body = request.InputStream;
#endif
            body.Position = 0;
            return body;
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
        /// 从请求的 HTTP body 中读取所有的数据。读取从输入流的当前位置开始，读取到流的末尾。
        /// 若没有读取到数据，返回一个空数组。
        /// </summary>
        /// <param name="request">请求对象。</param>
        /// <returns>HTTP body 中的数据。若没有读取到数据，返回一个空数组。</returns>
        public static byte[] BinaryReadToEnd(this HttpRequest request)
        {
            const int maxBufferLength = 30720;

            var inputStream = request.RequestInputStream();
            var inputLength = (int)inputStream.Length;
            var data = new byte[inputLength];

            // 有的流不支持 0 长度的 buffer ，例如 .net Core 的 FileBufferingStream 会抛出 ArgumentOutOfRangeException ；
            // 但有的流支持，例如 MemoryStream 。这里统一做前置判定。
            if (inputLength == 0)
                return data;

            var bufferLength = Math.Min(inputLength, maxBufferLength);
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
        /// <remarks>
        /// 此方法用于在 .net Core 上模拟 .net Framework 的 HttpRequest.Url.OriginalString 属性。
        /// </remarks>
        public static string FullUrl(this HttpRequest request)
        {
#if !NETFX
            return request.GetEncodedUrl();
#else
            return request.Url.OriginalString;
#endif
        }

        /// <summary>
        /// 获取请求方的IP地址。
        /// </summary>
        /// <remarks>
        /// 此方法用于在 .net Core 上模拟 .net Framework 的 HttpRequest.UserHostAddress 属性。
        /// </remarks>
        public static string UserHostAddress(this HttpRequest request)
        {
#if !NETFX
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
#if !NETFX
            return response.Body.WriteAsync(buffer, 0, buffer.Length);
#else
            response.BinaryWrite(buffer);
            return Task.CompletedTask;
#endif
        }

        /// <summary>
        /// 获取表单请求所携带的文件集合<see cref="IFormFileCollection"/>。
        /// 对于非表单类型的请求，返回空集。
        /// </summary>
        public static IFormFileCollection FormFiles(this HttpRequest request)
        {
#if !NETFX
            if (!request.HasFormContentType)
                return new FormFileCollection();

            return request.Form.Files;
#else
            return new FormFileCollectionImpl(request.Files);
#endif
        }

#if !NETFX
        /// <summary>
        /// 对请求的BODY部分启用缓存，使BODY支持随机、多次的读取。
        /// 调用此方法后，<see cref="HttpRequest.Body"/>的读取位置（<see cref="Stream.Position"/>）将位于其开头。
        /// </summary>
        /// <remarks>
        /// BODY将被缓存，以支持<see cref="Stream.Seek"/>。BODY越大，占用内存越多。
        /// </remarks>
        public static void TryEnableRequestBuffering(this HttpRequest request)
        {
            // 利用 .net Core 2.1 版提供的BODY缓存机制。
            if (!request.Body.CanSeek)
            {
                request.EnableRewind();
            }

            // EnableRewind 使用一个 MemoryStream 做缓存，并使用延迟写入——数据在首次读取时写入缓存。
            // 如果不把数据读一遍，缓存是空的，导致的获取流长度（Length 属性）为0。
            // 要让 Body 具有完整的功能，得把数据读一遍。
            request.Body.Position = 0;
            var buffer = new byte[256];
            while (request.Body.Read(buffer, 0, buffer.Length) > 0) { }

            // 重置到开头。
            request.Body.Position = 0;
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
