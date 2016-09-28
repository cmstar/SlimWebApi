using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using cmstar.WebApi.Filters;
using Common.Logging;

namespace cmstar.WebApi
{
    /// <summary>
    /// <see cref="IHttpHandler"/>的实现，包含了基本的API处理流程。这是一个抽象类。
    /// </summary>
    public abstract class ApiHttpHandlerBase : IHttpHandler
    {
        /// <summary>
        /// 400错误的异常码。
        /// </summary>
        public const int Code400 = 400;

        /// <summary>
        /// 500错误的异常码。
        /// </summary>
        public const int Code500 = 500;

        private static readonly ConcurrentDictionary<Type, ApiHandlerState> HandlerStates
            = new ConcurrentDictionary<Type, ApiHandlerState>();

        private ILog _log;

        public void ProcessRequest(HttpContext context)
        {
            var httpResponse = context.Response;

            httpResponse.AddHeader("Pragma", "No-Cache");
            httpResponse.Expires = 0;
            httpResponse.CacheControl = "no-cache";

            var handlerState = GetCurrentTypeHandler();
            TryInitHandlerState(handlerState);

            var requestState = CreateRequestState(context, handlerState);
            LogSetup = handlerState.LogSetup;

            try
            {
                PerformProcessRequest(context, handlerState, requestState);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Can not process the request.", ex);
                throw;
            }
        }

        public virtual bool IsReusable
        {
            get { return false; }
        }

        /// <summary>
        /// 获取或设置当前处理的请求上下文中所使用的路由信息。
        /// 若为null则对于当前请求的数据解析过程不使用路由信息。
        /// </summary>
        public RouteData RouteData { get; set; }

        /// <summary>
        /// 获取日志相关的配置信息。
        /// </summary>
        protected LogSetup LogSetup { get; private set; }

        /// <summary>
        /// 在每个API方法执行前触发此事件。
        /// 回调参数包扩：
        /// 被调用的API方法的信息；
        /// 传递给被调用方法的参数。
        /// 此事件回调早于<see cref="ApiMethodSetting.MethodInvoking"/>。
        /// </summary>
        public event Action<ApiMethodInfo, IDictionary<string, object>> MethodInvoking
        {
            add { GetCurrentTypeHandler().MethodInvoking += value; }
            remove { GetCurrentTypeHandler().MethodInvoking -= value; }
        }

        /// <summary>
        /// 在每个API方法执行后触发此事件。
        /// 回调参数包扩：
        /// 被调用的API方法的信息；
        /// 传递给被调用方法的参数；
        /// 调用的方法的返回值。若方法没有返回值，或调用过程中出现异常，为null；
        /// 调用的方法所抛出的异常，若无异常，则为null。
        /// 此事件回调晚于<see cref="ApiMethodSetting.MethodInvoked"/>。
        /// </summary>
        public event Action<ApiMethodInfo, IDictionary<string, object>, object, Exception> MethodInvoked
        {
            add { GetCurrentTypeHandler().MethodInvoked += value; }
            remove { GetCurrentTypeHandler().MethodInvoked -= value; }
        }

        /// <summary>
        /// 对WebAPI进行注册和配置。
        /// </summary>
        /// <param name="setup">提供用于Web API注册与配置的方法。</param>
        public abstract void Setup(ApiSetup setup);

        /// <summary>
        /// 在<see cref="Setup"/>方法执行之前，在同一个<see cref="ApiSetup"/>对象上执行此方法。
        /// </summary>
        /// <param name="setup"><see cref="ApiSetup"/>。</param>
        protected virtual void PreSetup(ApiSetup setup) { }

        /// <summary>
        /// 在<see cref="Setup"/>方法执行之后，在同一个<see cref="ApiSetup"/>对象上执行此方法。
        /// </summary>
        /// <param name="setup"><see cref="ApiSetup"/>。</param>
        protected virtual void PostSetup(ApiSetup setup) { }

        /// <summary>
        /// 获取指定的API方法所对应的参数解析器。
        /// </summary>
        /// <param name="method">包含API方法的有关信息。</param>
        /// <returns>key为解析器的名称，value为解析器实例。</returns>
        protected abstract IDictionary<string, IRequestDecoder> ResolveDecoders(ApiMethodInfo method);

        /// <summary>
        /// 创建用于保存当前API请求信息的对象实例。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="handlerState"><see cref="ApiHandlerState"/>的实例。</param>
        /// <returns>用于保存当前API请求信息的对象实例。</returns>
        protected abstract object CreateRequestState(
            HttpContext context, ApiHandlerState handlerState);

