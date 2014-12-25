namespace cmstar.WebApi
{
    public class ApiResponse<T>
    {
        public ApiResponse()
            : this(0, string.Empty)
        {
        }

        public ApiResponse(T data)
            : this(0, string.Empty, data)
        {
        }

        public ApiResponse(int code, string message, T data = default(T))
        {
            Code = code;
            Data = data;
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse()
            : this(0, string.Empty)
        {
        }

        public ApiResponse(object data)
            : this(0, string.Empty, data)
        {
        }

        public ApiResponse(int code, string message, object data = null)
            : base(code, message, data)
        {
        }
    }
}
