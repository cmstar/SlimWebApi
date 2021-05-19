using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace cmstar.WebApi
{
    /// <summary>
    /// 封装<see cref="HttpClient"/>，隐藏其部分属性和方法，避免这些成员被误用。
    /// 封装后提供的方法都是线程安全的。
    /// </summary>
    /// <remarks>
    /// <see cref="HttpClient"/>应尽量单例化使用，但其部分成员如
    /// <see cref="HttpClient.BaseAddress"/>和<see cref="HttpClient.Timeout"/>却和特定的业务相关联。
    /// 单例是全局共享的，如果使用过程中错误的设置了这些成员，可能导致程序运行结果不符合预期。
    /// </remarks>
    public class HttpClientContainer
    {
        private readonly HttpClient _client;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="client">被封装的<see cref="HttpClient"/>实例。</param>
        public HttpClientContainer(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<string> GetStringAsync(string requestUri) => _client.GetStringAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a string in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<string> GetStringAsync(Uri requestUri) => _client.GetStringAsync(requestUri);

        /// <summary>
        /// Sends a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<byte[]> GetByteArrayAsync(string requestUri) => _client.GetByteArrayAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a byte array in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<byte[]> GetByteArrayAsync(Uri requestUri) => _client.GetByteArrayAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<Stream> GetStreamAsync(string requestUri) => _client.GetStreamAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri and return the response body as a stream in an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<Stream> GetStreamAsync(Uri requestUri) => _client.GetStreamAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(string requestUri) => _client.GetAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(Uri requestUri) => _client.GetAsync(requestUri);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
            => _client.GetAsync(requestUri, completionOption);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP completion option value that indicates when the operation should be considered completed.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
            => _client.GetAsync(requestUri, completionOption);

        /// <summary>
        /// Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
            => _client.GetAsync(requestUri, cancellationToken);

        /// <summary>
        /// Send a GET request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
            => _client.GetAsync(requestUri, cancellationToken);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(
            string requestUri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
            => _client.GetAsync(requestUri, completionOption, cancellationToken);

        /// <summary>
        /// Send a GET request to the specified Uri with an HTTP completion option and a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="completionOption">An HTTP  completion option value that indicates when the operation should be considered completed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> GetAsync(
            Uri requestUri,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
            => _client.GetAsync(requestUri, completionOption, cancellationToken);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
            => _client.PostAsync(requestUri, content);

        /// <summary>
        /// Send a POST request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
            => _client.PostAsync(requestUri, content);

        /// <summary>
        /// Send a POST request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PostAsync(
            string requestUri,
            HttpContent content,
            CancellationToken cancellationToken)
            => _client.PostAsync(requestUri, content, cancellationToken);

        /// <summary>
        /// Send a POST request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PostAsync(
            Uri requestUri,
            HttpContent content,
            CancellationToken cancellationToken)
            => _client.PostAsync(requestUri, content, cancellationToken);

        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
            => _client.PostAsync(requestUri, content);

        /// <summary>
        /// Send a PUT request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
            => _client.PostAsync(requestUri, content);

        /// <summary>
        /// Send a PUT request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PutAsync(
            string requestUri,
            HttpContent content,
            CancellationToken cancellationToken)
            => _client.PostAsync(requestUri, content, cancellationToken);

        /// <summary>
        /// Send a PUT request with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="content">The HTTP request content sent to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> PutAsync(
            Uri requestUri,
            HttpContent content,
            CancellationToken cancellationToken)
            => _client.PostAsync(requestUri, content, cancellationToken);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> DeleteAsync(string requestUri) => _client.DeleteAsync(requestUri);

        /// <summary>
        /// Send a DELETE request to the specified Uri as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri) => _client.DeleteAsync(requestUri);

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
            => _client.DeleteAsync(requestUri, cancellationToken);

        /// <summary>
        /// Send a DELETE request to the specified Uri with a cancellation token as an asynchronous operation.
        /// </summary>
        /// <param name="requestUri">The Uri the request is sent to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="requestUri" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
            => _client.DeleteAsync(requestUri, cancellationToken);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) => _client.SendAsync(request);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => _client.SendAsync(request, cancellationToken);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
            => _client.SendAsync(request, completionOption);

        /// <summary>
        /// Send an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="completionOption">When the operation should complete (as soon as a response is available or after reading the whole response content).</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="request" /> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The request message was already sent by the <see cref="T:System.Net.Http.HttpClient" /> instance.
        /// </exception>
        /// <exception cref="HttpRequestException">
        /// The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.
        /// </exception>
        public Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken)
            => _client.SendAsync(request, completionOption, cancellationToken);
    }
}
