using System.Collections.Generic;
using System.Web;

namespace cmstar.WebApi
{
    /// <summary>
    /// 总是返回0个参数值的<see cref="IRequestDecoder"/>实现。
    /// </summary>
    public class EmptyParamMethodRequestDecoder : IRequestDecoder
    {
        /// <summary>
        /// <see cref="EmptyParamMethodRequestDecoder"/>的唯一实例。
        /// </summary>
        public static readonly EmptyParamMethodRequestDecoder Instance = new EmptyParamMethodRequestDecoder();

        private EmptyParamMethodRequestDecoder() { }

        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            return new Dictionary<string, object>(0);
        }
    }
}