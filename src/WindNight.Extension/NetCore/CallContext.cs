#if !NETFRAMEWORK
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json.Extension;

namespace WindNight.Extension.NetCore
{
    /// <summary>
    /// provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class CallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();

        /// <summary>
        /// stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">the name with which to associate the new item in the call context.</param>
        /// <param name="data">the object to store in the call context.</param>
        public static void SetData(string name, object data) =>
            state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;


        /// <summary>
        /// retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">the name of the item in the call context.</param>
        /// <returns>the object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static object GetData(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data being retrieved. Must match the type used when the <paramref name="name"/> was set via <see cref="SetData{T}(string, T)"/>.</typeparam>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or a default value for <typeparamref name="T"/> if none is found.</returns>
        public static T GetData<T>(string name) =>
            state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value.To<T>() : default;
    }


}
#endif
