using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace cmstar.WebApi
{
    /// <summary>
    /// 提供API注册的便捷入口。
    /// </summary>
    public class ApiSetup
    {
        private readonly List<ApiMethodInfo> _apiMethodInfos;

        /// <summary>
        /// 初始化<see cref="ApiSetup"/>的新实例。
        /// </summary>
        /// <param name="callerType">进行API注册的类型。</param>
        public ApiSetup(Type callerType)
        {
            ArgAssert.NotNull(callerType, "callerType");

            _apiMethodInfos = new List<ApiMethodInfo>();
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
        /// 注册具有0个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TResult>(Func<TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有1个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, TResult>(Func<T1, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有2个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, TResult>(Func<T1, T2, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有3个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有4个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有5个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有6个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有7个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有8个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <typeparam name="T8">方法中第8个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有0个参数且没有返回值的方法到API注册信息中。
        /// </summary>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method(Action method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有1个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1>(Action<T1> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有2个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2>(Action<T1, T2> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有3个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3>(Action<T1, T2, T3> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有4个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有5个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有6个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有7个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 注册具有8个参数且有返回值的方法到API注册信息中。
        /// </summary>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <typeparam name="T8">方法中第8个参数的类型。</typeparam>
        /// <param name="method">注册的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, T6, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, T6, T7, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="TResult">方法返回值的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <typeparam name="T8">方法中第8个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider>(Func<TProvider> provider,
            Expression<Func<TProvider, Action>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5, T6>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5, T6, T7>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// 以非单例的方式添加API注册。
        /// </summary>
        /// <typeparam name="TProvider">提供API逻辑实现的类型。</typeparam>
        /// <typeparam name="T1">方法中第1个参数的类型。</typeparam>
        /// <typeparam name="T2">方法中第2个参数的类型。</typeparam>
        /// <typeparam name="T3">方法中第3个参数的类型。</typeparam>
        /// <typeparam name="T4">方法中第4个参数的类型。</typeparam>
        /// <typeparam name="T5">方法中第5个参数的类型。</typeparam>
        /// <typeparam name="T6">方法中第6个参数的类型。</typeparam>
        /// <typeparam name="T7">方法中第7个参数的类型。</typeparam>
        /// <typeparam name="T8">方法中第8个参数的类型。</typeparam>
        /// <param name="provider">返回一个对象，该对象为提供API逻辑实现的类型实例。</param>
        /// <param name="methodSelector">返回要注册为API的方法。</param>
        /// <returns><see cref="ApiMethodSetup"/>的实例。</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7, T8>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5, T6, T7, T8>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        private MethodInfo ResolveMethodInfo(LambdaExpression methodSelector)
        {
            ArgAssert.NotNull(methodSelector, "methodSelector");
            var body = methodSelector.Body;

            // exp: Convert(content)
            var unaryExpression = body as UnaryExpression;
            if (unaryExpression != null)
                body = unaryExpression.Operand;

            // exp: CreateDelegate(methodSignature, instance, methodInfo)
            var methodCallExpression = (MethodCallExpression)body;
            var methodConstantExpression = (ConstantExpression)methodCallExpression.Arguments[2];
            var methodInfo = (MethodInfo)methodConstantExpression.Value;

            return methodInfo;
        }

        private ApiMethodSetup AppendMethod(Delegate method)
        {
            ArgAssert.NotNull(method, "method");
            return AppendMethod(method.Target, method.Method);
        }

        private ApiMethodSetup AppendMethod(object provider, MethodInfo method)
        {
            if (provider == null && !method.IsStatic)
                throw new ArgumentNullException(
                    "provider", "The provider can not be null if the method is not static.");

            return AppendMethod(() => provider, method);
        }

        private ApiMethodSetup AppendMethod(Func<object> provider, MethodInfo method)
        {
            var apiMethodInfo = new ApiMethodInfo(provider, method);
            var apiMethodSetup = new ApiMethodSetup(this, apiMethodInfo);

            _apiMethodInfos.Add(apiMethodInfo);
            return apiMethodSetup;
        }
    }
}