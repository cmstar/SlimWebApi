using System.IO;
using System.IO.Compression;
using System.Web;

namespace cmstar.WebApi.Filters
{
    /// <summary>
    /// 使用gZip压缩流作为<see cref="HttpRequest.Filter"/>。
    /// </summary>
    public class GzipCompressionFilter : CompressionFilter
    {
        private readonly Stream _compressionStream;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="underlyingStream">原始的<see cref="HttpRequest.Filter"/>。</param>
        public GzipCompressionFilter(Stream underlyingStream)
            : base(underlyingStream)
        {
            _compressionStream = new GZipStream(underlyingStream, CompressionMode.Compress);
        }

        protected override Stream CompressionStream
        {
            get { return _compressionStream; }
        }
    }
}
