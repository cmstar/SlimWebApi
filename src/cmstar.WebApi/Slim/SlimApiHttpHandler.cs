using System;
using System.Collections.Generic;
using System.Web;
using Common.Logging;

namespace cmstar.WebApi.Slim
{
    public abstract class SlimApiHttpHandler : IHttpHandler
    {
        private static readonly Dictionary<Type, SlimApiInvocationHandler> InternalHandlers
            = new Dictionary<Type, SlimApiInvocationHandler>();

        public void ProcessRequest(HttpContext context)
        {
            var httpResponse = context.Response;

            httpResponse.AddHeader("Pragma", "No-Cache");
            httpResponse.Expires = 0;
            httpResponse.CacheControl = "no-cache";

            try
            {
                var internalHandler = GetInternalHandler();
                internalHandler.ProcessRequest(context);
            }
            catch (Exception ex)
            {
                httpResponse.ContentType = "text/plain; charset=utf-8";
                httpResponse.StatusCode = 500;
                httpResponse.Write("500 internal error.");

                var logger = LogManager.GetLogger(GetType());
                logger.Fatal("Failed on handling the request.", ex);
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public abstract void Setup(ApiSetup setup);

        private SlimApiInvocationHandler GetInternalHandler()
        {
            var type = GetType();

            SlimApiInvocationHandler internalHandler;
            if (InternalHandlers.TryGetValue(type, out internalHandler))
                return internalHandler;

            lock (InternalHandlers)
            {
                if (InternalHandlers.TryGetValue(type, out internalHandler))
                    return internalHandler;

                var setup = new ApiSetup(type);
                Setup(setup);

                internalHandler = new SlimApiInvocationHandler(setup.CallerType, setup.ApiMethodInfos);
                InternalHandlers.Add(type, internalHandler);

                return internalHandler;
            }
        }
    }
}
