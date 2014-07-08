using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using cmstar.RapidReflection.Emit;

namespace cmstar.WebApi.Slim.ParamDecoders
{
    public class SingleObjectHttpParamDecoder : IRequestDecoder
    {
        public enum MemberPriority
        {
            Property,
            Field
        }

        private readonly Dictionary<string, MemberCache> _memberMap;
        private readonly Func<object> _constructor;
        private readonly string _paramName;

        public SingleObjectHttpParamDecoder(
            ApiMethodParamInfoMap paramInfoMap,
            IEqualityComparer<string> nameComparer = null,
            MemberPriority memerPriority = MemberPriority.Property)
        {
            ArgAssert.NotNull(paramInfoMap, "paramInfoMap");

            var paramCount = paramInfoMap.ParamCount;
            if (paramCount == 0)
                return;

            if (paramCount != 1)
            {
                var msg = string.Format("The method {0} should not have more than one parameter.", paramInfoMap.Method);
                throw new ArgumentException(msg, "paramInfoMap");
            }

            var paramInfo = paramInfoMap.ParamInfos.First().Value;
            var paramType = paramInfo.Type;

            _memberMap = new Dictionary<string, MemberCache>(nameComparer ?? ApiEnvironment.DefaultMethodNameComparer);
            _constructor = ConstructorInvokerGenerator.CreateDelegate(paramType);
            _paramName = paramInfo.Name;

            var props = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var setter = PropertyAccessorGenerator.CreateSetter(prop);
                var m = new MemberCache
                {
                    Setter = setter,
                    MemberType = prop.PropertyType
                };
                _memberMap.Add(prop.Name, m);
            }

            var fields = paramType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                if (_memberMap.ContainsKey(field.Name) && memerPriority != MemberPriority.Field)
                    continue;

                var setter = FieldAccessorGenerator.CreateSetter(field);
                var m = new MemberCache
                {
                    Setter = setter,
                    MemberType = field.FieldType
                };
                _memberMap.Add(field.Name, m);
            }
        }

        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            if (_memberMap == null)
                return new Dictionary<string, object>(0);

            var instance = _constructor();
            
            foreach (var key in request.Params.AllKeys)
            {
                // the key may be null in http params
                if (key == null)
                    continue;

                MemberCache m;
                if (!_memberMap.TryGetValue(key, out m))
                    continue;

                var httpParam = request.Params[key];
                var value = TypeHelper.ConvertString(httpParam, m.MemberType);

                m.Setter(instance, value);
            }

            return new Dictionary<string, object>(1) { { _paramName, instance } };
        }

        private class MemberCache
        {
            public Type MemberType;
            public Action<object, object> Setter;
        }
    }
}
