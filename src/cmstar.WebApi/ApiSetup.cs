using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cmstar.RapidReflection.Emit;

namespace cmstar.WebApi
{
    /// <summary>
    /// 提供API注册的便捷入口。
    /// </summary>
    public partial class ApiSetup
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        private readonly List<ApiMethodInfo> _apiMethodInfos;
        private readonly LogSetup _logSetup;

        /// <summary>
        /// 初始化<see cref="ApiSetup"/>的新实例。
        /// </summary>
        /// <param name="callerType">进行API注册的类型。</param>
        public ApiSetup(Type callerType)
        {
            ArgAssert.NotNull(callerType, "callerType");

            _apiMethodInfos = new List<ApiMethodInfo>();
            _logSetup = LogSetup.Default();
            CallerType = callerType;
        }

        /// <summary>
        /// 获取进行API注册的类型。
        /// </summary>
        public Type CallerType { get; private set; }

        /// <summary>
        /// 获取API缓存提供器。若API方法注册中没有单独指定缓存提供器，则套用此缓存提供器。
        /// </summary>
        public IApiCacheProvider CacheProvider { get; private set; }

        /// <summary>
        /// 获取缓存的超时时间。若API方法注册中没有单独指定超时时间，则套用此超时时间。
        /// </summary>
        public TimeSpan CacheExpiration { get; private set; }

        /// <summary>
        /// 获取日志相关的配置入口。
        /// </summary>
        public LogSetup Log
        {
            get { return _logSetup; }
        }

        /// <summary>
        /// 获取于当前实例注册的所有API方法注册信息的序列。
        /// </summary>
        public IEnumerable<ApiMethodInfo> ApiMethodInfos
        {
            get { return _apiMethodInfos; }
        }

        /// <summary>
        /// 设置从当前实例注册的API所使用的缓存提供器。
        /// 若API方法注册中没有单独指定缓存提供器，将套用此提供器。
        /// </summary>
        /// <param name="provider">API缓存提供器。</param>
        /// <param name="expiration">缓存的超时时间。</param>
        public void SetupCacheBase(IApiCacheProvider provider, TimeSpan expiration)
        {
            if (expiration.Ticks <= 0)
                throw new ArgumentException("The expiration must be greater than zero.", "expiration");

            CacheProvider = provider;
            CacheExpiration = expiration;
        }

        /// <summary>
        /// 添加一个API方法注册。
        /// </summary>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="method">被注册为WebAPI的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method(Func<object> provider, MethodInfo method)
        {
            return AppendMethod(provider, method);
        }

        /// <summary>
        /// 添加一个API方法注册。
        /// </summary>
        /// <param name="provider">
        /// 提供API逻辑实现的类型实例。若方法为静态方法，使用<c>null</c>。
        /// </param>
        /// <param name="method">被注册为WebAPI的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method(object provider, MethodInfo method)
        {
            return AppendMethod(provider, method);
        }

        /// <summary>
        /// 从指定类型的实例加载API方法的注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API方法的类型。</typeparam>
        /// <param name="provider">提供API方法的实例。</param>
        /// <param name="parseAttribute">
        /// 若为true，则仅加载标记有<see cref="ApiMethodAttribute"/>的方法；否则加载所有方法。
        /// 方法的筛选受<paramref name="bindingFlags"/>影响。
        /// </param>
        /// <param name="bindingFlags">指定方法的过滤方式。默认加载所有公共方法。</param>
        /// <returns>返回被注册的API方法的注册信息。</returns>
        public IEnumerable<ApiMethodSetup> Auto<TProvider>(
            TProvider provider, bool parseAttribute = true, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            var methods = GetMethods(typeof(TProvider), bindingFlags);
            return AppendMethods(methods, m => (() => provider), parseAttribute);
        }

        /// <summary>
        /// 从指定类型加载API方法的注册。
        /// </summary>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="parseAttribute">
        /// 若为true，则仅加载标记有<see cref="ApiMethodAttribute"/>的方法；否则加载所有方法。
        /// 方法的筛选受<paramref name="bindingFlags"/>影响。
        /// </param>
        /// <param name="bindingFlags">指定方法的过滤方式。默认加载所有公共方法。</param>
        /// <returns>返回被注册的API方法的注册信息。</returns>
        public IEnumerable<ApiMethodSetup> Auto<TProvider>(
            Func<TProvider> provider, bool parseAttribute = true, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            var methods = GetMethods(typeof(TProvider), bindingFlags);
            return AppendMethods(methods, m => (() => provider()), parseAttribute);
        }

