using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using cmstar.Util;

namespace cmstar.WebApi
{
    /// <summary>
    /// Provides methods for type management in the API framework.
    /// </summary>
    internal static class TypeHelper
    {
        /// <summary>
        /// The character used to split a string into an array.
        /// e.g. a~b~c => ["a", "b", "c"]
        /// </summary>
        public const char CollectionElementDelimiter = '~';

        /// <summary>
        /// The array contains only the <see cref="CollectionElementDelimiter"/> 
        /// and is used for splitting strings.
        /// </summary>
        public static readonly char[] CollectionElementSpliter = { CollectionElementDelimiter };

        /// <summary>
        /// The generic defination of collections.
        /// </summary>
        public static Type GenericCollecitonDefination = typeof(ICollection<>);

        /// <summary>
        /// Returns an instance of <see cref="MemberTypeStat"/> which contains statistic infomation
        /// for the parameters of the specified method.
        /// </summary>
        /// <param name="methodInfo">The method.</param>
        /// <returns>An instance of <see cref="MemberTypeStat"/>.</returns>
        public static MemberTypeStat GetMethodParamStat(MethodInfo methodInfo)
        {
            ArgAssert.NotNull(methodInfo, "methodInfo");

            var stat = new MemberTypeStat();
            var ps = methodInfo.GetParameters();
            CountMemberTypes(stat, ps.Select(x => x.ParameterType));
            return stat;
        }

        /// <summary>
        /// Returns an instance of <see cref="MemberTypeStat"/> which contains statistic infomation
        /// for the properties and fields in the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An instance of <see cref="MemberTypeStat"/>.</returns>
        public static MemberTypeStat GetTypeMemberStat(Type type)
        {
            ArgAssert.NotNull(type, "Type");

            var stat = new MemberTypeStat();

            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            CountMemberTypes(stat, props.Select(x => x.PropertyType));

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            CountMemberTypes(stat, fields.Select(x => x.FieldType));

            return stat;
        }

        /// <summary>
        /// Determines wheter a type is a simple type.
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
        /// Determines wheter a type is a simple type or a generic colleciton whose elements are all simple.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// true if the type is a simple type or a generic colleciton whose elements are all simple; otherwise false.
        /// </returns>
        public static bool IsSimpleTypeOrCollection(Type type)
        {
            if (IsSimpleType(type))
                return true;

            var elementType = GetElementType(type);
            return elementType != null && IsSimpleType(elementType);
        }

        /// <summary>
        /// Determines whether a type is a generic collection type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>true if the type is a generic colleciton type; otherwise false.</returns>
        public static bool IsGenericCollection(Type type)
        {
            return ReflectionUtils.GetGenericArguments(type, GenericCollecitonDefination) != null;
        }

        /// <summary>
        /// Converts a string to a collection specifed by the <paramref name="collectionType"/>. 
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="collectionType">The type of the collection.</param>
        /// <returns>The instance of the specified type.</returns>
        /// <remarks>
        /// The string is splitted into an array by <see cref="CollectionElementDelimiter"/>,
        /// and then each element is to be convert the the target type of the collection.
        /// </remarks>
        public static object ConvertToCollection(string value, Type collectionType)
        {
            if (value == null)
                return null;

            var elementType = GetElementType(collectionType);
            if (elementType == null)
                throw CannotCastValue(value, collectionType, null);

            if (value.Length == 0)
                return Array.CreateInstance(elementType, 0);

            try
            {
                var stringValues = value.Split(CollectionElementSpliter);
                var elements = CreateElementArray(elementType, stringValues);

                if (collectionType.IsInterface || collectionType.IsArray)
                    return elements;

                var collection = CreateCollectionInstance(collectionType, elementType, elements);
                return collection;
            }
            catch (Exception ex)
            {
                throw CannotCastValue(value, collectionType, ex);
            }
        }

        /// <summary>
        /// Converts a string to an equivalant object of the specified type.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="type">The target type.</param>
        /// <returns>The instance of the specified type.</returns>
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
                if (ReflectionUtils.IsNullableType(type))
                {
                    if (string.IsNullOrEmpty(value))
                        return null;

                    type = Nullable.GetUnderlyingType(type);
                }

                if (type == typeof(bool))
                    return ToBoolean(value);

                if (type.IsEnum)
                    return Enum.Parse(type, value, true);

                if (typeof(IConvertible).IsAssignableFrom(type))
                    return Convert.ChangeType(value, type);

                if (type == typeof(Guid))
                    return new Guid(value);
            }
            catch (Exception ex)
            {
                innerException = ex;
            }

            throw CannotCastValue(value, type, innerException);
        }

        private static void CountMemberTypes(MemberTypeStat output, IEnumerable<Type> types)
        {
            foreach (var t in types)
            {
                if (IsSimpleType(t))
                {
                    output.Plains++;
                    continue;
                }

                if (t == typeof(Stream) || t == typeof(HttpFileCollection))
                {
                    output.HasFileInput = true;
                    continue;
                }

                var elementType = GetElementType(t);
                if (elementType != null)
                {
                    output.Collections++;

                    if (IsSimpleType(elementType))
                    {
                        output.PlainCollections++;
                        continue;
                    }
                }

                output.Others++;
            }
        }

        private static bool ToBoolean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            // treat any non-zero numbers as true; only false if the value is exactly zero
            double d;
            if (double.TryParse(value, out d))
                return !d.Equals(0D);

            return bool.Parse(value);
        }

        private static Exception CannotCastValue(string value, Type type, Exception innerException)
        {
            var msg = string.Format("Cannnot cast value \"{0}\" to type {1}.", value, type);
            return new InvalidCastException(msg, innerException);
        }

        private static Array CreateElementArray(Type elementType, string[] stringValues)
        {
            var resultArray = Array.CreateInstance(elementType, stringValues.Length);

            for (int i = 0; i < stringValues.Length; i++)
            {
                var element = ConvertString(stringValues[i], elementType);
                resultArray.SetValue(element, i);
            }

            return resultArray;
        }

        private static object CreateCollectionInstance(Type collectionType, Type elementType, IEnumerable elements)
        {
            var collection = Activator.CreateInstance(collectionType);
            var appendMethod = typeof(TypeHelper)
                .GetMethod("PerformAppending", BindingFlags.Instance | BindingFlags.NonPublic)
                .MakeGenericMethod(elementType);

            appendMethod.Invoke(null, new[] { collection, elements });
            return collection;
        }

        // ReSharper disable UnusedMember.Local
        private static void PerformAppending<T>(ICollection<T> collection, IEnumerable elements)
        {
            foreach (var element in elements)
            {
                collection.Add((T)element);
            }
        }
        // ReSharper restore UnusedMember.Local

        private static Type GetElementType(Type collectionType)
        {
            var genericArguments = ReflectionUtils.GetGenericArguments(
                collectionType, typeof(ICollection<>));

            if (genericArguments == null || genericArguments.Length != 1)
                return null;

            return genericArguments[0];
        }
    }

    internal class MemberTypeStat
    {
        public int Plains;
        public int Others;
        public int PlainCollections;
        public int Collections;

        /// <summary>
        /// true if there are only plain members (exclude collections).
        /// </summary>
        public bool IsPurePlain
        {
            get { return Others == 0 && PlainCollections == 0 && !HasFileInput; }
        }

        /// <summary>
        /// true if any complex member exists.
        /// </summary>
        public bool HasCoplexMember
        {
            get { return Others > 0; }
        }

        /// <summary>
        /// true if any <see cref="Stream"/> or <see cref="HttpFileCollection"/> member exists.
        /// </summary>
        public bool HasFileInput { get; set; }
    }
}
