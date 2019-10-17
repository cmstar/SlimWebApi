namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 记录<see cref="SlimApiHttpHandler"/>处理的一个请求中的状态。
    /// </summary>
    public class SlimApiRequestState : IRequestState
    {
        /// <summary>
        /// 当前请求指定的方法入口的名称。
        /// </summary>
        public string MethodName;

        /// <summary>
        /// 当前请求指定的回调函数的名称。
        /// 若指定了此名称，则请求是一个JSONP请求。
        /// </summary>
        public string CallbackName;

        /// <summary>
        /// 当前请求指定的请求参数的格式，如 json、get。
        /// </summary>
        public string RequestFormat;

        /// <summary>
        /// 返回结果是否使用类型为 text/plain 的 Content-Type 域。
        /// 若为 false，则根据具体请求给定：JSONP 请求使用 text/javascript；JSON 格式则为 application/json。
        /// </summary>
        public bool UsePlainText;
    }
}
