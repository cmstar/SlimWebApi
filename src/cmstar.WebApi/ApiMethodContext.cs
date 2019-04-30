using System.Web;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含一次API方法调用期间的有关信息。
    /// </summary>
    public class ApiMethodContext
    {
        private const string ApiMethodContextHttpItemName = "CORE_WEBAPI_CONTEXT";

        private static readonly ApiMethodContext EmptyContext;

        static ApiMethodContext()
        {
            EmptyContext = new ApiMethodContext();
        }

        /// <summary>
        /// 获取本次API方法调用所使用的<see cref="ApiMethodContext"/>。
        /// </summary>
        public static ApiMethodContext Current
        {
            get
            {
                var context = HttpContext.Current.Items[ApiMethodContextHttpItemName] as ApiMethodContext;
                return context ?? EmptyContext;
            }
        }

        /// <summary>
        /// 切换当前调用上下文，使切换后的<see cref="HttpContext.Current"/>和<see cref="Current"/>可以正确获取到。
        /// </summary>
        internal static void SwitchContext(HttpContext httpContext, ApiMethodContext apiMethodContext)
        {
            // 将 ApiMethodContext 存储在 HttpContext.Items 里。
            httpContext.Items[ApiMethodContextHttpItemName] = apiMethodContext;

            // 设置 HttpContext.Current 等同于设置 CallContext.HostContext。
            HttpContext.Current = httpContext;
        }

        /// <summary>
        /// 获取当前API方法所关联的原始<see cref="HttpContext"/>。
        /// 若当前并不处于HTTP请求上下文中，返回<c>null</c>。
        /// </summary>
        public HttpContext Raw => HttpContext.Current;
    }
}
