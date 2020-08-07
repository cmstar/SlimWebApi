#if !NETFX
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace cmstar.WebApi
{
    /// <summary>
    /// 模拟 .net Framework 版 HttpTaskAsyncHandler ，用于 .net Core 和 .net Framework 间的兼容处理。
    /// </summary>
    public abstract class HttpTaskAsyncHandler
    {
        public abstract Task ProcessRequestAsync(HttpContext context);
    }
}
#endif