﻿using System;
using System.Collections.Generic;
using System.IO;
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
        protected static string MetaParamMethodName = "~method";
        protected static string MetaParamFormat = "~format";
        protected static string MetaParamCallback = "~callback";

        protected const string MetaRequestFormatJson = "json";
        protected const string MetaRequestFormatPost = "post";
        protected const string MetaRequestFormatGet = "get";
        protected const string MetaResponseFormatPlain = "plain";

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
                    = decoderMap[String.Empty]
                    = EmptyParamMethodRequestDecoder.Instance;

                return decoderMap;
            }

            var paramInfoMap = method.ParamInfoMap;

            if (TypeHelper.IsPlainMethod(method.Method, true))
            {
                decoderMap[MetaRequestFormatGet]
                    = decoderMap[MetaRequestFormatPost]
                    = new InlineParamHttpParamDecoder(paramInfoMap);

                decoderMap[MetaRequestFormatJson]
                    = new InlineParamJsonDecoder(paramInfoMap);
            }
            else if (param.Length == 1)
            {
                decoderMap[MetaRequestFormatJson]
                    = new SingleObjectJsonDecoder(paramInfoMap);

                var paramType = param[0].ParameterType;
                if (TypeHelper.IsPlainType(paramType, true))
                {
                    decoderMap[MetaRequestFormatGet]
                        = decoderMap[MetaRequestFormatPost]
                        = new SingleObjectHttpParamDecoder(paramInfoMap);
                }
            }
            else
            {
                decoderMap[MetaRequestFormatJson]
                    = new InlineParamJsonDecoder(paramInfoMap);
            }

            // 优先使用HttpParamDecoder作为默认的Decoder
            decoderMap[String.Empty] =
                decoderMap.ContainsKey(MetaRequestFormatGet)
                ? decoderMap[MetaRequestFormatGet]
                : decoderMap[MetaRequestFormatJson];

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

            // 元参数可以两种方式体现（中括号内内容为可选）：
            // 形式1：http://domain/entry?~method=METHOD[&~format=FORMAT][&~callback=CALLBACK]
            // 形式2：http://domain/entry?METHOD[.FORMAT][(CALLBACK)]
            // 形式2优先级高于形式1

            string method = null;
            string callback = null;
            string format = null;

            // 先从形式2中获取
            var mixedMetaParams = request.QueryString[null];
            if (mixedMetaParams != null)
            {
                ParseMixedMetaParams(mixedMetaParams, out method, out format, out callback);
            }

            // 未从形式2中获取的参数再从形式1中获取
            if (string.IsNullOrEmpty(method))
                method = request.ExplicicParam(MetaParamMethodName);

            if (string.IsNullOrEmpty(callback))
                callback = request.ExplicicParam(MetaParamCallback);

            if (string.IsNullOrEmpty(format))
                format = request.ExplicicParam(MetaParamFormat);

            if (!string.IsNullOrEmpty(format))
            {
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
                var jsonContractException = ex as JsonContractException;
                if (jsonContractException != null)
                    throw new ApiException(400, "Bad JSON. " + jsonContractException.Message, ex);

                var jsonFormatException = ex as JsonFormatException;
                if (jsonFormatException != null)
                    throw new ApiException(400, "Bad JSON. " + jsonFormatException.Message, ex);

                if (ex is InvalidCastException)
                    throw new ApiException(400, "Invalid parameter value.", ex);

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
        protected override string GetRequestDescription(
            HttpContext context, object requestState, ApiResponse apiResponse)
        {
            var request = context.Request;
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

            if (apiResponse != null)
            {
                if (apiResponse.Code != 0)
                {
                    sb.AppendLine();
                    sb.Append("Code: ").Append(apiResponse.Code);
                }

                if (!string.IsNullOrEmpty(apiResponse.Message))
                {
                    sb.AppendLine();
                    sb.Append("Message: ").Append(apiResponse.Message);
                }
            }

            return sb.ToString();
        }

        private string ReadRequestBody(HttpRequest request)
        {
            // 重读InputStream
            request.InputStream.Position = 0;
            var streamReader = new StreamReader(request.InputStream);
            var body = streamReader.ReadToEnd();

            return body;
        }

        private void ParseMixedMetaParams(string input, out string method, out string format, out string callback)
        {
            // METHOD.FORMAT(CALLBACK)
            format = callback = null;

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

                format = input.Substring(formatStartIndex, paramLength);
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

                callback = input.Substring(callbackStartIndex, paramLength);
            }
        }

        private class SlimApiRequestState
        {
            public string MethodName;
            public string CallbackName;
            public string RequestFormat;
            public bool UsePlainText;
        }
    }
}
