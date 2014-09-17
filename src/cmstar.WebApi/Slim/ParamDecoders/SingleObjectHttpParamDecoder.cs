using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using cmstar.RapidReflection.Emit;

namespace cmstar.WebApi.Slim.ParamDecoders
{
    /// <summary>
    /// <see cref="IRequestDecoder"/>的实现。
    /// 解析HTTP请求参数（GET或POST方式），并将这些HTTP参数映射到只有一个参数的方法的唯一参数。
    /// </summary>
    public class SingleObjectHttpParamDecoder : IRequestDecoder
    {
        /// <summary>
        /// 定义匹配类型成员名称时的优先级。
        /// </summary>
        public enum MemberPriority
        {
            /// <summary>
            /// 在名称模糊匹配时，若能同时匹配到属性与域，则优先匹配属性名称。
            /// </summary>
            Property,

            /// <summary>
            /// 在名称模糊匹配时，若能同时匹配到属性与域，则优先匹配域名称。
            /// </summary>
            Field
        }

        private readonly Dictionary<string, MemberCache> _memberMap;
        private readonly Func<object> _constructor;
        private readonly string _paramName;

        /// <summary>
        /// 初始化<see cref="SingleObjectHttpParamDecoder"/>的新实例。
        /// </summary>
        /// <param name="paramInfoMap">包含方法参数相关的信息。</param>
        /// <param name="nameComparer">指定类型名称和HTTP参数名称间的匹配方式。</param>
        /// <param name="memerPriority">指定匹配类型成员名称时的优先级。</param>
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
                    MemberType = prop.PropertyType,
                    IsGenericCollection = TypeHelper.IsGenericCollection(prop.PropertyType)
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
                    MemberType = field.FieldType,
                    IsGenericCollection = TypeHelper.IsGenericCollection(field.FieldType)
                };
                _memberMap.Add(field.Name, m);
            }
        }

        /// <summary>
        /// 解析<see cref="HttpRequest"/>并创建该请求所对应要调用方法的参数值集合。
        /// 集合以参数名称为key，参数的值为value。
        /// </summary>
        /// <param name="request">HTTP请求。</param>
        /// <returns>记录参数名称和对应的值。</returns>
        public IDictionary<string, object> DecodeParam(HttpRequest request)
        {
            if (_memberMap == null)
                return new Dictionary<string, object>(0);

            var instance = _constructor();

            foreach (var key in request.ExplicicParamKeys())
            {
                // the key may be null in http params
                if (key == null)
                    continue;

                MemberCache m;
                if (!_memberMap.TryGetValue(key, out m))
                    continue;

                var httpParam = request.ExplicicParam(key);
                var value = m.IsGenericCollection
                    ? TypeHelper.ConvertToCollection(httpParam, m.MemberType)
                    : TypeHelper.ConvertString(httpParam, m.MemberType);

                m.Setter(instance, value);
            }

            return new Dictionary<string, object>(1) { { _paramName, instance } };
        }

        private class MemberCache
        {
            public Type MemberType;
            public Action<object, object> Setter;
            public bool IsGenericCollection;
        }
    }
}
