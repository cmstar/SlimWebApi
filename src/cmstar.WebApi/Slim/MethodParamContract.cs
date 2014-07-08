using System;
using System.Collections.Generic;
using cmstar.Serialization.Json;

namespace cmstar.WebApi.Slim
{
    public class MethodParamContract : JsonContract
    {
        private readonly Dictionary<string, JsonContract> _paramContractMap;

        public MethodParamContract(
            Dictionary<string, Type> paramTypeMap, IJsonContractResolver jsonContractResolver)
            : base(typeof(object))
        {
            ArgAssert.NotNull(paramTypeMap, "paramTypeMap");
            ArgAssert.NotNull(jsonContractResolver, "jsonContractResolver");

            _paramContractMap = new Dictionary<string, JsonContract>();
            foreach (var keyValue in paramTypeMap)
            {
                var paramName = keyValue.Key;
                var paramType = keyValue.Value;
                var contract = jsonContractResolver.ResolveContract(paramType);
                _paramContractMap.Add(paramName, contract);
            }
        }

        protected override void DoWrite(
            JsonWriter writer,
            JsonSerializingState state,
            IJsonContractResolver contractResolver,
            object obj)
        {
            throw new InvalidOperationException();
        }

        protected override object DoRead(JsonReader reader, JsonDeserializingState state)
        {
            reader.Read();

            var token = reader.Token;

            if (token == JsonToken.NullValue || token == JsonToken.None)
                return null;

            if (token != JsonToken.ObjectStart)
                throw JsonContractErrors.UnexpectedToken(JsonToken.ObjectStart, token);

            var paramValueMap = new Dictionary<string, object>();

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
                    continue;

                var value = contract.Read(reader, state);
                paramValueMap.Add(paramName, value);
            }

            return paramValueMap;
        }
    }
}
