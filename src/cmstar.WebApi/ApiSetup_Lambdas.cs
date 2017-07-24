using System;
using System.Linq.Expressions;
using System.Reflection;
#if NET35
using cmstar.WebApi.NetFuture;
#endif

namespace cmstar.WebApi
{
    public partial class ApiSetup
    {
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

            var methodCallExpression = (MethodCallExpression)body;

            /* 
             * http://stackoverflow.com/questions/8225302/get-the-name-of-a-method-using-an-expression
             * C# 5.0 compiler does behave differently depending on the target framework, for .Net 4.0,
             * it generates Call to Delegate.CreateDelegate, while for .Net 4.5, it's the new
             * MethodInfo.CreateDelegate
             */
#if NET35 || NET40
            // exp: CreateDelegate(methodSignature, instance, methodInfo)
            var methodConstantExpression = (ConstantExpression)methodCallExpression.Arguments[2];
            var methodInfo = (MethodInfo)methodConstantExpression.Value;
#else
            var constantExpression = (ConstantExpression)methodCallExpression.Object;
            var methodInfo = (MethodInfo)constantExpression.Value;
#endif

            return methodInfo;
        }
    }
}
