#if !NETCORE
using System.Web;
using System.Web.Routing;

namespace cmstar.WebApi.Routing
{
    /// <summary>
    /// 提供WebAPI的URL路由模式注册的相关方法。
    /// 这些方法支持以更简洁的语法进行模式注册。
    /// </summary>
    public static class ApiRouteMapping
    {
        /// <summary>
        /// 定义一个路由规则。
        /// </summary>
        /// <typeparam name="T">用于处理请求的<see cref="IHttpHandler"/>。</typeparam>
        /// <param name="routes">路由表。</param>
        /// <param name="routeUrl">
        /// 包含路由配置的URL。
        /// 使用扩展语法，对于每个路由参数，可使用形如 {placeholder=default} 的语法，
        /// 详见<see cref="ApiRouteTemplateParser.ParseRouteParam"/>
        /// </param>
        public static void MapApiRoute<T>(this RouteCollection routes, string routeUrl)
            where T : IHttpHandler, new()
        {
            ArgAssert.NotNull(routes, nameof(routes));

            var conf = ApiRouteTemplateParser.ParseRouteTemplate(routeUrl);
            routes.Add(new Route(conf.Url, conf.Defaults, conf.Constraints, new ApiRouteHandler<T>()));
        }

        private class ApiRouteHandler<T> : IRouteHandler
            where T : IHttpHandler, new()
        {
            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                var handler = new T();
                if (handler is ApiHttpHandlerBase apiHttpHandler)
                {
                    apiHttpHandler.RouteData = requestContext.RouteData;
                }
                return handler;
            }
        }
    }
}
#endif