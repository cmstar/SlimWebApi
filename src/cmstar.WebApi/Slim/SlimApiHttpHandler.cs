using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using cmstar.Serialization.Json;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 提供Slim WebAPI的入口。继承此类以实现API的注册和使用。
    /// 这是一个抽象类。
    /// </summary>
    public abstract class SlimApiHttpHandler : ApiHttpHandlerBase
    {
        /// <summary>
        /// 指定被调用的方法的名称的参数。
        /// </summary>
        public static string MetaParamMethodName = "~method";

        /// <summary>
        /// 指定数据的序列化方式的参数。
        /// </summary>
        public static string MetaParamFormat = "~format";

        /// <summary>
        /// 指定回执的JSONP回调方法名称的参数。
        /// </summary>
        public static string MetaParamCallback = "~callback";

        /// <summary>
        /// 指定使用JSON请求。
        /// </summary>
        public const string MetaRequestFormatJson = "json";

        /// <summary>
        /// 指定使用GET方法请求。
        /// </summary>
        public const string MetaRequestFormatPost = "post";

        /// <summary>
        /// 指定使用POST方法请求。
        /// </summary>
        public const string MetaRequestFormatGet = "get";

        /// <summary>
        /// 指定回执的MIME类型为text/plain。
        /// </summary>
        public const string MetaResponseFormatPlain = "plain";

        private const int OutputBodyLimitLower = 1024;
        private const int OutputBodyLimitUpper = 65536;

        /// <summary>
        /// 获取请求方的IP地址。
        /// </summary>
        /// <param name="request">当前请求对应的<see cref="HttpRequest"/>实例。</param>
        /// <returns>请求方的IP地址。</returns>
        protected virtual string GetUserHostAddress(HttpRequest request)
        {
            return request.UserHostAddress;
        }

        /// <summary>
        /// 获取指定的API方法所对应的参数解析器。
        /// </summary>
        /// <param name="method">包含API方法的有关信息。</param>
        /// <returns>key为解析器的名称，value为解析器实例。</returns>
        protected override IDictionary<string, IRequestDecoder> ResolveDecoders(ApiMethodInfo method)
        {
            var param = method.Method.GetParameters();
            var decoderMap = new Dictionary<string, IRequestDecoder>(4);

            if (param.Length == 0)
            {
                decoderMap[MetaRequestFormatGet]
                    = decoderMap[MetaRequestFormatPost]
                        = decoderMap[MetaRequestFormatJson]
                            = decoderMap[string.Empty]
                                = EmptyParamMethodRequestDecoder.Instance;

                return decoderMap;
            }

            var paramInfoMap = method.ParamInfoMap;
            var methodParamStat = TypeHelper.GetMethodParamStat(method.Method);
            if (!methodParamStat.HasCoplexMember)
            {
                var inlineParamHttpParamDecoder = new InlineParamHttpParamDecoder(paramInfoMap);
                decoderMap[MetaRequestFormatGet] = inlineParamHttpParamDecoder;

                if (!methodParamStat.HasFileInput)
                {
                    decoderMap[MetaRequestFormatPost] = inlineParamHttpParamDecoder;
                    decoderMap[MetaRequestFormatJson] = new InlineParamJsonDecoder(paramInfoMap);
                }
            }
            else if (param.Length == 1)
            {
                var paramType = param[0].ParameterType;
                var paramTypeStat = TypeHelper.GetTypeMemberStat(paramType);

                if (paramTypeStat.HasFileInput)
                {
                    if (paramTypeStat.HasCoplexMember)
                    {
                        throw NoSupportedDecoderError(method.Method);
                    }

                    decoderMap[MetaRequestFormatGet] = new SingleObjectHttpParamDecoder(paramInfoMap);
                }
                else
                {
                    if (!paramTypeStat.HasCoplexMember)
                    {
                        var singleObjectHttpParamDecoder = new SingleObjectHttpParamDecoder(paramInfoMap);
                        decoderMap[MetaRequestFormatGet] = singleObjectHttpParamDecoder;
                        decoderMap[MetaRequestFormatPost] = singleObjectHttpParamDecoder;
                    }

                    decoderMap[MetaRequestFormatJson] = new SingleObjectJsonDecoder(paramInfoMap);
                }
            }
            else // param.Length > 1 || methodParamStat.HasCoplexMember
            {
                if (methodParamStat.HasFileInput)
                {
                    throw NoSupportedDecoderError(method.Method);
                }

                decoderMap[MetaRequestFormatJson] = new InlineParamJsonDecoder(paramInfoMap);
            }

            // 设置默认的Decoder，优先级 get->json->post
            if (decoderMap.ContainsKey(MetaRequestFormatGet))
            {
                decoderMap[string.Empty] = decoderMap[MetaRequestFormatGet];
            }
            else if (decoderMap.ContainsKey(MetaRequestFormatJson))
            {
                decoderMap[string.Empty] = decoderMap[MetaRequestFormatJson];
            }
            else if (decoderMap.ContainsKey(MetaRequestFormatPost))
            {
                decoderMap[string.Empty] = decoderMap[MetaRequestFormatPost];
            }

            return decoderMap;
        }

        /// <summary>
        /// 创建用于保存当前API请求信息的对象实例。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="handlerState"><see cref="ApiHandlerState"/>的实例。</param>
        /// <returns>用于保存当前API请求信息的对象实例。</returns>
        protected override object CreateRequestState(HttpContext context, ApiHandlerState handlerState)
        {
            var requestState = new SlimApiRequestState();
            var request = context.Request;

            // 元参数可以多种方式体现（中括号内内容为可选）：
            // 形式1：http://domain/entry?~method=METHOD[&~format=FORMAT][&~callback=CALLBACK]
            // 形式2：http://domain/entry?METHOD[.FORMAT][(CALLBACK)]
            // 形式3使用路由：http://domain/entry/{~method}/[{~format}/][{~callback}/]
            // 优先级自上而下

            // 形式1
            var method = request.ExplicicParam(MetaParamMethodName);
            var callback = request.ExplicicParam(MetaParamCallback);
            var format = request.ExplicicParam(MetaParamFormat);

            // 形式2
            if (method == null)
            {
                var mixedMetaParams = request.QueryString[null];
                if (mixedMetaParams != null)
                {
                    ParseMixedMetaParams(mixedMetaParams, out method, ref format, ref callback);
                }
            }

            // 形式3
            if (RouteData != null)
            {
                if (method == null)
                    method = RouteData.Param(MetaParamMethodName);

                if (callback == null)
                    callback = RouteData.Param(MetaParamCallback);

                if (format == null)
                    format = RouteData.Param(MetaParamFormat);
            }

            if (string.IsNullOrEmpty(format))
            {
                // 没有通过参数直接指定格式的情况下，尝试从Content-Type判断
                switch (request.ContentType)
                {
                    case "application/json":
                        requestState.RequestFormat = MetaRequestFormatJson;
                        break;

                    case "application/x-www-form-urlencoded":
                        requestState.RequestFormat = MetaRequestFormatPost;
                        break;
                }
            }
            else
            {
                // format参数可包含多段使用逗号隔开的值（e.g. json,plain）
                var formatOptions = format.Split(TypeHelper.CollectionElementSpliter);
                var ignoreCaseComparer = StringComparer.OrdinalIgnoreCase;

                foreach (var formatOption in formatOptions)
                {
                    if (ignoreCaseComparer.Equals(formatOption, MetaResponseFormatPlain))
                    {
                        requestState.UsePlainText = true;
                    }
                    else
                    {
                        requestState.RequestFormat = formatOption;
                    }
                }
            }

            requestState.MethodName = method;
            requestState.CallbackName = callback;

            return requestState;
        }

        /// <summary>
        /// 获取当前API请求所管理的方法名称。
        /// 若未能获取到名称，返回null。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <returns>方法名称。</returns>
        protected override string RetriveRequestMethodName(HttpContext context, object requestState)
        {
            return ((SlimApiRequestState)requestState).MethodName;
        }

        /// <summary>
        /// 获取当前调用的API方法所使用的参数解析器的名称。
        /// 若未能获取到名称，返回null。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <returns>调用的API方法所使用的参数解析器的名称。</returns>
        protected override string RetrieveRequestDecoderName(HttpContext context, object requestState)
        {
            return ((SlimApiRequestState)requestState).RequestFormat;
        }

        /// <summary>
        /// 创建当前调用的API方法所需要的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="decoder">用于API参数解析的<see cref="IRequestDecoder"/>实例。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        protected override IDictionary<string, object> DecodeParam(
            HttpContext context, object requestState, IRequestDecoder decoder)
        {
            try
            {
                return decoder.DecodeParam(context.Request, null);
            }
            catch (Exception ex)
            {
                if (ex is JsonContractException || ex is JsonFormatException || ex is InvalidCastException)
                    throw new ApiException(Code400, "Bad request.", ex);

                throw;
            }
        }

        /// <summary>
        /// 将指定的<see cref="ApiResponse"/>序列化并写如HTTP输出流中。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="apiResponse">用于表示API返回的数据。</param>
        protected override void WriteResponse(
            HttpContext context, object requestState, ApiResponse apiResponse)
        {
            var slimApiRequestState = (SlimApiRequestState)requestState;
            var httpResponse = context.Response;
            var isJsonp = !String.IsNullOrEmpty(slimApiRequestState.CallbackName);

            if (slimApiRequestState.UsePlainText)
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
                httpResponse.Write(slimApiRequestState.CallbackName);
                httpResponse.Write("(");
            }

            var responseJson = JsonHelper.Serialize(apiResponse);
            httpResponse.Write(responseJson);

            if (isJsonp)
            {
                httpResponse.Write(")");
            }
        }

        /// <summary>
        /// 获取当前请求的描述信息。
        /// </summary>
        /// <param name="context">当前请求的<see cref="HttpContext"/>实例。</param>
        /// <param name="requestState">用于保存当前API请求信息的对象实例。</param>
        /// <param name="apiResponse">用于表示API返回的数据。</param>
        /// <returns>描述信息。</returns>
        protected override LogMessage GetRequestDescription(
            HttpContext context, object requestState, ApiResponse apiResponse)
        {
            var request = context.Request;
            var logMessage = new LogMessage();
            logMessage.SetProperty("Ip", GetUserHostAddress(request));
            logMessage.SetProperty("Url", request.RawUrl);

            if (request.Files.Count == 0)
            {
                var bodyLength = request.InputStream.Length;
                if (bodyLength > 0)
                {
                    logMessage.SetProperty("Length", bodyLength.ToString());

                    // 输出body部分按照下述逻辑判定：
                    // 1 1K以下的数据直接输出；
                    // 2 64K以上的数据不输出；
                    // 3 之间的数据校验是否是文本，若为文本则输出；
                    bool canOutputBody;
                    if (bodyLength < OutputBodyLimitLower)
                    {
                        canOutputBody = true;
                    }
                    else if (bodyLength >= OutputBodyLimitUpper)
                    {
                        canOutputBody = false;
                    }
                    else
                    {
                        canOutputBody = IsTextRequestBody(request.InputStream);
                    }

                    if (canOutputBody)
                    {
                        var body = ReadRequestBody(request.InputStream);
                        logMessage.SetProperty("Body", body);
                    }
                }
            }
            else
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    logMessage.SetProperty("File", file.FileName);
                    logMessage.SetProperty("ContentType", file.ContentType);
                    logMessage.SetProperty("Length", file.ContentLength.ToString());
                }
            }

            if (apiResponse != null)
            {
                var messageBuilder = new StringBuilder();

                if (apiResponse.Code != 0)
                {
                    messageBuilder.Append('(').Append(apiResponse.Code).Append(')');
                }

                if (!string.IsNullOrEmpty(apiResponse.Message))
                {
                    if (messageBuilder.Length > 0)
                    {
                        messageBuilder.Append(' ');
                    }
                    messageBuilder.Append(apiResponse.Message);
                }

                logMessage.Message = messageBuilder.ToString();
            }

            return logMessage;
        }

        private Exception NoSupportedDecoderError(MethodInfo method)
        {
            var msg = string.Format(
                "Can not resolve decoder for method {0} in type {1}.",
                method.Name, method.DeclaringType);
            return new NotSupportedException(msg);
        }

        private bool IsTextRequestBody(Stream inputStream)
        {
            // 重读InputStream
            inputStream.Position = 0;

            // 使用检测\0\0的方式校验，实际上在ASCII和utf8下文本中一个\0都不会有；
            // 但utf16可能会存在一个\0，从兼容性考虑这里校验两个更为保险
            var buffer = new byte[128];
            int len;
            while ((len = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                bool preZero = false;
                for (int i = 0; i < len; i++)
                {
                    if (buffer[i] == '\0')
                    {
                        if (preZero)
                            return false;

                        preZero = true;
                    }
                    else
                    {
                        preZero = false;
                    }
                }
            }

            return true;
        }

        private string ReadRequestBody(Stream inputStream)
        {
            // 重读InputStream
            inputStream.Position = 0;
            var streamReader = new StreamReader(inputStream);
            var body = streamReader.ReadToEnd();

            return body;
        }

        private void ParseMixedMetaParams(string input, out string method, ref string format, ref string callback)
        {
            // METHOD.FORMAT(CALLBACK)
            const int followedByFomat = 1;
            const int followedByCallback = 2;

            var inputLength = input.Length;
            var position = 0;
            var followedBy = 0;

            while (position < inputLength)
            {
                var c = input[position];

                if (c == '.') // hit the beginning of the format name
                {
                    followedBy = followedByFomat;
                    break;
                }

                if (c == '(') // hit the beginning of the callback name
                {
                    followedBy = followedByCallback;
                    break;
                }

                if (++position == inputLength)
                {
                    method = input;
                    return;
                }
            }

            method = input.Substring(0, position);

            if (followedBy == followedByFomat)
            {
                position++; // move to the next char after .
                var formatStartIndex = position;
                var paramLength = 0;
                while (position < inputLength)
                {
                    var c = input[position];

                    if (c == '(') // hit the beginning of the callback name
                    {
                        followedBy = followedByCallback;
                        break;
                    }

                    paramLength++;
                    position++;
                }

                if (format == null)
                {
                    format = input.Substring(formatStartIndex, paramLength);
                }
            }

            if (followedBy == followedByCallback)
            {
                position++; // move to the next char after (

                var callbackStartIndex = position;
                var paramLength = 0;
                while (true)
                {
                    var c = input[position];

                    if (c == ')') // hit the end of the callback name
                        break;

                    paramLength++;
                    position++;

                    // if there's not a ')', means no callback name specified
                    if (position == inputLength)
                        return;
                }

                if (callback == null)
                {
                    callback = input.Substring(callbackStartIndex, paramLength);
                }
            }
        }

        /// <summary>
        /// 保存请求处理过程中的数据。
        /// </summary>
        protected class SlimApiRequestState
        {
            public string MethodName;
            public string CallbackName;
            public string RequestFormat;
            public bool UsePlainText;
        }
    }
}
