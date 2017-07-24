using System;
#if NET35
using cmstar.WebApi.NetFuture;
#endif

namespace cmstar.WebApi
{
    public partial class ApiSetup
    {
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

        private ApiMethodSetup AppendMethod(Delegate method)
        {
            ArgAssert.NotNull(method, "method");
            return AppendMethod(method.Target, method.Method);
        }
    }
}
