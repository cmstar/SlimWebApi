using System;

namespace cmstar.WebApi
{
    /// <summary>
    /// 表示WebAPI的调用客户端发生的错误。
    /// </summary>
    public class ApiClientException : Exception
    {
        /// <summary>
        /// 初始化类型的新示例。
        /// </summary>
        /// <param name="message">描述此异常的消息。</param>
        /// <param name="innerException">指定引起此异常的异常。</param>
        public ApiClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
