using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using cmstar.Serialization.Json;

namespace cmstar.WebApi.Slim
{
    // SlimApiInvoker的异步方法
    public partial class SlimApiInvoker
    {
        /// <summary>
        /// 默认的执行异步操作的超时时间。
        /// </summary>
        public const int DefaultAsyncTimeout = DefaultConnectionTimeout + DefaultReadWriteTimeout;

        /// <summary>
        /// 上传一个文件到指定接口，该接口没有返回值。
        /// </summary>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据，以 QueryString 形势拼接在 URL 上。</param>
        /// <param name="fileName">指定上传的文件的名称。</param>
        /// <param name="file">上传的文件。文件上传后，其<see cref="Stream.Position"/>位于流的末尾。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        public static Task UploadFileAsync(
            string entry, string method, IDictionary<string, string> param,
            string fileName, Stream file,
            int timeout = DefaultAsyncTimeout)
        {
            return UploadFileAsync<object>(entry, method, param, fileName, file, timeout);
        }

        /// <summary>
        /// 上传一个文件到指定接口，并返回不包含数据信封的结果，即<see cref="ApiResponse{T}.Data"/>。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据，以 QueryString 形势拼接在 URL 上。</param>
        /// <param name="fileName">指定上传的文件的名称。</param>
        /// <param name="file">上传的文件。文件上传后，其<see cref="Stream.Position"/>位于流的末尾。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public static async Task<T> UploadFileAsync<T>(
            string entry, string method, IDictionary<string, string> param,
            string fileName, Stream file,
            int timeout = DefaultAsyncTimeout)
        {
            var response = await RawUploadFileAsync<T>(entry, method, param, fileName, file, timeout);
            if (response.Code != 0)
                throw new ApiException(response.Code, response.Message);

            return response.Data;
        }

        /// <summary>
        /// 上传一个文件到指定接口，并返回包含数据信封的结果。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据，以 QueryString 形势拼接在 URL 上。</param>
        /// <param name="fileName">指定上传的文件的名称。</param>
        /// <param name="file">上传的文件。文件上传后，其<see cref="Stream.Position"/>位于流的末尾。</param>
        /// <param name="millisecondsTimeout">异步操作的超时时间，单位为毫秒。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public static async Task<ApiResponse<T>> RawUploadFileAsync<T>(
            string entry, string method, IDictionary<string, string> param,
            string fileName, Stream file,
            int millisecondsTimeout = DefaultAsyncTimeout)
        {
            var url = BuildEntryUrl(entry, method, SlimApiHttpHandler.MetaRequestFormatGet);
            url = AppendQueryString(url, param);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(file), "file", fileName ?? string.Empty);

            var httpClient = HttpClientPool.GetClient(millisecondsTimeout);
            var httpResponse = await httpClient.PostAsync(url, content);
            httpResponse.EnsureSuccessStatusCode();

            var responseData = await httpResponse.Content.ReadAsByteArrayAsync();
            var responseJson = SlimApiHttpHandler.DefaultEncoding.GetString(responseData);
            var response = JsonHelper.DeserializeApiResponse<T>(responseJson);
            return response;
        }

        /// <summary>
        /// 执行一次API调用，该调用没有返回值。
        /// </summary>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        public static Task InvokeAsync(string entry, string method, object param,
            int timeout = DefaultAsyncTimeout)
        {
            return InvokeAsync<object>(entry, method, param, timeout);
        }

        /// <summary>
        /// 执行一次API调用。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值。</returns>
        public static async Task<T> InvokeAsync<T>(string entry, string method, object param,
            int timeout = DefaultAsyncTimeout)
        {
            var response = await InternalInvokeAsync<T>(entry, method, param, timeout);
            if (response.Code != 0)
                throw new ApiException(response.Code, response.Message);

            return response.Data;
        }

        /// <summary>
        /// 执行一次API调用，并返回包含数据信封的结果。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public static async Task<ApiResponse<T>> RawInvokeAsync<T>(string entry, string method, object param,
            int timeout = DefaultAsyncTimeout)
        {
            return await InternalInvokeAsync<T>(entry, method, param, timeout);
        }

        /// <summary>
        /// 执行一次API调用。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="template">用于提供返回数据的类型的模板实例。</param>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值。</returns>
        public static Task<T> InvokeAsync<T>(T template, string entry, string method, object param,
            int timeout = DefaultAsyncTimeout)
        {
            return InvokeAsync<T>(entry, method, param, timeout);
        }

        /// <summary>
        /// 执行一次API调用，并返回包含数据信封的结果。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="template">用于提供返回数据的类型的模板实例。</param>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public static Task<ApiResponse<T>> RawInvokeAsync<T>(T template, string entry, string method, object param,
            int timeout = DefaultAsyncTimeout)
        {
            return InternalInvokeAsync<T>(entry, method, param, timeout);
        }

        /// <summary>
        /// 上传一个文件到指定接口，该接口没有返回值。
        /// </summary>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据，以 QueryString 形势拼接在 URL 上。</param>
        /// <param name="fileName">指定上传的文件的名称。</param>
        /// <param name="file">上传的文件。文件上传后，其<see cref="Stream.Position"/>位于流的末尾。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        public virtual Task UploadFileAsync(
            string method, IDictionary<string, string> param,
            string fileName, Stream file,
            int timeout = DefaultAsyncTimeout)
        {
            return UploadFileAsync<object>(method, param, fileName, file, timeout);
        }

        /// <summary>
        /// 上传一个文件到指定接口，并返回不包含数据信封的结果，即<see cref="ApiResponse{T}.Data"/>。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据，以 QueryString 形势拼接在 URL 上。</param>
        /// <param name="fileName">指定上传的文件的名称。</param>
        /// <param name="file">上传的文件。文件上传后，其<see cref="Stream.Position"/>位于流的末尾。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public virtual async Task<T> UploadFileAsync<T>(
            string method, IDictionary<string, string> param,
            string fileName, Stream file,
            int timeout = DefaultAsyncTimeout)
        {
            var response = await RawUploadFileAsync<T>(_entry, method, param, fileName, file, timeout);
            if (response.Code != 0)
                throw new ApiException(response.Code, response.Message);

            return response.Data;
        }

        /// <summary>
        /// 上传一个文件到指定接口，并返回包含数据信封的结果。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据，以 QueryString 形势拼接在 URL 上。</param>
        /// <param name="fileName">指定上传的文件的名称。</param>
        /// <param name="file">上传的文件。文件上传后，其<see cref="Stream.Position"/>位于流的末尾。</param>
        /// <param name="timeout">异步操作的超时时间。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public virtual Task<ApiResponse<T>> RawUploadFileAsync<T>(
            string method, IDictionary<string, string> param,
            string fileName, Stream file,
            int timeout = DefaultAsyncTimeout)
        {
            return RawUploadFileAsync<T>(_entry, method, param, fileName, file, timeout);
        }

        /// <summary>
        /// 执行一次API调用，该调用没有返回值。
        /// </summary>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        public virtual Task InvokeAsync(string method, object param)
        {
            return InvokeAsync<object>(method, param);
        }

        /// <summary>
        /// 执行一次API调用。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <returns>API返回值。</returns>
        public virtual async Task<T> InvokeAsync<T>(string method, object param)
        {
            var asyncTimeout = _connectionTimeout + _readWriteTimeout;
            var response = await InternalInvokeAsync<T>(_entry, method, param, asyncTimeout);
            if (response.Code != 0)
                throw new ApiException(response.Code, response.Message);

            return response.Data;
        }

        /// <summary>
        /// 执行一次API调用，并返回包含数据信封的结果。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public virtual Task<ApiResponse<T>> RawInvokeAsync<T>(string method, object param)
        {
            var asyncTimeout = _connectionTimeout + _readWriteTimeout;
            return InternalInvokeAsync<T>(_entry, method, param, asyncTimeout);
        }

        private static async Task<ApiResponse<T>> InternalInvokeAsync<T>(string entry, string method, object param, int timeout)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(entry, nameof(entry));

            var requestJson = JsonSerializer.Default.Serialize(param);
            try
            {
                var url = BuildEntryUrl(entry, method);

                // 在异步操作中，TimedWebClient超时使用设置的两个超时之和，
                // 这里初始化就将两个超时各设置为超时时间的一半即可，虽然谁多谁少并没有什么意义。
                var halfTimeout = timeout / 2;
                var webClient = new TimedWebClient(halfTimeout, halfTimeout);

                var responseData = await webClient.UploadDataTaskAsync(url, Encoding.UTF8.GetBytes(requestJson));
                var responseJson = Encoding.UTF8.GetString(responseData);
                var response = JsonHelper.DeserializeApiResponse<T>(responseJson);
                return response;
            }
            catch (Exception ex)
            {
                var msg = "Error thrown while requesting target with param: " + requestJson;
                throw new ApiClientException(msg, ex);
            }
        }

        private static string AppendQueryString(string url, IDictionary<string, string> param)
        {
            var builder = new StringBuilder(url);
            var hasQuestionMark = url.IndexOf('?') > 0;

            foreach (var kv in param)
            {
                if (hasQuestionMark)
                {
                    builder.Append('&');
                }
                else
                {
                    builder.Append('?');
                    hasQuestionMark = true;
                }

                builder.Append(kv.Key).Append('=').Append(HttpUtility.UrlEncode(kv.Value));
            }

            return builder.ToString();
        }
    }
}
