using System.Web.Routing;

namespace cmstar.WebApi.Routing
{
    /// <summary>
    /// 包含WebAPI路由注册所需的基本信息。
    /// </summary>
    public class ApiRouteConfig
    {
        /// <summary>
        /// URL。
        /// </summary>
        public string Url;

        /// <summary>
        /// 当未在URL中给定相关参数时，参数所使用的默认值。
        /// </summary>
        public RouteValueDictionary Defaults;

        /// <summary>
        /// 包含用于匹配各参数的正则表达式。
        /// </summary>
        public RouteValueDictionary Constraints;
    }
}