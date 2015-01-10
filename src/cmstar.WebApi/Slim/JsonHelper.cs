using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using cmstar.Serialization.Json;
using cmstar.Serialization.Json.Contracts;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 包含Slim WebAPI中与JSON序列化有关的方法。
    /// </summary>
    internal static class JsonHelper
    {
        private static readonly object SyncBlock = new object();
        private static JsonSerializer _jsonSerializer;

        public static JsonSerializer GetSerializer()
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
                jsonContractResolver.CaseSensitive = false;
                _jsonSerializer = new JsonSerializer(jsonContractResolver);
            }

            return _jsonSerializer;
        }

        public static string Serialize(object obj)
        {
            var stringBuilder = new StringBuilder(256);
            var stringWriter = new StringWriter(stringBuilder);

            using (var jsonWriter = new JsonWriter(stringWriter))
            {
                jsonWriter.EscapeSolidus = false;

                var serializer = GetSerializer();
                var contractResolver = serializer.ContractResolver;
                var contract = contractResolver.ResolveContract(obj);

                var state = new JsonSerializingState();
                state.CheckCycleReference = false;

                contract.Write(jsonWriter, state, contractResolver, obj);
            }

            return stringBuilder.ToString();
        }

        public static object Deserialize(string json, Type type)
        {
            using (var reader = new StringReader(json))
            {
                return GetSerializer().Deserialize(reader, type);
            }
        }

        public static object Deserialize(TextReader reader, Type type)
        {
            return GetSerializer().Deserialize(reader, type);
        }

        private static JsonContract GetCustomFormatDateTimeContract()
        {
            var contract = new CustomFormatDateTimeContract();
            contract.Format = "yyyy-MM-dd HH:mm:ss";
            return contract;
        }
    }
}
