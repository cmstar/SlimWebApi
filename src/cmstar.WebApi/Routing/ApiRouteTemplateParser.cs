using System;
using System.Text;

#if !NETFX
using Microsoft.AspNetCore.Routing;
#else
using System.Web.Routing;
#endif

namespace cmstar.WebApi.Routing
{
    public static class ApiRouteTemplateParser
    {
        private const char DefaultValueDelimiter1 = ',';
        private const char DefaultValueDelimiter2 = '=';
        private const char ConstraintValueDelimiter = ':';

        /// <summary>
        /// 基于扩展语法解析给定的路由URL模板。
        /// </summary>
        /// <param name="routeUrl">
        /// 包含路由配置的URL。
        /// 使用扩展语法，对于每个路由参数，可使用形如 {placeholder,default:'constraint'} 的语法，
        /// 在注册路由URL的同时，注册约束与默认值。
        /// </param>
        /// <returns>路由注册所需的基本信息。</returns>
        /// <remarks>
        /// 1. 默认值与约束部分的顺序没有要求，可以是 {placeholder,default:'constraint'} ，
        /// 也可以是 {placeholder:'constraint',default} 。
        /// 2. 默认值可以使用逗号如 {placeholder,default} ，也可以使用等号 {placeholder=default} 。
        /// 3. 目前从兼容性考虑，只支持设置一个约束。
        /// </remarks>
        public static ApiRouteConfig ParseRouteTemplate(string routeUrl)
        {
            ArgAssert.NotNull(routeUrl, nameof(routeUrl));

            var len = routeUrl.Length;
            var urlBuilder = new StringBuilder(len);
            var config = new ApiRouteConfig();
            var braced = false;

            for (int i = 0; i < len;)
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
                    case DefaultValueDelimiter1:
                    case DefaultValueDelimiter2:
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
                else if (c == DefaultValueDelimiter1 || c == DefaultValueDelimiter2 || c == ConstraintValueDelimiter)
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

            return new ArgumentException(msg, nameof(routeUrl));
        }

        private struct ApiRouteParam
        {
            public string Name;
            public int Length;
            public string Default;
            public string Constraint;

            public override string ToString()
            {
                return $"{Length} {{{Name},{Default}:{Constraint}}}";
            }
        }

        private struct ApiRouteParamSegment
        {
            public int Index;
            public char Ending;
            public string Value;

            public override string ToString()
            {
                return $"Index:{Index} Ending:{(Ending == '\0' ? "\\0" : Ending.ToString())} Value:{Value}";
            }
        }
    }
}
