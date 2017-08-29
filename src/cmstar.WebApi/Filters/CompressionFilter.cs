using System;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace cmstar.WebApi.Filters
{
    /// <summary>
    /// 提供gZip或deflate压缩流作为<see cref="HttpRequest.Filter"/>时的支持。
    /// </summary>
    /// <remarks>
    /// <see cref="GZipStream"/>与<see cref="DeflateStream"/>的Flush方法不按我们预期的方式运作，
    /// 直到将其流关闭为止，才能保证压缩流将所有数据写入压缩数据中而不丢失数据，因此提供此类型，
    /// 不对压缩流做Flush操作。
    /// ref
    /// http://stackoverflow.com/questions/3653250/gzipstream-is-cutting-off-last-part-of-xml
    /// </remarks>
    public abstract class CompressionFilter : Stream
    {
        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="underlyingStream">原始的<see cref="HttpRequest.Filter"/>。</param>
        protected CompressionFilter(Stream underlyingStream)
        {
            UnderlyingStream = underlyingStream;
        }

        /// <summary>
        /// 获取压缩流。
        /// </summary>
        protected abstract Stream CompressionStream { get; }

        /// <summary>
        /// 获取原始的<see cref="HttpRequest.Filter"/>。
        /// </summary>
        protected Stream UnderlyingStream { get; }

        /// <inheritdoc />
        public override bool CanRead { get; } = false;

        /// <inheritdoc />
        public override bool CanSeek { get; } = false;

        /// <inheritdoc />
        public override bool CanWrite { get; } = true;

        /// <inheritdoc />
        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public override void Flush()
        {
            // 直接Flush压缩流可能导致部分数据丢失，故不对压缩流做Flush
            UnderlyingStream.Flush();
        }
        
        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            CompressionStream.Write(buffer, offset, count);
        }
        
        /// <inheritdoc />
        public override void WriteByte(byte value)
        {
            CompressionStream.WriteByte(value);
        }

        /// <inheritdoc />
        public override void Close()
        {
            CompressionStream.Close();
            base.Close();
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CompressionStream.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
