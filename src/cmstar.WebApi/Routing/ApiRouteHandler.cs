using System.Web;
using System.Web.Routing;

namespace cmstar.WebApi.Routing
{
    /// <summary>
    /// 用于响应特定的WebAPI路由的<see cref="IRouteHandler"/>实现。
    /// </summary>
    /// <typeparam name="T">指定处理请求的<see cref="IHttpHandler"/>实例的类型。</typeparam>
    public class ApiRouteHandler<T> : IRouteHandler
        where T : IHttpHandler, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var handler = new T();
            return handler;
        }
    }
}
