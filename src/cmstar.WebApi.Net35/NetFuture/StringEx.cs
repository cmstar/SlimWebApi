using System.Linq;

namespace cmstar.WebApi.NetFuture
{
    internal static class StringEx
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.All(char.IsWhiteSpace);
        }
    }
}
