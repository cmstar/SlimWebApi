using System;
using System.Collections.Generic;
using System.Web;
using Common.Logging;

namespace cmstar.WebApi
{
    /// <summary>
    /// <see cref="IHttpHandler"/>的实现，包含了基本的API处理流程。这是一个抽象类。
    /// </summary>
    public abstract class ApiHttpHandlerBase : IHttpHandler
    {
        private static readonly Dictionary<Type, ApiHandlerState> HandlerStates
            = new Dictionary<Type, ApiHandlerState>();

        public void ProcessRequest(HttpContext context)
        {
            var httpResponse = context.Response;

            httpResponse.AddHeader("Pragma", "No-Cache");
            httpResponse.Expires = 0;
            httpResponse.CacheControl = "no-cache";

            var handlerState = GetCurrentTypeHandler();
            var invocationHandler = CreateInvocationHandler(context, handlerState);

            try
            {
                PerformProcessRequest(context, invocationHandler, handlerState);
            }
            catch (Exception ex)
            {
                handlerState.Logger.Fatal("Can not process the request, there must be a bug.", ex);
                throw;
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        /// <summary>
        /// 对WebAPI进行注册和配置。
        /// </summary>
        /// <param name="setup">提供用于Web API注册与配置的方法。</param>
        public abstract void Setup(ApiSetup setup);

        /// <summary>
        /// 获取用于当前API调用的<see cref="IApiInvocationHandler"/>实现。
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/>。</param>
        /// <param name="handlerState">包含API方法注册相关的信息。</param>
        /// <returns><see cref="IApiInvocationHandler"/>实现。</returns>
        protected abstract IApiInvocationHandler CreateInvocationHandler(HttpContext context, ApiHandlerState handlerState);

        /// <summary>
        /// 获取指定的API方法所对应的参数解析器。
        /// </summary>
        /// <param name="method">包含API方法的有关信息。</param>
        /// <returns>key为解析器的名称，value为解析器实例。</returns>
        protected abstract IDictionary<string, IRequestDecoder> ResolveDecoders(ApiMethodInfo method);

        /// <summary>
        /// 处理<see cref="ApiMethodInfo"/>的调用过程中出现的异常。
        /// 重写此方法以定制异常处理逻辑。返回null以忽略此过程。
        /// </summary>
        /// <param name="ex"><see cref="ApiMethodInfo"/>的调用过程中出现的异常。</param>
        /// <param name="error">
        /// false指示之后的处理流程中，此方法返回的<see cref="ApiResponse"/>不再被作为错误信息处理；
        /// 默认值为true，表示后续步骤中仍然继续异常处理流程。
        /// </param>
        /// <returns>
        /// 返回异常处理后的API回执实例。若返回null，则异常处理流程忽略此过程，继续后续步骤。
        /// </returns>
        protected virtual ApiResponse OnMethodError(Exception ex, ref bool error)
        {
            return null;
        }

        /// <summary>
        /// 返回当前API使用的<see cref="ILog"/>实例。null表示使用使用默认的实例。
        /// </summary>
        /// <returns>当前API使用的<see cref="ILog"/>实例。</returns>
        protected virtual ILog GetLogger()
        {
            return null;
        }

        private void PerformProcessRequest(
            HttpContext context, IApiInvocationHandler invocationHandler, ApiHandlerState handlerState)
        {
            var methodInvocationStarted = false;

            try
            {
                var methodName = invocationHandler.GetMethodName();
                var method = handlerState.GetMethod(methodName);
                if (method == null)
                {
                    invocationHandler.OnMethodNotFound(methodName);
                    return;
                }

                var decoderName = invocationHandler.GetDecoderName();
                var decoder = handlerState.GetDecoder(methodName, decoderName);
                if (decoder == null)
                {
                    invocationHandler.OnDecoderNotFound(methodName, decoderName);
                    return;
                }

                var param = invocationHandler.DecodeParam(decoder) ?? new Dictionary<string, object>(0);

                var apiMethodContext = new ApiMethodContext();
                apiMethodContext.Raw = context;
                apiMethodContext.CacheProvider = method.CacheProvider;
                apiMethodContext.CacheExpiration = method.CacheExpiration;
                apiMethodContext.CacheKeyProvider = () => CacheKeyHelper.GetCacheKey(method, param);
                ApiMethodContext.Current = apiMethodContext;

                object result;
                if (method.AutoCacheEnabled)
                {
                    var cacheProvider = method.CacheProvider;
                    var cacheKey = CacheKeyHelper.GetCacheKey(method, param);
                    result = cacheProvider.Get(cacheKey);

                    if (result == null)
                    {
                        methodInvocationStarted = true;
                        result = method.Invoke(param);
                        cacheProvider.Add(cacheKey, result, method.CacheExpiration);
                    }
                }
                else
                {
                    methodInvocationStarted = true;
                    result = method.Invoke(param);
                }

                var apiResponse = new ApiResponse(result);
                invocationHandler.OnSuccess(apiResponse);
            }
            catch (Exception ex)
            {
                if (methodInvocationStarted)
                {
                    var error = true;
                    var apiResponse = OnMethodError(ex, ref error);
                    invocationHandler.OnHandledError(apiResponse, ex, error);
                }
                else
                {
                    invocationHandler.OnUnhandledError(ex);
                }
            }
        }

        private ApiHandlerState GetCurrentTypeHandler()
        {
            var type = GetType();

            ApiHandlerState state;
            if (HandlerStates.TryGetValue(type, out state))
                return state;

            lock (HandlerStates)
            {
                if (HandlerStates.TryGetValue(type, out state))
                    return state;

                var setup = new ApiSetup(type);
                Setup(setup);

                var logger = GetLogger() ?? LogManager.GetLogger(type);
                state = new ApiHandlerState(logger);

                foreach (var apiMethodInfo in setup.ApiMethodInfos)
                {
                    var methodName = apiMethodInfo.MethodName;

                    // 由于函数可能有重载，名称是一样的，这里自动对方法名称进行改名
                    for (var i = 2; state.GetMethod(methodName) != null; i++)
                    {
                        methodName = apiMethodInfo.MethodName + i;
                    }

                    state.AddMethod(methodName, apiMethodInfo);

                    var decoderMap = ResolveDecoders(apiMethodInfo);
                    if (decoderMap != null)
                    {
                        foreach (var decoder in decoderMap)
                        {
                            state.AddDecoder(methodName, decoder.Key, decoder.Value);
                        }
                    }
                }

                HandlerStates.Add(type, state);
                return state;
            }
        }
    }
}
