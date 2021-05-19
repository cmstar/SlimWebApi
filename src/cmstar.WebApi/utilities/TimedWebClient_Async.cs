using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace cmstar.WebApi
{
    // 覆盖WebClient中的所有 *TaskAsync 方法，以实现异步操作的超时。
    // 因为这些方法都不是虚方法，不能override，所以全部采用new处理。
    public partial class TimedWebClient
    {
        /// <summary>
        /// 获取或设置一个值，该值表示实例内的 *Async 方法是否使用异步执行。
        /// 当值为<c>true</c>时，实例内的 *Async 方法使用异步执行；否则使用非异步方式执行。
        /// 默认值为<c>true</c>。
        /// </summary>
        public bool AsyncEnabled { get; set; } = true;

        /// <summary>
        /// 当启用异步操作（<see cref="AsyncEnabled"/>为true）时，使用的超时时间。
        /// 其值等于<see cref="ConnectionTimeout"/>与<see cref="ReadWriteTimeout"/>之和。
        /// </summary>
        /// <remarks>
        /// 异步操作中，我们并不能区分连接与读写阶段，所以为异步操作添加一个总的超时时间，值为两个超时设置值之和。
        /// </remarks>
        public int AsyncTimeout => ConnectionTimeout + ReadWriteTimeout;

        /// <summary>
        /// 执行<see cref="WebClient.DownloadStringTaskAsync(string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<string> DownloadStringTaskAsync(string address)
        {
            return AsyncEnabled
                ? DoAsync(base.DownloadStringTaskAsync(address))
                : Task.FromResult(DownloadString(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.DownloadStringTaskAsync(Uri)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<string> DownloadStringTaskAsync(Uri address)
        {
            return AsyncEnabled
                ? DoAsync(base.DownloadStringTaskAsync(address))
                : Task.FromResult(DownloadString(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.OpenReadTaskAsync(string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<Stream> OpenReadTaskAsync(string address)
        {
            return AsyncEnabled
                ? DoAsync(base.OpenReadTaskAsync(address))
                : Task.FromResult(OpenRead(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.OpenReadTaskAsync(Uri)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<Stream> OpenReadTaskAsync(Uri address)
        {
            return AsyncEnabled
                ? DoAsync(base.OpenReadTaskAsync(address))
                : Task.FromResult(OpenRead(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.OpenWriteTaskAsync(string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<Stream> OpenWriteTaskAsync(string address)
        {
            return AsyncEnabled
                ? DoAsync(base.OpenWriteTaskAsync(address))
                : Task.FromResult(OpenWrite(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.OpenWriteTaskAsync(Uri)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<Stream> OpenWriteTaskAsync(Uri address)
        {
            return AsyncEnabled
                ? DoAsync(base.OpenWriteTaskAsync(address))
                : Task.FromResult(OpenWrite(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.OpenWriteTaskAsync(string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<Stream> OpenWriteTaskAsync(string address, string method)
        {
            return AsyncEnabled
                ? DoAsync(base.OpenWriteTaskAsync(address, method))
                : Task.FromResult(OpenWrite(address, method));
        }

        /// <summary>
        /// 执行<see cref="WebClient.OpenWriteTaskAsync(Uri, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<Stream> OpenWriteTaskAsync(Uri address, string method)
        {
            return AsyncEnabled
                ? DoAsync(base.OpenWriteTaskAsync(address, method))
                : Task.FromResult(OpenWrite(address, method));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadStringTaskAsync(string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<string> UploadStringTaskAsync(string address, string data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadStringTaskAsync(address, data))
                : Task.FromResult(UploadString(address, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadStringTaskAsync(Uri, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<string> UploadStringTaskAsync(Uri address, string data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadStringTaskAsync(address, data))
                : Task.FromResult(UploadString(address, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadStringTaskAsync(string, string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<string> UploadStringTaskAsync(string address, string method, string data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadStringTaskAsync(address, method, data))
                : Task.FromResult(UploadString(address, method, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadStringTaskAsync(Uri, string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<string> UploadStringTaskAsync(Uri address, string method, string data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadStringTaskAsync(address, method, data))
                : Task.FromResult(UploadString(address, method, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.DownloadDataTaskAsync(string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> DownloadDataTaskAsync(string address)
        {
            return AsyncEnabled
                ? DoAsync(base.DownloadDataTaskAsync(address))
                : Task.FromResult(DownloadData(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.DownloadDataTaskAsync(Uri)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> DownloadDataTaskAsync(Uri address)
        {
            return AsyncEnabled
                ? DoAsync(base.DownloadDataTaskAsync(address))
                : Task.FromResult(DownloadData(address));
        }

        /// <summary>
        /// 执行<see cref="WebClient.DownloadFileTaskAsync(string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task DownloadFileTaskAsync(string address, string fileName)
        {
            if (AsyncEnabled)
                return DoAsync(base.DownloadFileTaskAsync(address, fileName));

            DownloadFile(address, fileName);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行<see cref="WebClient.DownloadFileTaskAsync(Uri, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task DownloadFileTaskAsync(Uri address, string fileName)
        {
            if (AsyncEnabled)
                return DoAsync(base.DownloadFileTaskAsync(address, fileName));

            DownloadFile(address, fileName);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadDataTaskAsync(string, byte[])"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadDataTaskAsync(string address, byte[] data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadDataTaskAsync(address, data))
                : Task.FromResult(UploadData(address, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadDataTaskAsync(Uri, byte[])"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadDataTaskAsync(Uri address, byte[] data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadDataTaskAsync(address, data))
                : Task.FromResult(UploadData(address, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadDataTaskAsync(string, string, byte[])"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadDataTaskAsync(string address, string method, byte[] data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadDataTaskAsync(address, method, data))
                : Task.FromResult(UploadData(address, method, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadDataTaskAsync(Uri, string, byte[])"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadDataTaskAsync(Uri address, string method, byte[] data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadDataTaskAsync(address, method, data))
                : Task.FromResult(UploadData(address, method, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadFileTaskAsync(string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadFileTaskAsync(string address, string fileName)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadFileTaskAsync(address, fileName))
                : Task.FromResult(UploadFile(address, fileName));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadFileTaskAsync(Uri, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadFileTaskAsync(Uri address, string fileName)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadFileTaskAsync(address, fileName))
                : Task.FromResult(UploadFile(address, fileName));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadFileTaskAsync(string, string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadFileTaskAsync(string address, string method, string fileName)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadFileTaskAsync(address, method, fileName))
                : Task.FromResult(UploadFile(address, method, fileName));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadFileTaskAsync(Uri, string, string)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadFileTaskAsync(Uri address, string method, string fileName)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadFileTaskAsync(address, method, fileName))
                : Task.FromResult(UploadFile(address, method, fileName));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadValuesTaskAsync(string, NameValueCollection)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadValuesTaskAsync(string address, NameValueCollection data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadValuesTaskAsync(address, data))
                : Task.FromResult(UploadValues(address, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadValuesTaskAsync(string, string, NameValueCollection)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadValuesTaskAsync(string address, string method, NameValueCollection data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadValuesTaskAsync(address, method, data))
                : Task.FromResult(UploadValues(address, method, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadValuesTaskAsync(Uri, NameValueCollection)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadValuesTaskAsync(Uri address, NameValueCollection data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadValuesTaskAsync(address, data))
                : Task.FromResult(UploadValues(address, data));
        }

        /// <summary>
        /// 执行<see cref="WebClient.UploadValuesTaskAsync(Uri, string, NameValueCollection)"/>。
        /// 当启用了异步操作时，若用时超过<see cref="AsyncTimeout"/>，则抛出<see cref="TimeoutException"/>。
        /// </summary>
        public new Task<byte[]> UploadValuesTaskAsync(Uri address, string method, NameValueCollection data)
        {
            return AsyncEnabled
                ? DoAsync(base.UploadValuesTaskAsync(address, method, data))
                : Task.FromResult(UploadValues(address, method, data));
        }

        private async Task DoAsync(Task task)
        {
            await ThrowIfRequestTimeout(task);
        }

        private async Task<T> DoAsync<T>(Task<T> task)
        {
            await ThrowIfRequestTimeout(task);
            return await task;
        }

        private async Task ThrowIfRequestTimeout(Task workerTask)
        {
            var request = _currentWebRequest;
            var timeout = AsyncTimeout;

            if (workerTask == await Task.WhenAny(workerTask, Task.Delay(timeout)))
                return;

            const string msgTemplate = "The asynchronous request has timed out ({0}ms) while requesting '{1}'.";

            string msg;
            if (request is DetailedWebRequest)
            {
                var r = (DetailedWebRequest)request;
                msg = string.Format(msgTemplate, timeout, r.RequestUri);
            }
            else if (request is HttpWebRequest)
            {
                var r = (HttpWebRequest)request;
                msg = string.Format(msgTemplate, timeout, r.RequestUri);
            }
            else
            {
                msg = string.Format(msgTemplate, timeout, request.RequestUri);
            }

            // 超时的时候，终止（Abort）请求，并抛出异常。
            // 要小心 Abort 方法可能会抛出异常，若出现这种情况，catch这个异常，并作为要抛出的异常的内部异常。
            try
            {
                request.Abort();
                throw new TimeoutException(msg);
            }
            catch (Exception ex)
            {
                throw new TimeoutException(msg, ex);
            }
        }
    }
}