        /// <summary>
        /// 获取当前API请求所管理的方法名称。
        /// 若未能获取到名称，返回null。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <returns>方法名称。</returns>
        protected abstract string RetriveRequestMethodName(
            HttpContext context, object requestState);

        /// <summary>
        /// 获取当前调用的API方法所使用的参数解析器的名称。
        /// 若未能获取到名称，返回null。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <returns>调用的API方法所使用的参数解析器的名称。</returns>
        protected abstract string RetrieveRequestDecoderName(
            HttpContext context, object requestState);

        /// <summary>
        /// 创建当前调用的API方法所需要的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="decoder">用于API参数解析的<see cref="IRequestDecoder"/>实例。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        protected abstract IDictionary<string, object> DecodeParam(
            HttpContext context, object requestState, IRequestDecoder decoder);

        /// <summary>
        /// 将指定的<see cref="ApiResponse"/>序列化并写如HTTP输出流中。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="apiResponse">用于表示API返回的数据。</param>
        protected abstract void WriteResponse(
            HttpContext context, object requestState, ApiResponse apiResponse);

        /// <summary>
        /// 获取当前请求的描述信息。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="apiResponse">用于表示API返回的数据。</param>
        /// <returns>描述信息。</returns>
        protected abstract string GetRequestDescription(
            HttpContext context, object requestState, ApiResponse apiResponse);

        /// <summary>
        /// 当成功处理API方法调用后触发此方法。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="apiResponse">用于表示API返回的数据。</param>
        protected virtual void OnSuccess(HttpContext context, object requestState, ApiResponse apiResponse)
        {
            WriteResponse(context, requestState, apiResponse);
            WriteLog(LogSetup.SuccessLogLevel, () => GetRequestDescription(context, requestState, apiResponse));
        }

        /// <summary>
        /// 当调用过程中出现未处理异常时触发此方法。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="exception">异常的实例。</param>
        protected virtual void OnError(HttpContext context, object requestState, Exception exception)
        {
            var apiException = exception as ApiException;
            var apiResponse = apiException == null
                ? new ApiResponse(Code500, "Internal error.")
                : new ApiResponse(apiException.Code, apiException.Description);

            WriteResponse(context, requestState, apiResponse);

            var logLevel = apiResponse.Code == Code400 ? LogSetup.Code400LogLevel : LogLevel.Error;
            WriteLog(logLevel, () => GetRequestDescription(context, requestState, apiResponse), exception);
        }

        /// <summary>
        /// 当本次API访问中未指定访问的方法名称或名称错误时触发此方法。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        protected virtual void OnMethodNotFound(HttpContext context, object requestState)
        {
            const string msg = "Bad entry.";

            var apiResponse = new ApiResponse(Code400, msg);
            WriteResponse(context, requestState, apiResponse);
            WriteLog(LogSetup.Code400LogLevel, () => GetRequestDescription(context, requestState, apiResponse));
        }

        /// <summary>
        /// 当本次API访问中指定访问的方法所关联的参数解析器名称不存在时触发此方法。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        protected virtual void OnDecoderNotFound(HttpContext context, object requestState)
        {
            const string msg = "Unsupported format.";

            var apiResponse = new ApiResponse(Code400, msg);
            WriteResponse(context, requestState, apiResponse);
            WriteLog(LogSetup.Code400LogLevel, () => GetRequestDescription(context, requestState, apiResponse));
        }

        /// <summary>
        /// 处理<see cref="ApiMethodInfo"/>的调用过程中出现的异常。
        /// </summary>
        /// <param name="ex"><see cref="ApiMethodInfo"/>的调用过程中出现的异常。</param>
        /// <returns>
        /// 返回一个<see cref="ApiResponse"/>实例以表示请求处理成功，后续进入<see cref="OnSuccess"/>方法；
        /// 返回null则继续异常处理流程，进入<see cref="OnError"/>方法。
        /// </returns>
        protected virtual ApiResponse TranslateMethodInvocationError(Exception ex)
        {
            return null;
        }

        /// <summary>
        /// 获取当前API使用的<see cref="ILog"/>实例。
        /// </summary>
        /// <returns>当前API使用的<see cref="ILog"/>实例。</returns>
        protected ILog Logger
        {
            get
            {
                if (_log == null)
                {
                    _log = LogSetup == null || LogSetup.LoggerName == null
                        ? LogManager.GetLogger(GetType())
                        : LogManager.GetLogger(LogSetup.LoggerName);
                }

                return _log;
            }
        }

