namespace cmstar.WebApi
{
    /// <summary>
    /// 用于记录一个WebAPI方法的返回结果。
    /// </summary>
    /// <typeparam name="T">WebAPI方法的返回值的类型。</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 创建<see cref="ApiResponse{T}"/>的新实例。
        /// 此实例表示一次无异常的WebAPI方法调用，且该方法没有返回值。
        /// </summary>
        public ApiResponse()
            : this(0, string.Empty)
        {
        }

        /// <summary>
        /// 创建<see cref="ApiResponse{T}"/>的新实例。
        /// 此实例表示一次无异常的WebAPI方法调用，并指定了该方法的返回值。
        /// </summary>
        public ApiResponse(T data)
            : this(0, string.Empty, data)
        {
        }

        /// <summary>
        /// 创建<see cref="ApiResponse{T}"/>的新实例。
        /// </summary>
        public ApiResponse(int code, string message, T data = default(T))
        {
            Code = code;
            Data = data;
            Message = message;
        }

        /// <summary>
        /// 一个状态码，用于表示WebAPI方法的调用过程是否有异常。
        /// 0表示无异常；其他值为异常。非0时的具体值可根据使用场景定义。
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 附加信息。通常可在WebAPI方法的调用过程出现错误时，提供更详细的描述信息。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 当WebAPI方法调用无异常时，存储其返回值。否则为返回值类型的默认值。
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 用于记录一个WebAPI方法的返回结果。
    /// 这是<see cref="ApiResponse{T}"/>的弱类型版本。
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// 创建<see cref="ApiResponse"/>的新实例。
        /// </summary>
        public ApiResponse()
            : this(0, string.Empty)
        {
        }

        /// <summary>
        /// 创建<see cref="ApiResponse"/>的新实例。
        /// 此实例表示一次无异常的WebAPI方法调用，并指定了该方法的返回值。
        /// </summary>
        public ApiResponse(object data)
            : this(0, string.Empty, data)
        {
        }

        /// <summary>
        /// 创建<see cref="ApiResponse"/>的新实例。
        /// </summary>
        public ApiResponse(int code, string message, object data = null)
            : base(code, message, data)
        {
        }
    }
}
