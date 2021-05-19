#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Text;

namespace cmstar.WebApi
{
    /// <summary>
    /// 支持请求超时设置的<see cref="WebClient"/>。
    /// 默认字符集为<see cref="Encoding.UTF8"/>。
    /// </summary>
#if NET35
    public class TimedWebClient : WebClient
#else
    public partial class TimedWebClient : WebClient
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

        /// <summary>
        /// 默认的字符集，使用 UTF-8 。
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

#if !NET35
        /// <summary>
        /// 默认的执行异步操作的超时时间。
        /// 其值等于<see cref="ConnectionTimeout"/>与<see cref="ReadWriteTimeout"/>之和。
        /// </summary>
        /// <remarks>
        /// 异步操作中，我们并不能区分连接与读写阶段，所以为异步操作添加一个总的超时时间，值为两个超时设置值之和。
        /// </remarks>
        public const int DefaultAsyncTimeout = DefaultConnectionTimeout + DefaultReadWriteTimeout;
#endif

        // 记录最近一次请求的WebRequest实例，用于超时时的Abort操作。
        // 由于WebClient不支持并发操作，同一时间仅允许一个请求，所以这个实例是不会篡掉的。
        private WebRequest _currentWebRequest;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="connectionTimeout">建立连接超时设置，单位为毫秒。</param>
        /// <param name="readWriteTimeout">读写数据的超时设置，单位为毫秒。</param>
        public TimedWebClient(
            int connectionTimeout = DefaultConnectionTimeout,
            int readWriteTimeout = DefaultReadWriteTimeout)
        {
            if (connectionTimeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(connectionTimeout));

            if (readWriteTimeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(readWriteTimeout));

            ConnectionTimeout = connectionTimeout;
            ReadWriteTimeout = readWriteTimeout;
            Encoding = DefaultEncoding;
        }

        /// <summary>
        /// 建立连接超时设置，单位为毫秒。
        /// </summary>
        public int ConnectionTimeout { get; }

        /// <summary>
        /// 读写数据的超时设置，单位为毫秒。
        /// </summary>
        public int ReadWriteTimeout { get; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var baseRequest = base.GetWebRequest(address);
            var request = new DetailedWebRequest(baseRequest, ConnectionTimeout, ReadWriteTimeout);

            // 记录本次请求所使用的WebRequest实例
            _currentWebRequest = request;

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                return base.GetWebResponse(request);
            }
            catch (WebException ex)
            {
                throw PrepareException(request, ex);
            }
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            try
            {
                return base.GetWebResponse(request, result);
            }
            catch (WebException ex)
            {
                throw PrepareException(request, ex);
            }
        }

        public static WebException PrepareException(WebRequest request, Exception raw)
        {
            // ReSharper disable UseStringInterpolation
            string msg;
            switch (request)
            {
                case DetailedWebRequest detailedWebRequest:
                    msg = string.Format(
                        "Error on requesting '{0}' with request-timeout {1}ms, read-write-timeout {2}ms.",
                        detailedWebRequest.RequestUri, detailedWebRequest.Timeout, detailedWebRequest.ReadWriteTimeout);
                    break;

                case HttpWebRequest httpWebRequest:
                    msg = string.Format(
                        "Error on requesting '{0}' with request-timeout {1}ms, read-write-timeout {2}ms.",
                        httpWebRequest.RequestUri, httpWebRequest.Timeout, httpWebRequest.ReadWriteTimeout);
                    break;

                default:
                    msg = string.Format(
                        "Error on requesting '{0}' with request-timeout {1}ms.",
                        request.RequestUri, request.Timeout);
                    break;
            }
            // ReSharper restore UseStringInterpolation

            var res = raw is WebException webException
                ? new WebException(msg, raw, webException.Status, webException.Response)
                : new WebException(msg, raw);
            return res;
        }

        /// <summary>
        /// 相比<see cref="WebRequest"/>，提供更详细的异常信息。
        /// </summary>
        private class DetailedWebRequest : WebRequest
        {
            private readonly WebRequest _raw;

            public DetailedWebRequest(WebRequest raw, int connetctionTimeout, int readWriteTimeout)
            {
                _raw = raw;

                if (connetctionTimeout > 0)
                {
                    _raw.Timeout = connetctionTimeout;
                }

                if (_raw is HttpWebRequest httpRequest && readWriteTimeout > 0)
                {
                    httpRequest.ReadWriteTimeout = readWriteTimeout;
                }
            }

            public int ReadWriteTimeout
            {
                get
                {
                    var httpRequest = _raw as HttpWebRequest;
                    return httpRequest?.ReadWriteTimeout ?? 0;
                }
            }

            public override bool PreAuthenticate
            {
                get { return _raw.PreAuthenticate; }
                set { _raw.PreAuthenticate = value; }
            }

            public override string ContentType
            {
                get { return _raw.ContentType; }
                set { _raw.ContentType = value; }
            }

            public override string ConnectionGroupName
            {
                get { return _raw.ConnectionGroupName; }
                set { _raw.ConnectionGroupName = value; }
            }

            public override int Timeout
            {
                get { return _raw.Timeout; }
                set { _raw.Timeout = value; }
            }

            public override WebHeaderCollection Headers
            {
                get { return _raw.Headers; }
                set { _raw.Headers = value; }
            }

            public override RequestCachePolicy CachePolicy
            {
                get { return _raw.CachePolicy; }
                set { _raw.CachePolicy = value; }
            }

            public override IWebProxy Proxy
            {
                get { return _raw.Proxy; }
                set { _raw.Proxy = value; }
            }

            public override long ContentLength
            {
                get { return _raw.ContentLength; }
                set { _raw.ContentLength = value; }
            }

            public override string Method
            {
                get { return _raw.Method; }
                set { _raw.Method = value; }
            }

            public override Uri RequestUri => _raw.RequestUri;

            public override ICredentials Credentials
            {
                get { return _raw.Credentials; }
                set { _raw.Credentials = value; }
            }

            public override bool UseDefaultCredentials
            {
                get { return _raw.UseDefaultCredentials; }
                set { _raw.UseDefaultCredentials = value; }
            }

            public override Stream GetRequestStream()
            {
                try
                {
                    return _raw.GetRequestStream();
                }
                catch (Exception ex)
                {
                    throw PrepareException(this, ex);
                }
            }

            public override WebResponse GetResponse()
            {
                try
                {
                    return _raw.GetResponse();
                }
                catch (Exception ex)
                {
                    throw PrepareException(this, ex);
                }
            }

            public override void Abort()
            {
                _raw.Abort();
            }

            public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
            {
                return _raw.BeginGetRequestStream(callback, state);
            }

            public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
            {
                return _raw.BeginGetResponse(callback, state);
            }

#if NETFX
            public override System.Runtime.Remoting.ObjRef CreateObjRef(Type requestedType)
            {
                return _raw.CreateObjRef(requestedType);
            }
#endif

            public override Stream EndGetRequestStream(IAsyncResult asyncResult)
            {
                return _raw.EndGetRequestStream(asyncResult);
            }

            public override WebResponse EndGetResponse(IAsyncResult asyncResult)
            {
                return _raw.EndGetResponse(asyncResult);
            }

            public override object InitializeLifetimeService()
            {
                return _raw.InitializeLifetimeService();
            }

            protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
            {
                throw new InvalidOperationException("The operation is invalid.");
            }
        }
    }
}
