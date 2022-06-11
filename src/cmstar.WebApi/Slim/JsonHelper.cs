using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using cmstar.Serialization.Json;
using cmstar.Serialization.Json.Contracts;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 包含Slim WebAPI中与JSON序列化有关的方法。
    /// </summary>
    public static class JsonHelper
    {
        public const string DefaultDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        private static readonly JsonDeserializingState ApiResponseDeserializingState;
        private static readonly JsonSerializer Serializer;

        static JsonHelper()
        {
            var knownContracts = new Dictionary<Type, JsonContract>();
            var dateTimeContract = new CustomFormatDateTimeOffsetContract { Format = DefaultDateTimeFormat };
            knownContracts.Add(typeof(DateTimeOffset), dateTimeContract);

            var jsonContractResolver = new JsonContractResolver(knownContracts);
            jsonContractResolver.CaseSensitive = false;
            Serializer = new JsonSerializer(jsonContractResolver);

            ApiResponseDeserializingState = new JsonDeserializingState
            {
                NullValueHandling = JsonDeserializationNullValueHandling.AsDefaultValue
            };
        }

        /// <summary>
        /// 获取在 Slim WebAPI 的JSON交互中使用的<see cref="JsonSerializer"/>实例。
        /// </summary>
        /// <returns></returns>
        public static JsonSerializer GetSerializer()
        {
            return Serializer;
        }

        /// <summary>
        /// 序列化指定的对象。
        /// </summary>
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

        /// <summary>
        /// 反序列化给定的JSON到指定类型的实例。
        /// </summary>
        public static object Deserialize(string json, Type type)
        {
            using (var reader = new StringReader(json))
            {
                return GetSerializer().Deserialize(reader, type);
            }
        }

        /// <summary>
        /// 反序列化给定的JSON到<see cref="ApiResponse{T}"/>实例。
        /// </summary>
        public static ApiResponse<T> DeserializeApiResponse<T>(string json)
        {
            var serializer = GetSerializer();
            var result = serializer.Deserialize<ApiResponse<T>>(json, ApiResponseDeserializingState);
            return result;
        }

        /// <summary>
        /// 从<paramref name="reader"/>读取JSON，并反序列化到指定类型的实例。
        /// </summary>
        /// <param name="reader">用于读取JSON的<see cref="TextReader"/>。</param>
        /// <param name="type">目标类型。</param>
        /// <param name="keepReaderOpen">若为false，则读取结束后关闭<paramref name="reader"/>。</param>
        /// <returns>目标类型的实例。</returns>
        public static object Deserialize(TextReader reader, Type type, bool keepReaderOpen)
        {
            var serializer = GetSerializer();
            var contract = serializer.ContractResolver.ResolveContract(type);

            using (var jsonReader = new JsonReader(reader) { AutoCloseInternalReader = !keepReaderOpen })
            {
                return contract.Read(jsonReader, new JsonDeserializingState());
            }
        }
    }
}
