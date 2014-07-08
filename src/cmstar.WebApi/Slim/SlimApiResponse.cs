namespace cmstar.WebApi.Slim
{
    public class SlimApiResponse<T>
    {
        public SlimApiResponse()
            : this(0, string.Empty)
        {
        }

        public SlimApiResponse(T data)
            : this(0, string.Empty, data)
        {
        }

        public SlimApiResponse(int code, string message, T data = default(T))
        {
            Code = code;
            Data = data;
            Message = message;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