        /// <summary>
        /// 写日志。
        /// </summary>
        /// <param name="logLevel">
        /// 指定日志的级别。
        /// <see cref="LogLevel.All"/>将作为<see cref="LogLevel.Info"/>处理。
        /// </param>
        /// <param name="msg">信息。</param>
        /// <param name="ex">异常。</param>
        protected void WriteLog(LogLevel logLevel, object msg, Exception ex = null)
        {
            if (logLevel == LogLevel.Info || logLevel == LogLevel.All)
            {
                Logger.Info(msg, ex);
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Debug:
                    Logger.Debug(msg, ex);
                    break;

                case LogLevel.Warn:
                    Logger.Warn(msg, ex);
                    break;

                case LogLevel.Error:
                    Logger.Error(msg, ex);
                    break;

                case LogLevel.Fatal:
                    Logger.Fatal(msg, ex);
                    break;

                case LogLevel.Trace:
                    Logger.Trace(msg, ex);
                    break;
            }
        }

        /// <summary>
        /// 写日志。
        /// </summary>
        /// <param name="logLevel">
        /// 指定日志的级别。
        /// <see cref="LogLevel.All"/>将作为<see cref="LogLevel.Info"/>处理。
        /// </param>
        /// <param name="getMsgCallback">获取日志信息的方法。</param>
        /// <param name="ex">异常。</param>
        protected void WriteLog(LogLevel logLevel, Func<string> getMsgCallback, Exception ex = null)
        {
            string msg;
            if (logLevel == LogLevel.Info || logLevel == LogLevel.All)
            {
                if (!Logger.IsInfoEnabled)
                    return;

                msg = getMsgCallback();
                Logger.Info(msg, ex);
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Debug:
                    if (!Logger.IsDebugEnabled)
                        return;

                    msg = getMsgCallback();
                    Logger.Debug(msg, ex);
                    break;

                case LogLevel.Warn:
                    if (!Logger.IsWarnEnabled)
                        return;

                    msg = getMsgCallback();
                    Logger.Warn(msg, ex);
                    break;

                case LogLevel.Error:
                    if (!Logger.IsErrorEnabled)
                        return;

                    msg = getMsgCallback();
                    Logger.Error(msg, ex);
                    break;

                case LogLevel.Fatal:
                    if (!Logger.IsFatalEnabled)
                        return;

                    msg = getMsgCallback();
                    Logger.Fatal(msg, ex);
                    break;

                case LogLevel.Trace:
                    if (!Logger.IsTraceEnabled)
                        return;

                    msg = getMsgCallback();
                    Logger.Trace(msg, ex);
                    break;
            }
        }

        private void PerformProcessRequest(
            HttpContext context, ApiHandlerState handlerState, object requestState)
        {
            var methodInvocationStarted = false;

            try
            {
                var methodName = RetriveRequestMethodName(context, requestState);
                var method = handlerState.GetMethod(methodName);
                if (method == null)
                {
                    OnMethodNotFound(context, requestState);
                    return;
                }

                var decoderName = RetrieveRequestDecoderName(context, requestState);
                var decoder = handlerState.GetDecoder(methodName, decoderName);
                if (decoder == null)
                {
                    OnDecoderNotFound(context, requestState);
                    return;
                }

                var param = DecodeParam(context, requestState, decoder) ?? new Dictionary<string, object>(0);

                ApiMethodContext.Current = new ApiMethodContext
                {
                    Raw = context,
                    CacheProvider = method.Setting.CacheProvider,
                    CacheExpiration = method.Setting.CacheExpiration,
                    CacheKeyProvider = () => CacheKeyHelper.GetCacheKey(method, param)
                };

                object result;
                if (method.Setting.AutoCacheEnabled)
                {
                    var cacheProvider = method.Setting.CacheProvider;
                    var cacheKey = CacheKeyHelper.GetCacheKey(method, param);
                    result = cacheKey == null ? null : cacheProvider.Get(cacheKey);

                    if (result == null)
                    {
                        methodInvocationStarted = true;
                        result = MethodInvoke(handlerState, method, param);
                        cacheProvider.Add(cacheKey, result, method.Setting.CacheExpiration);
                    }
                }
                else
                {
                    methodInvocationStarted = true;
                    result = MethodInvoke(handlerState, method, param);
                }

                // 按需使用压缩流传输
                AppendCompressionFilter(context, method);

                var apiResponse = new ApiResponse(result);
                OnSuccess(context, requestState, apiResponse);
            }
            catch (Exception ex)
            {
                var apiResponse = methodInvocationStarted ? TranslateMethodInvocationError(ex) : null;
                if (apiResponse == null)
                {
                    OnError(context, requestState, ex);
                }
                else
                {
                    OnSuccess(context, requestState, apiResponse);
                }
            }
        }

