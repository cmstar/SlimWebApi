using System.Linq;

namespace cmstar.WebApi.NetFuture
{
    internal static class StringUtils
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
            return string.IsNullOrEmpty(value) || value.All(char.IsWhiteSpace);
        }
    }
}
