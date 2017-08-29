using System;
using System.Collections.Generic;
using cmstar.Serialization.Json;

namespace cmstar.WebApi.Slim
{
    /// <summary>
    /// 此<see cref="JsonContract"/>用于从JSON中获取调用方法所需的参数信息。
    /// 其将JSON反序列化到一个字典，其键为方法参数的名称，值为参数的值。
    /// </summary>
    public class MethodParamContract : JsonContract
    {
        private readonly Dictionary<string, JsonContract> _paramContractMap;

        /// <summary>
        /// 初始化<see cref="MethodParamContract"/>的新实例并指定各参数的类型。
        /// </summary>
        /// <param name="paramContractMap">包含方法中各个参数的名称参数类型所对应的<see cref="JsonContract"/>。</param>
        public MethodParamContract(IDictionary<string, JsonContract> paramContractMap)
            : base(typeof(object))
        {
            ArgAssert.NotNull(paramContractMap, "paramContractMap");

            _paramContractMap = new Dictionary<string, JsonContract>(
                paramContractMap.Count, StringComparer.OrdinalIgnoreCase);

            foreach (var kv in paramContractMap)
            {
                _paramContractMap.Add(kv.Key, kv.Value);
            }
        }

        /// <inheritdoc />
        protected override void DoWrite(
            JsonWriter writer,
            JsonSerializingState state,
            IJsonContractResolver contractResolver,
            object obj)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc />
        protected override object DoRead(JsonReader reader, JsonDeserializingState state)
        {
            reader.Read();

            var token = reader.Token;

            if (token == JsonToken.NullValue || token == JsonToken.None)
                return null;

            if (token != JsonToken.ObjectStart)
                throw JsonContractErrors.UnexpectedToken(JsonToken.ObjectStart, token);

            var paramValueMap = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            while (reader.Read())
            {
                token = reader.Token;

                if (token == JsonToken.ObjectEnd)
                    break;

                if (token == JsonToken.Comma)
                    continue;

                if (token != JsonToken.PropertyName)
                    throw JsonContractErrors.UnexpectedToken(token);

                var paramName = (string)reader.Value;

                JsonContract contract;
                if (!_paramContractMap.TryGetValue(paramName, out contract))
                {
                    SkipPropertyValue(reader);
                    continue;
                }

                var value = contract.Read(reader, state);
                paramValueMap.Add(paramName, value);
            }

            return paramValueMap;
        }
    }
}
