using System;

namespace cmstar.WebApi
{
    /// <summary>
    /// 表示WebAPI调用期间发生的异常。
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// 初始化<see cref="ApiException"/>的新实例。
        /// </summary>
        /// <param name="code">描述此异常的状态码。</param>
        public ApiException(int code)
            : this(code, "There is an error during the API invocation.")
        {
        }

        /// <summary>
        /// 初始化<see cref="ApiException"/>的新实例。
        /// </summary>
        /// <param name="code">描述此异常的状态码。</param>
        /// <param name="message">描述此异常的消息。</param>
        public ApiException(int code, string message)
            : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 初始化<see cref="ApiException"/>的新实例。
        /// </summary>
        /// <param name="code">描述此异常的状态码。</param>
        /// <param name="message">描述此异常的消息。</param>
        /// <param name="innerException">指定引起此异常的异常。</param>
        public ApiException(int code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

        /// <summary>
        /// 获取描述此异常的状态码。
        /// </summary>
        public int Code { get; private set; }

        /// <summary>
        /// 获取此异常的描述，包含状态码和消息。
        /// </summary>
        public override string Message
        {
            get { return string.Concat("Code: ", Code, ", ", base.Message); }
        }

        /// <summary>
        /// 获取描述此异常的消息，不包含状态码。
        /// </summary>
        public string Description
        {
            get { return base.Message; }
        }
    }
}