        /// <summary>
        /// 从指定类型加载API方法的注册。可以使用此方法加载静态/抽象类中的方法。
        /// 若加载实例方法，则类型必须有一个无参的构造函数。
        /// </summary>
        /// <param name="providerType">提供API方法的类型。</param>
        /// <param name="singleton">true若在API提供对象上使用单例模式；否则为false。默认为true。</param>
        /// <param name="parseAttribute">
        /// 若为true，则仅加载标记有<see cref="ApiMethodAttribute"/>的方法；否则加载所有方法。
        /// 方法的筛选受<paramref name="bindingFlags"/>影响。
        /// </param>
        /// <param name="bindingFlags">指定方法的过滤方式。默认加载所有公共方法。</param>
        /// <returns>返回被注册的API方法的注册信息。</returns>
        public IEnumerable<ApiMethodSetup> FromType(Type providerType, bool singleton = true,
            bool parseAttribute = true, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            ArgAssert.NotNull(providerType, "providerType");

            Func<object> provider = null;
            Func<MethodInfo, Func<object>> providerCreator = m =>
            {
                if (m.IsStatic)
                    return null;

                if (provider == null)
                {
                    if (singleton)
                    {
                        var instance = Activator.CreateInstance(providerType);
                        provider = () => instance;
                    }
                    else
                    {
                        provider = ConstructorInvokerGenerator.CreateDelegate(providerType);
                    }
                }

                return provider;
            };

            var methods = GetMethods(providerType, bindingFlags);
            var methodSetups = AppendMethods(methods, providerCreator, parseAttribute);
            return methodSetups;
        }

        private MethodInfo[] GetMethods(Type type, BindingFlags bingBindingFlags)
        {
            var methods = type.GetMethods(bingBindingFlags);

            // 默认去掉 .net 底层基类里的方法。如果确实需要这些方法，只能一个一个注册，不能通过自动注册。
            if (methods.Length > 0)
            {
                methods = methods.Where(x => x.DeclaringType != typeof(object)).ToArray();
            }

            return methods;
        }

        private List<ApiMethodSetup> AppendMethods(
            MethodInfo[] methods, Func<MethodInfo, Func<object>> providerCreator, bool parseAttribute)
        {
            var methodSetups = new List<ApiMethodSetup>();

            if (parseAttribute)
            {
                foreach (var methodInfo in methods)
                {
                    var attrs = methodInfo.GetCustomAttributes(typeof(ApiMethodAttribute), true);
                    if (attrs.Length == 0)
                        continue;

                    // ApiMethodAttribute是可以被标注多次的，此时将一个方法发布为API多次
                    foreach (ApiMethodAttribute apiMethodAttr in attrs)
                    {
                        var provider = methodInfo.IsStatic ? null : providerCreator(methodInfo);
                        var apiMethodSetting = apiMethodAttr.GetUnderlyingSetting();
                        var apiSetupInfo = AppendMethod(provider, methodInfo, apiMethodSetting);
                        methodSetups.Add(apiSetupInfo);
                    }
                }
            }
            else
            {
                foreach (var methodInfo in methods)
                {
                    var provider = methodInfo.IsStatic ? null : providerCreator(methodInfo);
                    var methodSetup = AppendMethod(provider, methodInfo);
                    methodSetups.Add(methodSetup);
                }
            }

            return methodSetups;
        }

        private ApiMethodSetup AppendMethod(object provider, MethodInfo method)
        {
            if (provider == null && !method.IsStatic)
                throw new ArgumentNullException(
                    "provider", "The provider can not be null if the method is not static.");

            return AppendMethod(() => provider, method);
        }

        private ApiMethodSetup AppendMethod(
            Func<object> provider, MethodInfo method, ApiMethodSetting apiMethodSetting = null)
        {
            var apiMethodInfo = new ApiMethodInfo(provider, method, apiMethodSetting);
            var apiMethodSetup = new ApiMethodSetup(this, apiMethodInfo);

            _apiMethodInfos.Add(apiMethodInfo);
            return apiMethodSetup;
        }
    }
}