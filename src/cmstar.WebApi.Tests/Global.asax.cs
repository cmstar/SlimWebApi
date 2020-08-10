using System;
using System.Web;
using System.Web.Routing;
using cmstar.WebApi.Routing;

namespace cmstar.WebApi
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // 注册路由，通过 IApiRouteAdapter 中转。
            RegisterRoutes(RouteTable.Routes.CreateApiRouteAdapter());
        }

        private void RegisterRoutes(IApiRouteAdapter routes)
        {
            routes.MapApiRoute<DelegateExample>("routed/slim/{~method,GetGuid}/");
        }
    }
}