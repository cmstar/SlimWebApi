#if NETCORE || NETSTANDARD
using System.Collections.Concurrent;
using System.Threading;

namespace cmstar.WebApi
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    /// <remarks>
    /// ref http://www.cazzulino.com/callcontext-netstandard-netcore.html
    /// 模拟实现 .net Standard 没有定义的 CallContext 类。
    /// </remarks>
    public static class CallContext
    {
        private static readonly ConcurrentDictionary<string, AsyncLocal<object>> State
            = new ConcurrentDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData(string name, object data) =>
            State.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object GetData(string name) =>
            State.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;

        /// <summary>
        /// Retrieves an object with the specified name from the logical call context.
        /// </summary>
        /// <param name="name">The name of the item in the logical call context.</param>
        /// <returns>The object in the logical call context associated with the specified name.</returns>
        public static object LogicalGetData(string name) => GetData(name);

        /// <summary>
        /// Stores a given object in the logical call context and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the logical call context.</param>
        /// <param name="data">The object to store in the logical call context, this object must be serializable.</param>
        public static void LogicalSetData(string name, object data) => SetData(name, data);
    }
}
#endif