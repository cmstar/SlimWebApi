using System.IO;
using System.IO.Compression;
using System.Web;

namespace cmstar.WebApi.Filters
{
    /// <summary>
    /// 使用defalte压缩流作为<see cref="HttpRequest.Filter"/>。
    /// </summary>
    public class DeflateCompressionFilter : CompressionFilter
    {
        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="underlyingStream">原始的<see cref="HttpRequest.Filter"/>。</param>
        public DeflateCompressionFilter(Stream underlyingStream)
            : base(underlyingStream)
        {
            CompressionStream = new DeflateStream(underlyingStream, CompressionMode.Compress);
        }

        /// <inheritdoc />
        protected override Stream CompressionStream { get; }
    }
}