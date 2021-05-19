using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace cmstar.WebApi
{
    /// <summary>
    /// 用于获取<see cref="HttpClient"/>（实际上是<see cref="HttpClientContainer"/>），以实现<see cref="HttpClient"/>的复用。
    /// </summary>
    /// <remarks>
    /// HttpClient 应尽量单例使用。
    /// ===
    /// HttpClient is intended to be instantiated once and reused throughout the life of an application.
    /// The following conditions can result in SocketException errors:
    ///     - Creating a new HttpClient instance per request.
    ///     - Server under heavy load.
    /// ===
    /// ref https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    /// ref https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
    /// ref https://medium.com/@nuno.caneco/c-httpclient-should-not-be-disposed-or-should-it-45d2a8f568bc
    /// ref https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient
    /// </remarks>
    public static class HttpClientPool
    {
        private static readonly ConcurrentDictionary<Tuple<string, int>, HttpClientContainer> Clients
            = new ConcurrentDictionary<Tuple<string, int>, HttpClientContainer>();

        /// <summary>
        /// <see cref="DefaultClient"/>的超时时间。
        /// </summary>
        public const int DefaultTimeout = 30000;

        /// <summary>
        /// 获取具有默认超时时间（<see cref="DefaultTimeout"/>）的<see cref="HttpClientContainer"/>。
        /// </summary>
        public static HttpClientContainer DefaultClient => GetClient(DefaultTimeout);

        /// <summary>
        /// 获取具有指定超时时间的<see cref="HttpClientContainer"/>。
        /// 其包含的<see cref="HttpClient.BaseAddress"/>是未经设置的。
        /// </summary>
        /// <returns><see cref="HttpClientContainer"/>实例，可以被缓存并重复使用。</returns>
        public static HttpClientContainer GetClient(int millisecondsTimeout)
        {
            return Clients.GetOrAdd(
                new Tuple<string, int>(null, millisecondsTimeout)
                , x => CreateHttpClientContainer(null, millisecondsTimeout));
        }

        /// <summary>
        /// 获取具有指定超时时间和地址（<see cref="HttpClient.BaseAddress"/>）的<see cref="HttpClientContainer"/>。
        /// </summary>
        /// <returns><see cref="HttpClientContainer"/>实例，可以被缓存并重复使用。</returns>
        public static HttpClientContainer GetClient(string baseAddress, int millisecondsTimeout)
        {
            return Clients.GetOrAdd(
                new Tuple<string, int>(baseAddress, millisecondsTimeout)
                , x => CreateHttpClientContainer(baseAddress, millisecondsTimeout));
        }

        private static HttpClientContainer CreateHttpClientContainer(string baseAddress, int millisecondsTimeout)
        {
            // 当前方法被 ConcurrentDictionary 并发调用时可能重复创建具有相同属性的 HttpClientContainer ，并不要紧。
            // 创建成本不大，多余的实例会被舍弃掉。只要实例的方法没有被调用到，就不会占用相关网络资源。
            var client = new HttpClient
            {
                BaseAddress = baseAddress == null ? null : new Uri(baseAddress),
                Timeout = TimeSpan.FromMilliseconds(millisecondsTimeout)
            };

            return new HttpClientContainer(client);
        }
    }
}
