using System;

namespace cmstar.WebApi
{
    internal static class ReflectionUtils
    {
        public static bool IsNullableType(Type t)
        {
            ArgAssert.NotNull(t, "t");

            return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetUnderlyingType(Type t)
        {
            return IsNullableType(t) ? Nullable.GetUnderlyingType(t) : t;
        }

        public static bool IsOrIsSubClassOf(Type thisType, Type targetType)
        {
            ArgAssert.NotNull(thisType, "thisType");
            ArgAssert.NotNull(targetType, "targetType");

            return thisType == targetType || thisType.IsSubclassOf(targetType);
        }

        public static Type[] GetGenericArguments(Type type, Type genericTypeDefinition)
        {
            ArgAssert.NotNull(type, "type");
            ArgAssert.NotNull(genericTypeDefinition, "genericTypeDefinition");

            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                var msg = string.Format(
                    "The type {0} is not a generic type definition.",
                    genericTypeDefinition.Name);
                throw new ArgumentException(msg, "genericTypeDefinition");
            }

            if (genericTypeDefinition.IsInterface)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
                    return type.GetGenericArguments();

                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    if (interfaceType.GetGenericTypeDefinition() != genericTypeDefinition)
                        continue;

                    return interfaceType.GetGenericArguments();
                }
            }
            else
            {
                var baseType = type;
                do
                {
                    if (!baseType.IsGenericType)
                        continue;

                    if (baseType.GetGenericTypeDefinition() != genericTypeDefinition)
                        continue;

                    return baseType.GetGenericArguments();

                } while ((baseType = baseType.BaseType) != null);
            }

            return null;
        }
    }
}
