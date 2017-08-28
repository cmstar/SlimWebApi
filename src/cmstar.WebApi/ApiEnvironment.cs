using System.Net;

namespace cmstar.WebApi
{
    /// <summary>
    /// 包含WebAPI调用上下文所需的一些属性与方法。
    /// </summary>
    public static class ApiEnvironment
    {
        /// <summary>
        /// 用于<see cref="ApiResponse{T}.Code"/>，表示客户端请求不能被识别。
        /// 值同<see cref="HttpStatusCode.BadRequest"/>（400）。
        /// </summary>
        public static int CodeBadRequest { get; } = (int)HttpStatusCode.BadRequest;

        /// <summary>
        /// 用于<see cref="ApiResponse{T}.Code"/>，表示服务端内部异常。
        /// 值同<see cref="HttpStatusCode.InternalServerError"/>（500）。
        /// </summary>
        public static int CodeInternalError { get; } = (int)HttpStatusCode.InternalServerError;
    }
}
