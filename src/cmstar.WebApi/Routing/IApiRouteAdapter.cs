#if NETFX
using System.Web;
#endif

namespace cmstar.WebApi.Routing
{
    /// <summary>
    /// 定义用于URL路由注册的抽象方法，以提供跨平台（.net Framework/Core/Standard）的兼容性。
    /// </summary>
    public interface IApiRouteAdapter
    {
        /// <summary>
        /// 定义一个路由规则。
        /// </summary>
        /// <typeparam name="T">指定提供API的类型，派生自<see cref="ApiHttpHandlerBase"/>。</typeparam>
        /// <param name="routeUrl">
        /// 包含路由配置的URL。
        /// 使用扩展语法，对于每个路由参数，可使用形如 {placeholder=default} 的语法，
        /// 详见<see cref="ApiRouteTemplateParser.ParseRouteParam"/>
        /// </param>
        IApiRouteAdapter MapApiRoute<T>(string routeUrl)
#if NETFX
            where T : IHttpHandler, new();
#else
            where T : HttpTaskAsyncHandler, new();
#endif
    }
}