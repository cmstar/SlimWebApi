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
            RegisterRoutes(RouteTable.Routes);
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            routes.MapApiRoute<DelegateExample>("routed/slim/{~method,GetGuid}/");
        }
    }
}