using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using cmstar.WebApi.Slim.ParamDecoders;
using Common.Logging;
using cmstar.Serialization.Json;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 包含Slim WebAPI中处理HTTP请求的具体流程。
    /// </summary>
    public class SlimApiInvocationHandler
    {
        private readonly Dictionary<string, ApiMethodInfo> _registeredMethods
            = new Dictionary<string, ApiMethodInfo>(ApiEnvironment.DefaultMethodNameComparer);

        private readonly Dictionary<string, DecoderBinding> _decoderMap
            = new Dictionary<string, DecoderBinding>(ApiEnvironment.DefaultMethodNameComparer);

        private readonly string _metaMethodName = SlimApiEnvironment.MetaParamMethodName;
        private readonly string _metaRequestFormat = SlimApiEnvironment.MetaParamFormat;
        private readonly string _metaCallback = SlimApiEnvironment.MetaParamCallback;

        protected readonly ILog Logger;

        /// <summary>
        /// 初始化<see cref="SlimApiInvocationHandler"/>的新实例。
        /// </summary>
        /// <param name="callerType">WebAPI的注册类型。通常是<see cref="SlimApiHttpHandler"/>的子类。</param>
        /// <param name="methods">
        /// API注册信息。可以为<c>null</c>，在之后通过<see cref="AddRegistry"/>方法添加API注册。
        /// </param>
        public SlimApiInvocationHandler(Type callerType, IEnumerable<ApiMethodInfo> methods)
        {
            if (methods != null)
            {
                foreach (var method in methods)
                {
                    AddRegistry(method);
                }
            }

            Logger = LogManager.GetLogger(callerType ?? GetType());
        }

        /// <summary>
        /// 添加一个WebAPI注册。
        /// </summary>
        /// <param name="apiMethodInfo">包含WebAPI的注册信息。</param>
        public void AddRegistry(ApiMethodInfo apiMethodInfo)
        {
            var methodName = apiMethodInfo.MethodName;

            // 由于函数可能有重载，名称是一样的，这里自动对方法名称进行改名
            for (var i = 2; _registeredMethods.ContainsKey(methodName); i++)
            {
                methodName = apiMethodInfo.MethodName + i;
            }

            _registeredMethods.Add(methodName, apiMethodInfo);

            var methodBinding = ResolveDefaultDecoderBinding(apiMethodInfo);
            _decoderMap.Add(methodName, methodBinding);
        }

        /// <summary>
        /// 处理HTTP请求。
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/>。</param>
        public void ProcessRequest(HttpContext context)
        {
            var request = context.Request;

            if (Logger.IsInfoEnabled)
            {
                var requestDescription = GetRequestDescritpion(request, null);
                Logger.Info(requestDescription);
            }

            var state = GetInvocationState(context);

            var apiMethodInfo = ResolveMethodInfo(state);
            if (apiMethodInfo == null)
                return;

            var decoder = ResolveDecoder(state);
            if (decoder == null)
                return;

            var paramValueMap = DecodeParam(state, decoder);
            if (paramValueMap == null)
                return;

            try
            {
                var apiMethodContext = new ApiMethodContext();
                apiMethodContext.Raw = context;
                apiMethodContext.CacheProvider = apiMethodInfo.CacheProvider;
                apiMethodContext.CacheExpiration = apiMethodInfo.CacheExpiration;
                apiMethodContext.CacheKeyProvider = () => CacheKeyHelper.GetCacheKey(apiMethodInfo, paramValueMap);
                ApiMethodContext.Current = apiMethodContext;

                object result;
                if (apiMethodInfo.AutoCacheEnabled)
                {
                    var cacheProvider = apiMethodInfo.CacheProvider;
                    var cacheKey = CacheKeyHelper.GetCacheKey(apiMethodInfo, paramValueMap);
                    result = cacheProvider.Get(cacheKey);

                    if (result == null)
                    {
                        result = apiMethodInfo.Invoke(paramValueMap);
                        cacheProvider.Add(cacheKey, result, apiMethodInfo.CacheExpiration);
                    }
                }
                else
                {
                    result = apiMethodInfo.Invoke(paramValueMap);
                }

                WriteResponse(state, 0, result);
            }
            catch (Exception ex)
            {
                WriteResponse(state, 500, null, "Unhandled exception.", ex);
            }
        }

        private InvocationState GetInvocationState(HttpContext context)
        {
            var request = context.Request;
            var state = new InvocationState();

            state.Context = context;
            state.MethodName = request.ExplicicParam(_metaMethodName);
            state.JsonpMethod = request.ExplicicParam(_metaCallback);

            var format = request.ExplicicParam(_metaRequestFormat);
            if (!string.IsNullOrEmpty(format))
            {
                var formatOptions = format.ToLower().Split(TypeHelper.CollectionElementSpliter);
                foreach (var formatOption in formatOptions)
                {
                    if (formatOption == SlimApiEnvironment.MetaResponseFormatPlain)
                    {
                        state.UsePlainText = true;
                    }
                    else
                    {
                        state.RequestFormat = formatOption;
                    }
                }
            }

            return state;
        }

        private IDictionary<string, object> DecodeParam(InvocationState state, IRequestDecoder decoder)
        {
            try
            {
                var paramValueMap = decoder.DecodeParam(state.Context.Request);
                return paramValueMap ?? new Dictionary<string, object>(0);
            }
            catch (Exception ex)
            {
                var jsonContractException = ex as JsonContractException;
                if (jsonContractException != null)
                {
                    WriteResponse(state, 400, null, "Bad JSON. " + jsonContractException.Message, ex);
                    return null;
                }

                var jsonFormatException = ex as JsonFormatException;
                if (jsonFormatException != null)
                {
                    WriteResponse(state, 400, null, "Bad JSON. " + jsonFormatException.Message, ex);
                    return null;
                }

                if (ex is InvalidCastException)
                {
                    WriteResponse(state, 400, null, "Invalid parameter value.", ex);
                    return null;
                }

                WriteResponse(state, 500, null, "Unhandled exception.", ex);
                return null;
            }
        }

        private ApiMethodInfo ResolveMethodInfo(InvocationState state)
        {
            if (string.IsNullOrEmpty(state.MethodName))
            {
                WriteResponse(state, 400, null, "Method name not specified.");
                return null;
            }

            ApiMethodInfo apiMethodInfo;
            if (!_registeredMethods.TryGetValue(state.MethodName, out apiMethodInfo))
            {
                WriteResponse(state, 400, null, "Method not found.");
                return null;
            }

            return apiMethodInfo;
        }

        private IRequestDecoder ResolveDecoder(InvocationState state)
        {
            var format = state.RequestFormat;
            if (string.IsNullOrEmpty(format))
                return _decoderMap[state.MethodName].DefaultDecoder;

            IRequestDecoder decoder = null;
            switch (format)
            {
                case SlimApiEnvironment.MetaRequestFormatJson:
                    decoder = _decoderMap[state.MethodName].JsonDecoder;
                    break;

                case SlimApiEnvironment.MetaRequestFormatGet:
                case SlimApiEnvironment.MetaRequestFormatPost:
                    decoder = _decoderMap[state.MethodName].HttpParamDecoder;
                    break;
            }

            if (decoder == null)
                WriteResponse(state, 400, null, "The format is not supported on the method.");

            return decoder;
        }

        private void WriteResponse(InvocationState state, int code,
            object responseData, string responseMessage = "", Exception ex = null)
        {
            var httpResponse = state.Context.Response;
            var isJsonp = !string.IsNullOrEmpty(state.JsonpMethod);

            if (state.UsePlainText)
            {
                httpResponse.ContentType = "text/plain";
            }
            else if (isJsonp)
            {
                httpResponse.ContentType = "text/javascript";
            }
            else
            {
                httpResponse.ContentType = "application/json";
            }

            if (isJsonp)
            {
                httpResponse.Write(state.JsonpMethod);
                httpResponse.Write("(");
            }

            var responseObject = new SlimApiResponse<object>(code, responseMessage, responseData);
            var responseJson = JsonHelper.Serialize(responseObject);
            httpResponse.Write(responseJson);

            if (isJsonp)
            {
                httpResponse.Write(")");
            }

            if (code != 0)
            {
                // 目前错误码还有没具体定义，但确定1000以下与HTTP状态码一致
                // 故先用状态码来表示某些类型错误
                if (code >= 400 && code < 500 && Logger.IsWarnEnabled) // 请求错误
                {
                    var requestDescription = GetRequestDescritpion(state.Context.Request, responseJson);
                    Logger.Warn(requestDescription, ex);
                }
                else if (code >= 500 && code < 600 && Logger.IsErrorEnabled) // 服务器错误
                {
                    var requestDescription = GetRequestDescritpion(state.Context.Request, responseJson);
                    Logger.Error(requestDescription, ex);
                }
            }
        }

        private string GetRequestDescritpion(HttpRequest request, string responseText)
        {
            /*
             * 此方法在一次请求开头被调用，之后的错误日志中再次被调用，考虑是否缓存这些信息。
             * 1 错误日志并不总是有，此方法多数情况应该只被调用一次；
             * 2 缓存需要考虑线程安全性，例如使用[ThreadStatic]字段，开销也不小；
             * 鉴于上述原因，每次此方法被调用时，都重新组装描述信息，不进行缓存。
             */
            var sb = new StringBuilder();
            sb.AppendLine(request.UserHostAddress);
            sb.Append("Url: ").Append(request.RawUrl);

            var bodyLength = request.InputStream.Length;
            if (bodyLength > 0)
            {
                sb.AppendLine();
                sb.Append("Length: ").Append(bodyLength);

                var body = ReadRequestBody(request);
                sb.AppendLine();
                sb.Append("Body: ").Append(body);
            }

            if (!string.IsNullOrEmpty(responseText))
            {
                sb.AppendLine();
                sb.Append("Response: ").Append(responseText);
            }

            return sb.ToString();
        }

        private string ReadRequestBody(HttpRequest request)
        {
            // 重读InputStream
            request.InputStream.Position = 0;
            var streamReader = new StreamReader(request.InputStream);
            var body = streamReader.ReadToEnd();

            // 重置Position以供再次读取
            request.InputStream.Position = 0;
            return body;
        }

        private DecoderBinding ResolveDefaultDecoderBinding(ApiMethodInfo apiMethodInfo)
        {
            var binding = new DecoderBinding();
            var param = apiMethodInfo.Method.GetParameters();

            if (param.Length == 0)
            {
                binding.HttpParamDecoder = EmptyParamMethodRequestDecoder.Instance;
                binding.JsonDecoder = EmptyParamMethodRequestDecoder.Instance;
                binding.DefaultDecoder = EmptyParamMethodRequestDecoder.Instance;
                return binding;
            }

            var paramInfoMap = apiMethodInfo.ParamInfoMap;

            if (TypeHelper.IsPlainMethod(apiMethodInfo.Method, true))
            {
                binding.HttpParamDecoder = new InlineParamHttpParamDecoder(paramInfoMap);
                binding.JsonDecoder = new InlineParamJsonDecoder(paramInfoMap);
            }
            else if (param.Length == 1)
            {
                binding.JsonDecoder = new SingleObjectJsonDecoder(paramInfoMap);

                var paramType = param[0].ParameterType;
                if (TypeHelper.IsPlainType(paramType, true))
                {
                    binding.HttpParamDecoder = new SingleObjectHttpParamDecoder(paramInfoMap);
                }
            }
            else
            {
                binding.JsonDecoder = new InlineParamJsonDecoder(paramInfoMap);
            }

            // 总是优先使用HttpParamDecoder作为默认的Decoder
            binding.DefaultDecoder = binding.HttpParamDecoder ?? binding.JsonDecoder;

            return binding;
        }

        private class DecoderBinding
        {
            public IRequestDecoder HttpParamDecoder;
            public IRequestDecoder JsonDecoder;
            public IRequestDecoder DefaultDecoder;
        }

        private class InvocationState
        {
            public HttpContext Context;

            public string MethodName;

            public string RequestFormat;

            public bool UsePlainText;

            public string JsonpMethod;
        }
    }
}
