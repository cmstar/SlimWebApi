namespace cmstar.WebApi
{
    /// <summary>
    /// 表示Web API回执数据的压缩方式。
    /// </summary>
    public enum ApiCompressionMethods
    {
        /// <summary>
        /// 不进行压缩。
        /// </summary>
        None,

        /// <summary>
        /// 根据客户端请求决定压缩方式。
        /// </summary>
        Auto,

        /// <summary>
        /// 使用gZip格式压缩。
        /// </summary>
        GZip,

        /// <summary>
        /// 使用defalte方式压缩。
        /// </summary>
        Defalte
    }
}
