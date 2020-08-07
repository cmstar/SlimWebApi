#if !NETFX
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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
        /// </summary>
        public static ApiMethodContext Current => CurrentKeeper?.Api ?? EmptyContext;

        private static Keeper CurrentKeeper
        {
            get => CallContext.LogicalGetData(ApiMethodContextHttpItemName) as Keeper;
            set => CallContext.LogicalSetData(ApiMethodContextHttpItemName, value);
        }

        /// <summary>
        /// 切换当前调用上下文。
        /// </summary>
        internal static void SwitchContext(HttpContext httpContext, ApiMethodContext apiMethodContext)
        {
            CurrentKeeper = new Keeper
            {
                Http = httpContext,
                Api = apiMethodContext
            };
        }

        /// <summary>
        /// 当结束当前API方法的处理过程时，调用此方法以清理当前上下文。
        /// </summary>
        internal static void ExitContext()
        {
            CurrentKeeper = null;
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
        public HttpContext Raw => CurrentKeeper?.Http;

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

        private class Keeper
        {
            public HttpContext Http;
            public ApiMethodContext Api;
        }
    }
}
#endif