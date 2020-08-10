#if !NETFX
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace cmstar.WebApi.Routing
{
    /// <summary>
    /// 提供WebAPI的URL路由模式注册的相关方法。
    /// 这些方法支持以更简洁的语法进行模式注册。
    /// </summary>
    public static class ApiRouteExtensions
    {
        /// <summary>
        /// 定义一个路由规则。
        /// </summary>
        /// <typeparam name="T">指定提供API的类型，派生自<see cref="ApiHttpHandlerBase"/>。</typeparam>
        /// <param name="app"><see cref="IApplicationBuilder"/>的实例。</param>
        /// <param name="routeUrl">
        /// 包含路由配置的URL。
        /// 使用扩展语法，对于每个路由参数，可使用形如 {placeholder=default} 的语法，
        /// 详见<see cref="ApiRouteTemplateParser.ParseRouteParam"/>
        /// </param>
        public static IApplicationBuilder MapApiRoute<T>(this IApplicationBuilder app, string routeUrl)
            where T : HttpTaskAsyncHandler, new()
        {
            app.UseRouter(routeBuilder =>
            {
                var routeConfig = ApiRouteTemplateParser.ParseRouteTemplate(routeUrl);
                var constraintResolver = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
                var apiHandler = new T();
                var routeHandler = new RouteHandler(apiHandler.ProcessRequestAsync);
                var route = new Route(
                    routeHandler,
                    routeConfig.Url,
                    routeConfig.Defaults,
                    routeConfig.Constraints,
                    null,
                    constraintResolver);

                routeBuilder.Routes.Add(route);
            });

            return app;
        }

        /// <summary>
        /// 创建一个用于注册路由规则的<see cref="IApiRouteAdapter"/>，以提供跨平台（.net Framework/Core/Standard）的兼容性。
        /// </summary>
        public static IApiRouteAdapter CreateApiRouteAdapter(this IApplicationBuilder app)
        {
            return new ApiRouteAdapter(app);
        }

        private class ApiRouteAdapter : IApiRouteAdapter
        {
            private readonly IApplicationBuilder _app;

            public ApiRouteAdapter(IApplicationBuilder app)
            {
                _app = app;
            }

            public IApiRouteAdapter MapApiRoute<T>(string routeUrl)
                where T : HttpTaskAsyncHandler, new()
            {
                _app.MapApiRoute<T>(routeUrl);
                return this;
            }
        }
    }
}
#endif