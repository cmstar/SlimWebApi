using System;
using System.Threading;
using System.Threading.Tasks;

namespace cmstar.WebApi
{
    /// <summary>
    /// 提供用于异步API测试的相关方法。
    /// </summary>
    public static class AsyncServiceProvider
    {
        /// <summary>
        /// 这个异步方法在等待过程中应该不消耗任何线程。
        /// </summary>
        /// <param name="ms">等待的毫秒数。</param>
        [ApiMethod("delay")]
        public static async Task<Guid> DelayAsync(int ms)
        {
            await Task.Delay(ms);
            return Guid.NewGuid();
        }

        /// <summary>
        /// 这个方法在阻塞过程中是要消耗一个工作线程的。
        /// </summary>
        /// <param name="ms">等待的毫秒数。</param>
        [ApiMethod("block")]
        public static async Task<Guid> BlockAsync(int ms)
        {
            Thread.Sleep(ms);
            return await DelayAsync(0);
        }

        /// <summary>
        /// 一个抛出异常的异步方法。
        /// </summary>
        [ApiMethod("error")]
        public static async Task ErrorAsync()
        {
            await Task.Delay(10);
            throw new Exception("Some message.");
        }
    }
}