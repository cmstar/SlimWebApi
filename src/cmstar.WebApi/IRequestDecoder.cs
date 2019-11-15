using System.Collections.Generic;

#if NETCORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace cmstar.WebApi
{
    /// <summary>
    /// 定义从<see cref="HttpRequest"/>解析API调用参数信息的方法。
    /// </summary>
    public interface IRequestDecoder
    {
        /// <summary>
        /// 解析<see cref="HttpRequest"/>并创建该请求所对应要调用方法的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="request">HTTP请求。</param>
        /// <param name="state">包含用于参数解析的有关数据。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        IDictionary<string, object> DecodeParam(HttpRequest request, object state);
    }
}
