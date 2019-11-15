using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace cmstar.WebApi.Routing
{
    [TestFixture]
    public class ApiRouteTemplateParserTests
    {
        [Test]
        public void TestParseRouteUrl()
        {
            Check("");
            Check("constant/part2");
            Check("{value}");
            Check("{");
            Check("}");
            Check("{{{");
            Check("}}}");
            Check("head/{controller}/{action}/");
            Check("head/{{{{{controller}}}/-{action}-tail");
            Check("{p1}/{p2}/{p3}");

            // with defaults
            Check("{value,default}", "{value}", "value=default");
            Check("{value,quoted' default value '}", "{value}", "value=quoted default value ");
            Check("{value=with space}", "{value}", "value=with space");
            Check("head{a=a}/{b,b}/{c=c}tail", "head{a}/{b}/{c}tail", "a=a&b=b&c=c");
            Check("{value='with brace{}'}", "{value}", "value=with brace{}");
            Check("{value,'It''s spatan!'}", "{value}", "value=It's spatan!");
            Check("{value,''''}", "{value}", "value='");
            Check("{value,','}", "{value}", "value=,");

            // with constaints
            Check(@"{value:\d+}", "{value}", expectedConstraints: @"value=\d+");
            Check("{value:longlonglong}", "{value}", expectedConstraints: "value=longlonglong");
            Check("{value:with space}", "{value}", expectedConstraints: "value=with space");
            Check("head{a:a}/{b:b}/{c:c}tail", "head{a}/{b}/{c}tail", expectedConstraints: "a=a&b=b&c=c");
            Check("{value:out' in'}", "{value}", expectedConstraints: "value=out in");

            // mix
            Check(@"domain/{{{product:[a-z]+,pencil}/{id,133:\d+}}}",
                "domain/{{{product}/{id}}}",
                "id=133&product=pencil",
                @"id=\d+&product=[a-z]+");
        }

        private void Check(string routeUrl,
            string expectedUrl = null,
            string expectedDefaults = null,
            string expectedConstraints = null)
        {
            Console.WriteLine("Testing: " + routeUrl);
            var res = ApiRouteTemplateParser.ParseRouteTemplate(routeUrl);

            if (expectedUrl == null)
            {
                Assert.AreEqual(res.Url, routeUrl, "URL");
            }
            else
            {
                Assert.AreEqual(res.Url, expectedUrl, "URL");
            }

            Assert.AreEqual(expectedDefaults, FormatDictionary(res.Defaults), "Defaults");
            Assert.AreEqual(expectedConstraints, FormatDictionary(res.Constraints), "Constraints");
        }

        private string FormatDictionary(IDictionary<string, object> dic)
        {
            if (dic == null)
                return null;

            var sb = new StringBuilder();

            foreach (var p in dic.OrderBy(x => x.Key))
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }

                sb.Append(p.Key).Append('=').Append(p.Value);
            }

            return sb.ToString();
        }
    }
}