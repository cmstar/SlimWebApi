using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using cmstar.Serialization.Json;
using Common.Logging;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 对应Slim WebAPI的<see cref="IApiInvocationHandler"/>实现。
    /// </summary>
    public class SlimApiInvocationHandler : IApiInvocationHandler
    {
        private readonly HttpContext _context;
        private readonly string _methodName;
        private readonly string _callbackName;
        private readonly string _requestFormat;
        private readonly bool _usePlainText;
        private readonly ILog _logger;

        /// <summary>
        /// 初始化类型的新实例。
        /// </summary>
        /// <param name="context">包含本次请求的上下文。</param>
        /// <param name="logger">指定请求上下文所使用的日志。</param>
        public SlimApiInvocationHandler(HttpContext context, ILog logger)
        {
            _context = context;
            _logger = logger;

            var request = context.Request;
            _methodName = request.ExplicicParam(SlimApiEnvironment.MetaParamMethodName);
            _callbackName = request.ExplicicParam(SlimApiEnvironment.MetaParamCallback);

            var format = request.ExplicicParam(SlimApiEnvironment.MetaParamFormat);
            if (!string.IsNullOrEmpty(format))
            {
                var formatOptions = format.ToLower().Split(TypeHelper.CollectionElementSpliter);
                foreach (var formatOption in formatOptions)
                {
                    if (formatOption == SlimApiEnvironment.MetaResponseFormatPlain)
                    {
                        _usePlainText = true;
                    }
                    else
                    {
                        _requestFormat = formatOption;
                    }
                }
            }
        }

        public string GetMethodName()
        {
            return _methodName;
        }

        public string GetDecoderName()
        {
            return _requestFormat;
        }

        public IDictionary<string, object> DecodeParam(IRequestDecoder decoder)
        {
            try
            {
                return decoder.DecodeParam(_context.Request, null);
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

        public void OnSuccess(ApiResponse response)
        {
            WriteResponse(response);

            if (_logger.IsInfoEnabled)
            {
                var requestDescription = GetRequestDescritpion(_context.Request, 0, null);
                _logger.Info(requestDescription);
            }
        }

        public void OnHandledError(ApiResponse response, Exception rawException, bool error)
        {
            if (response == null)
            {
                OnUnhandledError(rawException);
                return;
            }

            WriteResponse(response);

            if (error)
            {
                if (_logger.IsErrorEnabled)
                {
                    var requestDescription = GetRequestDescritpion(_context.Request, response.Code, response.Message);
                    _logger.Error(requestDescription, rawException);
                }
            }
            else
            {
                if (_logger.IsInfoEnabled)
                {
                    var requestDescription = GetRequestDescritpion(_context.Request, 0, null);
                    _logger.Info(requestDescription);
                }
            }
        }

        public void OnMethodNotFound(string methodName)
        {
            const int code = 400;
            const string msg = "Unknown method.";

            WriteResponse(400, null, msg);

            if (_logger.IsWarnEnabled)
            {
                var requestDescription = GetRequestDescritpion(_context.Request, code, msg);
                _logger.Warn(requestDescription);
            }
        }

        public void OnDecoderNotFound(string methodName, string decoderName)
        {
            const int code = 400;
            const string msg = "The format is not supported on the method.";

            WriteResponse(code, null, msg);

            if (_logger.IsWarnEnabled)
            {
                var requestDescription = GetRequestDescritpion(_context.Request, code, msg);
                _logger.Warn(requestDescription);
            }
        }

        public void OnUnhandledError(Exception exception)
        {
            int code;
            string msg;

            var apiException = exception as ApiException;
            if (apiException == null)
            {
                code = 500;
                msg = "Internal error.";
            }
            else
            {
                code = apiException.Code;
                msg = apiException.Description;
            }

            WriteResponse(code, null, msg);

            if (_logger.IsErrorEnabled)
            {
                var requestDescription = GetRequestDescritpion(_context.Request, code, msg);
                _logger.Error(requestDescription);
            }
        }

        private void WriteResponse(int code, object responseData, string responseMessage)
        {
            var apiResponse = new ApiResponse(code, responseMessage, responseData);
            WriteResponse(apiResponse);
        }

        private void WriteResponse(ApiResponse apiResponse)
        {
            var httpResponse = _context.Response;
            var isJsonp = !string.IsNullOrEmpty(_callbackName);

            if (_usePlainText)
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
                httpResponse.Write(_callbackName);
                httpResponse.Write("(");
            }

            var responseJson = JsonHelper.Serialize(apiResponse);
            httpResponse.Write(responseJson);

            if (isJsonp)
            {
                httpResponse.Write(")");
            }
        }

        private string GetRequestDescritpion(HttpRequest request, int code, string message)
        {
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

            if (code != 0)
            {
                sb.AppendLine();
                sb.Append("Code: ").Append(code);
            }

            if (!string.IsNullOrEmpty(message))
            {
                sb.AppendLine();
                sb.Append("Message: ").Append(message);
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
    }
}