        private object MethodInvoke(
            ApiHandlerState handlerState, ApiMethodInfo apiMethod, IDictionary<string, object> param)
        {
            try
            {
                handlerState.OnMethodInvoking(apiMethod, param);

                if (apiMethod.Setting.MethodInvoking != null)
                {
                    apiMethod.Setting.MethodInvoking(apiMethod, param);
                }

                var result = apiMethod.Invoke(param);

                if (apiMethod.Setting.MethodInvoked != null)
                {
                    apiMethod.Setting.MethodInvoked(apiMethod, param, result, null);
                }

                handlerState.OnMethodInvoked(apiMethod, param, result, null);
                return result;
            }
            catch (Exception ex)
            {
                if (apiMethod.Setting.MethodInvoked != null)
                {
                    apiMethod.Setting.MethodInvoked(apiMethod, param, null, ex);
                }

                handlerState.OnMethodInvoked(apiMethod, param, null, ex);
                throw;
            }
        }

        private void AppendCompressionFilter(HttpContext context, ApiMethodInfo method)
        {
            var compressionMethod = ParseCompressionMethods(context.Request, method.Setting.CompressionMethods);

            string contentEncoding;
            Stream filter;

            switch (compressionMethod)
            {
                case ApiCompressionMethods.Defalte:
                    contentEncoding = "deflate";
                    filter = new DeflateCompressionFilter(context.Response.Filter);
                    break;

                case ApiCompressionMethods.GZip:
                    contentEncoding = "gzip";
                    filter = new GzipCompressionFilter(context.Response.Filter);
                    break;

                default:
                    return;
            }

            context.Response.Headers["Content-Encoding"] = contentEncoding;
            context.Response.Filter = filter;
        }

        private ApiCompressionMethods ParseCompressionMethods(HttpRequest request, ApiCompressionMethods compressionMethod)
        {
            if (compressionMethod != ApiCompressionMethods.Auto)
                return compressionMethod;

            var header = request.Headers["Accept-Encoding"];
            if (header == null)
                return ApiCompressionMethods.None;

            // 由于deflate比gzip稍快一些，体积也较小，优先使用deflate
            if (header.Contains("deflate"))
                return ApiCompressionMethods.Defalte;

            if (header.Contains("gzip"))
                return ApiCompressionMethods.GZip;

            return ApiCompressionMethods.None;
        }

        private ApiHandlerState GetCurrentTypeHandler()
        {
            var type = GetType();
            var handlerState = HandlerStates.GetOrAdd(type, x => new ApiHandlerState());
            return handlerState;
        }

        private void TryInitHandlerState(ApiHandlerState handlerState)
        {
            if (handlerState.Initialized)
                return;

            lock (handlerState)
            {
                // double-lock check
                if (handlerState.Initialized)
                    return;

                InitHandlerState(handlerState);
                handlerState.Initialized = true;
            }
        }

        private void InitHandlerState(ApiHandlerState handlerState)
        {
            var setup = new ApiSetup(GetType());

            PreSetup(setup);
            Setup(setup);
            PostSetup(setup);

            handlerState.LogSetup = (LogSetup)setup.Log.Clone();

            foreach (var apiMethodInfo in setup.ApiMethodInfos)
            {
                var methodName = apiMethodInfo.Setting.MethodName;

                // 由于函数可能有重载，名称是一样的，这里自动对方法名称进行改名
                for (var i = 2; handlerState.GetMethod(methodName) != null; i++)
                {
                    methodName = apiMethodInfo.Setting.MethodName + i;
                }

                handlerState.AddMethod(methodName, apiMethodInfo);

                var decoderMap = ResolveDecoders(apiMethodInfo);
                if (decoderMap != null)
                {
                    foreach (var decoder in decoderMap)
                    {
                        handlerState.AddDecoder(methodName, decoder.Key, decoder.Value);
                    }
                }
            }
        }
    }
}
