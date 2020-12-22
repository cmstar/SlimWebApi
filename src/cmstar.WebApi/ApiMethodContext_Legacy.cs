#if NETFX
using System.Collections.Generic;
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
            EmptyContext = new ApiMethodContext(null, null, null);
        }

        /// <summary>
        /// 获取本次API方法调用所使用的<see cref="ApiMethodContext"/>。
        /// 不会返回 null ，若当前不在 API 方法调用的上下文中，返回一个空白的实例。
        /// </summary>
        public static ApiMethodContext Current
        {
            get
            {
                var httpContext = HttpContext.Current;
                if (httpContext == null)
                    return EmptyContext;

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

        internal ApiMethodContext(ApiMethodInfo apiMethodInfo, object state, IDictionary<string, object> args)
        {
            ApiMethodInfo = apiMethodInfo;
            RequestState = state;
            Arguments = args ?? new Dictionary<string, object>(0);
        }

        /// <summary>
        /// 获取当前API方法所关联的原始<see cref="HttpContext"/>。
        /// 若当前并不处于HTTP请求上下文中，返回<c>null</c>。
        /// </summary>
        public HttpContext Raw => HttpContext.Current;

        /// <summary>
        /// 获取当前被调用的方法的信息。
        /// 若当前并不处于调用上下文中，返回<c>null</c>。
        /// </summary>
        public ApiMethodInfo ApiMethodInfo { get; }

        /// <summary>
        /// 获取用于调用当前方法的参数的值。若方法没有参数，则为一个空集。
        /// </summary>
        public IDictionary<string, object> Arguments { get; }

        /// <summary>
        /// 获取一个实例，该实例用于记录当前处理的请求中的状态。
        /// 若当前并不处于调用上下文中，返回<c>null</c>。
        /// </summary>
        public object RequestState { get; }
    }
}
#endif