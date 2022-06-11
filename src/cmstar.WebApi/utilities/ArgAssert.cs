using System;
using System.Linq;

namespace cmstar.WebApi
{
    internal static class ArgAssert
    {
        public static void NotNull(object arg, string name)
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        public static void NotNullOrEmpty(string value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            if (value.Length == 0)
            {
                var msg = string.Format(
                    "The length of the argument \"{0}\" must be greater than zero.",
                    parameterName);
                throw new ArgumentException(msg);
            }
        }

        public static void NotNullOrEmptyOrWhitespace(string value, string parameterName)
        {
            NotNullOrEmpty(value, parameterName);

            if (value.All(char.IsWhiteSpace))
            {
                var msg = string.Format(
                    "The argument \"{0}\" only contains empty characters.",
                    parameterName);
                throw new ArgumentException(msg);
            }
        }
    }
}
