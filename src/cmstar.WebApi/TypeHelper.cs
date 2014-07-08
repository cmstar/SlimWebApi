using System;
using System.Linq;
using System.Reflection;
using cmstar.Util;

namespace cmstar.WebApi
{
    /// <summary>
    /// Provides methods for type management in the API framework.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// Indicates whether a method is a plain method 
        /// whose parameters' types are all simple (see <see cref="IsSimpleType"/>).
        /// </summary>
        /// <param name="methodInfo">The method.</param>
        /// <returns>true if the method is plain; otherwise false.</returns>
        public static bool IsPlainMethod(MethodInfo methodInfo)
        {
            ArgAssert.NotNull(methodInfo, "methodInfo");

            var ps = methodInfo.GetParameters();
            return ps.Length == 0 || ps.All(x => IsSimpleType(x.ParameterType));
        }

        /// <summary>
        /// Indicates whether a type is a plain type
        /// whose members' types are all simple (see <see cref="IsSimpleType"/>).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is plain; otherwise false.</returns>
        public static bool IsPlainType(Type type)
        {
            ArgAssert.NotNull(type, "Type");

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (props.Any(x => !IsSimpleType(x.PropertyType)))
                return false;

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            if (fields.Any(x => !IsSimpleType(x.FieldType)))
                return false;

            return true;
        }

        /// <summary>
        /// Determine wheter a type is a simple type.
        /// A simple type means that the instance can be convert from/to a string easily.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is a simple type; otherwise false.</returns>
        public static bool IsSimpleType(Type type)
        {
            ArgAssert.NotNull(type, "Type");

            type = ReflectionUtils.GetUnderlyingType(type);

            if (type.IsPrimitive)
                return true;

            if (typeof(IConvertible).IsAssignableFrom(type))
                return true;

            if (type == typeof(Guid))
                return true;

            return false;
        }

        /// <summary>
        /// Converts a string to an equivalant object of the specified type.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="type">The target type.</param>
        /// <returns>The instance of the specified type.</returns>
        /// <exception cref="InvalidCastException">Cannot convert the string.</exception>
        /// <remarks>
        /// If <paramref name="value"/> is <c>null</c>, the default value of the type will be returned.
        /// </remarks>
        public static object ConvertString(string value, Type type)
        {
            if (value == null)
                return type.IsValueType ? Activator.CreateInstance(type) : null;

            Exception innerException = null;
            try
            {
                type = ReflectionUtils.GetUnderlyingType(type);

                if (typeof(IConvertible).IsAssignableFrom(type))
                    return Convert.ChangeType(value, type);

                if (type == typeof(Guid))
                    return new Guid(value);
            }
            catch (Exception ex)
            {
                innerException = ex;
            }

            var msg = string.Format("Cannnot cast value \"{0}\" to type {1}.", value, type);
            throw new InvalidCastException(msg, innerException);
        }
    }
}
