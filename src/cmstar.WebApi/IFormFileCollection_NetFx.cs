using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace cmstar.WebApi
{
    /*
     * 模拟 .net Core 的 IFormFile 和 IFormFileCollection 接口。
     *
     * IFormFile 的 ContentDisposition 和 Headers 属性目前找不到对应的 API ，得用 MVC 框架才有，
     * 这里就省略了它们。
     */

    /// <summary>
    /// Represents the collection of files sent with the HttpRequest.
    /// </summary>
    public interface IFormFileCollection : IReadOnlyList<IFormFile>
    {
        IFormFile this[string name] { get; }

        IFormFile GetFile(string name);

        IReadOnlyList<IFormFile> GetFiles(string name);
    }

    /// <summary>Represents a file sent with the HttpRequest.</summary>
    public interface IFormFile
    {
        /// <summary>
        /// Gets the raw Content-Type header of the uploaded file.
        /// </summary>
        string ContentType { get; }

        /// <summary>Gets the file length in bytes.</summary>
        long Length { get; }

        /// <summary>
        /// Gets the form field name from the Content-Disposition header.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the file name from the Content-Disposition header.
        /// </summary>
        string FileName { get; }

        // 这两个属性在 .net Framework 版的原生方法里找不到对应的。
        /*
        /// <summary>
        /// Gets the raw Content-Disposition header of the uploaded file.
        /// </summary>
        string ContentDisposition { get; }

        /// <summary>Gets the header dictionary of the uploaded file.</summary>
        IHeaderDictionary Headers { get; }
        */

        /// <summary>
        /// Opens the request stream for reading the uploaded file.
        /// </summary>
        Stream OpenReadStream();

        /// <summary>
        /// Copies the contents of the uploaded file to the <paramref name="target" /> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        void CopyTo(Stream target);

        /// <summary>
        /// Asynchronously copies the contents of the uploaded file to the <paramref name="target" /> stream.
        /// </summary>
        /// <param name="target">The stream to copy the file contents to.</param>
        /// <param name="cancellationToken"></param>
        Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// 将<see cref="HttpFileCollection"/>适配到<see cref="FormFileCollectionImpl"/>。
    /// </summary>
    public class FormFileCollectionImpl : IFormFileCollection
    {
        private readonly HttpFileCollection _files;

        public FormFileCollectionImpl(HttpFileCollection files)
        {
            _files = files;
        }

        /// <inheritdoc />
        public IEnumerator<IFormFile> GetEnumerator()
        {
            var keys = _files.AllKeys;

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];
                var file = _files[key];
                var formFile = new FormFileImpl(key, file);
                yield return formFile;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => _files.Count;

        /// <inheritdoc />
        public IFormFile this[int index]
        {
            get
            {
                var file = _files.Get(index);
                var key = _files.AllKeys[index];
                return new FormFileImpl(key, file);
            }
        }

        /// <inheritdoc />
        public IFormFile this[string name] => GetFile(name);

        /// <inheritdoc />
        public IFormFile GetFile(string name)
        {
            var file = _files.Get(name);
            return new FormFileImpl(name, file);
        }

        /// <inheritdoc />
        public IReadOnlyList<IFormFile> GetFiles(string name)
        {
            return new[] { GetFile(name) };
        }
    }

    /// <summary>
    /// 将<see cref="HttpPostedFile"/>适配到<see cref="IFormFile"/>。
    /// </summary>
    public class FormFileImpl : IFormFile
    {
        private readonly HttpPostedFile _file;

        public FormFileImpl(string name, HttpPostedFile file)
        {
            Name = name;
            _file = file;
        }

        /// <inheritdoc />
        public string ContentType => _file.ContentType;

        /// <inheritdoc />
        public long Length => _file.ContentLength;

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public string FileName => _file.FileName;

        /// <inheritdoc />
        public Stream OpenReadStream()
        {
            return _file.InputStream;
        }

        /// <inheritdoc />
        public void CopyTo(Stream target)
        {
            _file.InputStream.CopyTo(target);
        }

        /// <inheritdoc />
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buf = GetCopyBufferSize(_file.InputStream);
            return _file.InputStream.CopyToAsync(target, buf, cancellationToken);
        }

        private static int GetCopyBufferSize(Stream s)
        {
            var num = 81920;
            if (!s.CanSeek)
                return num;

            var length = s.Length;
            var position = s.Position;
            if (length <= position)
            {
                num = 1;
            }
            else
            {
                long val2 = length - position;
                if (val2 > 0L)
                {
                    num = (int)Math.Min(num, val2);
                }
            }
            return num;
        }
    }
}
