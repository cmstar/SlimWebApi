using System;
using System.Collections.Generic;
using System.Threading;
using cmstar.Serialization.Json;
using cmstar.Serialization.Json.Contracts;

namespace cmstar.WebApi.Slim
{
    public static class SlimApiEnvironment
    {
        public static string MetaParamMethodName = "~method";
        public static string MetaParamFormat = "~format";
        public static string MetaParamCallback = "~callback";

        public const string MetaRequestFormatJson = "json";
        public const string MetaRequestFormatPost = "post";
        public const string MetaRequestFormatGet = "get";

        private static readonly object SyncBlock = new object();
        private static JsonSerializer _jsonSerializer;

        public static JsonSerializer JsonSerializer
        {
            get
            {
                if (_jsonSerializer != null)
                    return _jsonSerializer;

                lock (SyncBlock)
                {
                    Thread.MemoryBarrier();

                    if (_jsonSerializer != null)
                        return _jsonSerializer;

                    var knonwContracts = new Dictionary<Type, JsonContract>();
                    var dateTimeContract = GetCustomFormatDateTimeContract();
                    knonwContracts.Add(typeof(DateTime), dateTimeContract);

                    var jsonContractResolver = new JsonContractResolver(knonwContracts);
                    _jsonSerializer = new JsonSerializer(jsonContractResolver);
                }

                return _jsonSerializer;
            }
        }

        private static JsonContract GetCustomFormatDateTimeContract()
        {
            var contract = new CustomFormatDateTimeContract();
            contract.Format = "yyyy-MM-dd HH:mm:ss";
            return contract;
        }
    }
}
