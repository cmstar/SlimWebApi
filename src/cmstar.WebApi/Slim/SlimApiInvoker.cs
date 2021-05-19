using System;
using System.Text;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 包含调用SlimAPI的有关辅助方法。
    /// </summary>
#if NET35
    public class SlimApiInvoker
#else
    public partial class SlimApiInvoker
#endif
    {
        /// <summary>
        /// 默认的建立连接超时设置，单位为毫秒。
        /// </summary>
        public const int DefaultConnectionTimeout = 3000;

        /// <summary>
        /// 默认的读写数据的超时设置，单位为毫秒。
        /// </summary>
        public const int DefaultReadWriteTimeout = 25000;

        private readonly string _entry;
        private readonly int _connectionTimeout;
        private readonly int _readWriteTimeout;

        /// <summary>
        /// 执行一次API调用，该调用没有返回值。
        /// </summary>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        public static void Invoke(string entry, string method, object param,
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            Invoke<object>(entry, method, param, connectionTimeout, readWriteTimeout);
        }

        /// <summary>
        /// 执行一次API调用。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        /// <returns>API返回值。</returns>
        public static T Invoke<T>(string entry, string method, object param,
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            var response = InternalInvoke<T>(entry, method, param, connectionTimeout, readWriteTimeout);
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
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public static ApiResponse<T> RawInvoke<T>(string entry, string method, object param,
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            return InternalInvoke<T>(entry, method, param, connectionTimeout, readWriteTimeout);
        }

        /// <summary>
        /// 执行一次API调用。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="template">用于提供返回数据的类型的模板实例。</param>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        /// <returns>API返回值。</returns>
        public static T Invoke<T>(T template, string entry, string method, object param,
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            return Invoke<T>(entry, method, param, connectionTimeout, readWriteTimeout);
        }

        /// <summary>
        /// 执行一次API调用，并返回包含数据信封的结果。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="template">用于提供返回数据的类型的模板实例。</param>
        /// <param name="entry">API链接模板。</param>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        /// <returns>API返回值，连同信封部分。</returns>
        public static ApiResponse<T> RawInvoke<T>(T template, string entry, string method, object param,
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            return InternalInvoke<T>(entry, method, param, connectionTimeout, readWriteTimeout);
        }

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="entry">API链接模板。</param>
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        public SlimApiInvoker(string entry,
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(entry, nameof(entry));
            _entry = entry;
            _connectionTimeout = connectionTimeout;
            _readWriteTimeout = readWriteTimeout;
        }

        /// <summary>
        /// 获取当前实例使用的API链接模板。
        /// </summary>
        public string Entry => _entry;

        /// <summary>
        /// 执行一次API调用，该调用没有返回值。
        /// </summary>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        public virtual void Invoke(string method, object param)
        {
            Invoke<object>(method, param);
        }

        /// <summary>
        /// 执行一次API调用。
        /// </summary>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <param name="method">调用的方法名称。</param>
        /// <param name="param">请求的数据。</param>
        /// <returns>API返回值。</returns>
        public virtual T Invoke<T>(string method, object param)
        {
            var response = InternalInvoke<T>(_entry, method, param, _connectionTimeout, _readWriteTimeout);
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
        public virtual ApiResponse<T> RawInvoke<T>(string method, object param)
        {
            return InternalInvoke<T>(_entry, method, param, _connectionTimeout, _readWriteTimeout);
        }

        private static ApiResponse<T> InternalInvoke<T>(
            string entry, string method, object param, int connectionTimeout, int readWriteTimeout)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(entry, nameof(entry));

            var requestJson = JsonHelper.Serialize(param);
            try
            {
                var url = BuildEntryUrl(entry, method);
                var webClient = new TimedWebClient(connectionTimeout, readWriteTimeout);
                var responseData = webClient.UploadData(url, Encoding.UTF8.GetBytes(requestJson));
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

        /// <summary>
        /// 通过给定的URL模板和方法名称，构建用于发起请求的完整URL。
        /// </summary>
        /// <param name="entry">URL模板。支持URL路径占位符‘{~method}’、‘{~format}’和‘{~callback}’。</param>
        /// <param name="method">方法的名称。</param>
        /// <returns>用于请求的URL。</returns>
        /// <remarks>
        /// 构建的URL带有‘~format=json’或其等价形式，所以请求时，参数必须以JSON方式POST到目标地址。
        /// </remarks>
        public static string BuildEntryUrl(string entry, string method)
        {
            return BuildEntryUrl(entry, method, SlimApiHttpHandler.MetaRequestFormatJson);
        }

        /// <summary>
        /// 通过给定的URL模板和方法名称，构建用于发起请求的完整URL。
        /// </summary>
        /// <param name="entry">URL模板。支持URL路径占位符‘{~method}’、‘{~format}’和‘{~callback}’。</param>
        /// <param name="method">方法的名称。</param>
        /// <param name="format">指定数据格式，对应‘{~format}’参数。</param>
        /// <returns>用于请求的URL。</returns>
        internal static string BuildEntryUrl(string entry, string method, string format)
        {
            var entryLen = entry.Length;
            var builder = new StringBuilder(entryLen + 64);
            var start = 0;
            var hasMethod = false;
            var hasFormat = false;
            int index;

            while (start < entryLen && (index = entry.IndexOf('{', start)) >= 0)
            {
                builder.Append(entry, start, index - start);

                // 处理转义符的情况，'{'之后若再接着'{'，说明是转义符
                var next = index + 1;
                if (next < entryLen && entry[next] == '{')
                {
                    builder.Append('{');
                    start = next + 1;
                    continue;
                }

                if (MetaParamMatches(entry, index, SlimApiHttpHandler.MetaParamMethodName))
                {
                    builder.Append(method);
                    start = index + SlimApiHttpHandler.MetaParamMethodName.Length + 2; // 2来自{}
                    hasMethod = true;
                }
                else if (MetaParamMatches(entry, index, SlimApiHttpHandler.MetaParamFormat))
                {
                    builder.Append(format);
                    start = index + SlimApiHttpHandler.MetaParamFormat.Length + 2; // 2来自{}
                    hasFormat = true;
                }
                else
                {
                    builder.Append(entry[index]);
                    start = index + 1;
                }
            }

            if (start < entryLen)
            {
                builder.Append(entry, start, entryLen - start);
            }

            if (!hasMethod || !hasFormat)
            {
                var hasQuestionMark = entry.Contains("?");

                if (!hasMethod)
                {
                    builder.Append(hasQuestionMark ? '&' : '?')
                        .Append(SlimApiHttpHandler.MetaParamMethodName)
                        .Append('=')
                        .Append(method);
                    hasQuestionMark = true;
                }

                if (!hasFormat)
                {
                    builder.Append(hasQuestionMark ? '&' : '?')
                        .Append(SlimApiHttpHandler.MetaParamFormat)
                        .Append('=')
                        .Append(format);
                }
            }

            return builder.ToString();
        }

        private static bool MetaParamMatches(string entry, int index, string metaParam)
        {
            index++; // move to the next char of {

            if (index + metaParam.Length >= entry.Length)
                return false;

            // 匹配参数名称
            for (int i = 0; i < metaParam.Length; i++)
            {
                if (entry[index + i] != metaParam[i])
                    return false;
            }

            // 匹配结尾的'}'
            if (entry[index + metaParam.Length] != '}')
                return false;

            return true;
        }
    }
}
