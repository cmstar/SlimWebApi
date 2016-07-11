using System;
using System.Text;
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
        private const char DefaultValueDelimiter = ',';
        private const char ConstraintValueDelimiter = ':';

        /// <summary>
        /// 定义一个路由规则。
        /// </summary>
        /// <typeparam name="T">用于处理请求的<see cref="IHttpHandler"/>。</typeparam>
        /// <param name="routes">路由表。</param>
        /// <param name="routeUrl">
        /// 包含路由配置的URL。
        /// 使用扩展语法，对于每个路由参数，可使用形如 {placeholder:'constraint',default} 的语法，
        /// 在注册路由URL的同时，注册约束与默认值。
        /// </param>
        public static void MapApiRoute<T>(this RouteCollection routes, string routeUrl)
            where T : IHttpHandler, new()
        {
            ArgAssert.NotNull(routes, "routes");

            var conf = ParseRouteUrl(routeUrl);
            routes.Add(new Route(conf.Url, conf.Defaults, conf.Constraints, new ApiRouteHandler<T>()));
        }

        /// <summary>
        /// 基于扩展语法解析给定的路由URL。
        /// </summary>
        /// <param name="routeUrl">
        /// 包含路由配置的URL。
        /// 使用扩展语法，对于每个路由参数，可使用形如 {placeholder,default:'constraint'} 的语法，
        /// 在注册路由URL的同时，注册约束与默认值。
        /// </param>
        /// <returns>路由注册所需的基本信息。</returns>
        public static ApiRouteConfig ParseRouteUrl(string routeUrl)
        {
            ArgAssert.NotNull(routeUrl, "routeUrl");

            var len = routeUrl.Length;
            var urlBuilder = new StringBuilder(len);
            var config = new ApiRouteConfig();
            var braced = false;

            for (int i = 0; i < len; )
            {
                var c = routeUrl[i];

                if (c == '{')
                {
                    braced = !braced;

                    urlBuilder.Append(c);
                    i++;
                    continue;
                }

                if (!braced)
                {
                    urlBuilder.Append(c);
                    i++;
                    continue;
                }

                var routeParam = ParseRouteParam(routeUrl, i);
                urlBuilder.Append(routeParam.Name);

                if (routeParam.Default != null)
                {
                    if (config.Defaults == null)
                    {
                        config.Defaults = new RouteValueDictionary();
                    }

                    config.Defaults[routeParam.Name] = routeParam.Default;
                }

                if (routeParam.Constraint != null)
                {
                    if (config.Constraints == null)
                    {
                        config.Constraints = new RouteValueDictionary();
                    }

                    config.Constraints[routeParam.Name] = routeParam.Constraint;
                }

                i += routeParam.Length;
                braced = false;
            }

            config.Url = urlBuilder.ToString();
            return config;
        }

        private static ApiRouteParam ParseRouteParam(string routeUrl, int startIndex)
        {
            var arg = new ApiRouteParam();
            var len = routeUrl.Length;

            // NAME
            var seg = ReadSegment(routeUrl, startIndex);
            if (seg.Value == null)
                return arg;

            arg.Name = seg.Value;

            // DEFAULT/CONSTRAINT
            while (seg.Index != len)
            {
                var startWith = seg.Ending;

                seg = ReadSegment(routeUrl, seg.Index);
                if (seg.Value == null)
                    break;

                switch (startWith)
                {
                    case DefaultValueDelimiter:
                        // there can be only one default value
                        if (arg.Default != null)
                            throw ConfigError(routeUrl);

                        arg.Default = seg.Value;
                        break;

                    case ConstraintValueDelimiter:
                        // there can be only one constraint
                        if (arg.Constraint != null)
                            throw ConfigError(routeUrl);

                        arg.Constraint = seg.Value;
                        break;
                }
            }

            arg.Length = seg.Index - startIndex;
            return arg;
        }

        private static ApiRouteParamSegment ReadSegment(string value, int startIndex)
        {
            var len = value.Length;
            var quoted = false;
            var builder = (StringBuilder)null;
            var seg = new ApiRouteParamSegment();
            var index = startIndex;

            while (index < len)
            {
                var c = value[index];
                if (c == '}' && !quoted)
                    break;

                index++;

                if (quoted)
                {
                    // check if the current character is escaped single quote
                    if (c != '\'')
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder();
                        }

                        builder.Append(c);
                    }
                    else if (index < len && value[index] == '\'') // escaped
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder();
                        }

                        builder.Append(c);
                        index++;
                    }
                    else
                    {
                        quoted = false;
                    }
                }
                else if (c == '\'')
                {
                    quoted = true;
                }
                else if (c == DefaultValueDelimiter || c == ConstraintValueDelimiter)
                {
                    seg.Ending = c;
                    break;
                }
                else
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder();
                    }

                    builder.Append(c);
                }
            }

            seg.Index = index;
            seg.Value = builder == null ? null : builder.ToString();
            return seg;
        }

        private static ArgumentException ConfigError(string routeUrl)
        {
            var msg = new StringBuilder()
                .Append("The route parameter configuration {").Append(routeUrl).Append("} is not valid, ")
                .Append("the configuration cannot contain more than one constraint or default value.")
                .ToString();
            return new ArgumentException(msg, "routeUrl");
        }

        private struct ApiRouteParam
        {
            public string Name;
            public int Length;
            public string Default;
            public string Constraint;

            public override string ToString()
            {
                return string.Format("{0} {{{1},{2}:{3}}}", Length, Name, Default, Constraint);
            }
        }

        private struct ApiRouteParamSegment
        {
            public int Index;
            public char Ending;
            public string Value;

            public override string ToString()
            {
                return string.Format("Index:{0} Ending:{1} Value:{2}",
                    Index, Ending == '\0' ? "\\0" : Ending.ToString(), Value);
            }
        }
    }
}
