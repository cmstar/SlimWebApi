using System;
using System.Collections.Generic;

namespace cmstar.WebApi
{
    /// <summary>
    /// 定义了一次API调用处理流程中的各部分行为。
    /// </summary>
    public interface IApiInvocationHandler
    {
        /// <summary>
        /// 获取当前调用的API方法的名称。
        /// 若未能获取到名称，返回null。
        /// </summary>
        /// <returns>调用的API方法的名称。</returns>
        string GetMethodName();

        /// <summary>
        /// 获取当前调用的API方法所使用的参数解析器的名称。
        /// 若未能获取到名称，返回null。
        /// </summary>
        /// <returns>调用的API方法所使用的参数解析器的名称。</returns>
        string GetDecoderName();

        /// <summary>
        /// 创建该请求当前调用的API方法调用所需要的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="decoder">用于API参数解析的<see cref="IRequestDecoder"/>实例。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        IDictionary<string, object> DecodeParam(IRequestDecoder decoder);

        /// <summary>
        /// 当在没有异常的情况下获得API回执则触发此方法。
        /// </summary>
        /// <param name="response">用于表示API返回的数据。</param>
        void OnSuccess(ApiResponse response);

        /// <summary>
        /// 当发生了未处理异常时触发此方法。
        /// </summary>
        /// <param name="exception">异常的实例。</param>
        void OnUnhandledError(Exception exception);

        /// <summary>
        /// 当发生了异常，且异常被处理后触发此方法。
        /// </summary>
        /// <param name="response">表示API返回的数据。</param>
        /// <param name="rawException">原始异常。</param>
        /// <param name="error">
        /// false指示之后的处理流程中，此方法返回的<see cref="ApiResponse"/>不再被作为错误信息处理；
        /// 默认值为true，表示后续步骤中仍然继续异常处理流程。
        /// </param>
        void OnHandledError(ApiResponse response, Exception rawException, bool error);

        /// <summary>
        /// 当本次API访问中未指定访问的方法名称或名称错误时触发此方法。
        /// </summary>
        /// <param name="methodName">方法的名称。</param>
        void OnMethodNotFound(string methodName);

        /// <summary>
        /// 当本次API访问中指定访问的方法所关联的参数解析器名称不存在时触发此方法。。
        /// </summary>
        /// <param name="methodName">方法的名称。</param>
        /// <param name="decoderName">参数解析器的名称。</param>
        void OnDecoderNotFound(string methodName, string decoderName);
    }
}
