using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using cmstar.RapidReflection.Emit;

namespace cmstar.WebApi
{
    /// <summary>
    /// �ṩAPIע��ı����ڡ�
    /// </summary>
    public class ApiSetup
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private readonly List<ApiMethodInfo> _apiMethodInfos;
        private readonly LogSetup _logSetup;

        /// <summary>
        /// ��ʼ��<see cref="ApiSetup"/>����ʵ����
        /// </summary>
        /// <param name="callerType">����APIע������͡�</param>
        public ApiSetup(Type callerType)
        {
            ArgAssert.NotNull(callerType, "callerType");

            _apiMethodInfos = new List<ApiMethodInfo>();
            _logSetup = LogSetup.Default();
            CallerType = callerType;
        }

        /// <summary>
        /// ��ȡ����APIע������͡�
        /// </summary>
        public Type CallerType { get; private set; }

        /// <summary>
        /// ��ȡAPI�����ṩ������API����ע����û�е���ָ�������ṩ���������ô˻����ṩ����
        /// </summary>
        public IApiCacheProvider CacheProvider { get; private set; }

        /// <summary>
        /// ��ȡ����ĳ�ʱʱ�䡣��API����ע����û�е���ָ����ʱʱ�䣬�����ô˳�ʱʱ�䡣
        /// </summary>
        public TimeSpan CacheExpiration { get; private set; }

        /// <summary>
        /// ��ȡ��־��ص�������ڡ�
        /// </summary>
        public LogSetup Log
        {
            get { return _logSetup; }
        }

        /// <summary>
        /// ��ȡ�ڵ�ǰʵ��ע�������API����ע����Ϣ�����С�
        /// </summary>
        public IEnumerable<ApiMethodInfo> ApiMethodInfos
        {
            get { return _apiMethodInfos; }
        }

        /// <summary>
        /// ���ôӵ�ǰʵ��ע���API��ʹ�õĻ����ṩ����
        /// ��API����ע����û�е���ָ�������ṩ���������ô��ṩ����
        /// </summary>
        /// <param name="provider">API�����ṩ����</param>
        /// <param name="expiration">����ĳ�ʱʱ�䡣</param>
        public void SetupCacheBase(IApiCacheProvider provider, TimeSpan expiration)
        {
            if (expiration.Ticks <= 0)
                throw new ArgumentException("The expiration must be greater than zero.", "expiration");

            CacheProvider = provider;
            CacheExpiration = expiration;
        }

        /// <summary>
        /// ���һ��API����ע�ᡣ
        /// </summary>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="method">��ע��ΪWebAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method(Func<object> provider, MethodInfo method)
        {
            return AppendMethod(provider, method);
        }

        /// <summary>
        /// ���һ��API����ע�ᡣ
        /// </summary>
        /// <param name="provider">
        /// �ṩAPI�߼�ʵ�ֵ�����ʵ����������Ϊ��̬������ʹ��<c>null</c>��
        /// </param>
        /// <param name="method">��ע��ΪWebAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method(object provider, MethodInfo method)
        {
            return AppendMethod(provider, method);
        }

        /// <summary>
        /// ע�����0���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TResult>(Func<TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����1���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, TResult>(Func<T1, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����2���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, TResult>(Func<T1, T2, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����3���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����4���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����5���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����6���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����7���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����8���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <typeparam name="T8">�����е�8�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����0��������û�з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method(Action method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����1���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1>(Action<T1> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����2���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2>(Action<T1, T2> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����3���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3>(Action<T1, T2, T3> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����4���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����5���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����6���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����7���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7>(Action<T1, T2, T3, T4, T5, T6, T7> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// ע�����8���������з���ֵ�ķ�����APIע����Ϣ�С�
        /// </summary>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <typeparam name="T8">�����е�8�����������͡�</typeparam>
        /// <param name="method">ע��ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1, T2, T3, T4, T5, T6, T7, T8> method)
        {
            return AppendMethod(method);
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, T6, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, T6, T7, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="TResult">��������ֵ�����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <typeparam name="T8">�����е�8�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Func<TProvider> provider,
            Expression<Func<TProvider, Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider>(Func<TProvider> provider,
            Expression<Func<TProvider, Action>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5, T6>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5, T6, T7>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// �Էǵ����ķ�ʽ���APIע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI�߼�ʵ�ֵ����͡�</typeparam>
        /// <typeparam name="T1">�����е�1�����������͡�</typeparam>
        /// <typeparam name="T2">�����е�2�����������͡�</typeparam>
        /// <typeparam name="T3">�����е�3�����������͡�</typeparam>
        /// <typeparam name="T4">�����е�4�����������͡�</typeparam>
        /// <typeparam name="T5">�����е�5�����������͡�</typeparam>
        /// <typeparam name="T6">�����е�6�����������͡�</typeparam>
        /// <typeparam name="T7">�����е�7�����������͡�</typeparam>
        /// <typeparam name="T8">�����е�8�����������͡�</typeparam>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="methodSelector">����Ҫע��ΪAPI�ķ�����</param>
        /// <returns><see cref="ApiMethodSetup"/>��ʵ����</returns>
        public ApiMethodSetup Method<TProvider, T1, T2, T3, T4, T5, T6, T7, T8>(Func<TProvider> provider,
            Expression<Func<TProvider, Action<T1, T2, T3, T4, T5, T6, T7, T8>>> methodSelector)
        {
            return AppendMethod(() => provider(), ResolveMethodInfo(methodSelector));
        }

        /// <summary>
        /// ��ָ�����͵�ʵ������API������ע�ᡣ
        /// </summary>
        /// <typeparam name="TProvider">�ṩAPI���������͡�</typeparam>
        /// <param name="provider">�ṩAPI������ʵ����</param>
        /// <param name="parseAttribute">
        /// ��Ϊtrue��������ر����<see cref="ApiMethodAttribute"/>�ķ���������������з�����
        /// ������ɸѡ��<paramref name="bindingFlags"/>Ӱ�졣
        /// </param>
        /// <param name="bindingFlags">ָ�������Ĺ��˷�ʽ��Ĭ�ϼ������й���������</param>
        /// <returns>���ر�ע���API������ע����Ϣ��</returns>
        public IEnumerable<ApiMethodSetup> Auto<TProvider>(
            TProvider provider, bool parseAttribute = true, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            var methods = typeof(TProvider).GetMethods(bindingFlags);
            return AppendMethods(methods, m => (() => provider), parseAttribute);
        }

        /// <summary>
        /// ��ָ�����ͼ���API������ע�ᡣ
        /// </summary>
        /// <param name="provider">����һ�����󣬸ö���Ϊ�ṩAPI�߼�ʵ�ֵ�����ʵ����</param>
        /// <param name="parseAttribute">
        /// ��Ϊtrue��������ر����<see cref="ApiMethodAttribute"/>�ķ���������������з�����
        /// ������ɸѡ��<paramref name="bindingFlags"/>Ӱ�졣
        /// </param>
        /// <param name="bindingFlags">ָ�������Ĺ��˷�ʽ��Ĭ�ϼ������й���������</param>
        /// <returns>���ر�ע���API������ע����Ϣ��</returns>
        public IEnumerable<ApiMethodSetup> Auto<TProvider>(
            Func<TProvider> provider, bool parseAttribute = true, BindingFlags bindingFlags = DefaultBindingFlags)
        {
            var methods = typeof(TProvider).GetMethods(bindingFlags);
            return AppendMethods(methods, m => (() => provider()), parseAttribute);
        }

        /// <summary>
        /// ��ָ�����ͼ���API������ע�ᡣ����ʹ�ô˷������ؾ�̬/�������еķ�����
        /// ������ʵ�������������ͱ�����һ���޲εĹ��캯����
        /// </summary>
        /// <param name="providerType">�ṩAPI���������͡�</param>
        /// <param name="singleton">true����API�ṩ������ʹ�õ���ģʽ������Ϊfalse��Ĭ��Ϊtrue��</param>
        /// <param name="parseAttribute">
        /// ��Ϊtrue��������ر����<see cref="ApiMethodAttribute"/>�ķ���������������з�����
        /// ������ɸѡ��<paramref name="bindingFlags"/>Ӱ�졣
        /// </param>
        /// <param name="bindingFlags">ָ�������Ĺ��˷�ʽ��Ĭ�ϼ������й���������</param>
        /// <returns>���ر�ע���API������ע����Ϣ��</returns>
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

            var methods = providerType.GetMethods(bindingFlags);
            var methodSetups = AppendMethods(methods, providerCreator, parseAttribute);
            return methodSetups;
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

                    // ApiMethodAttribute�ǿ��Ա���ע��εģ���ʱ��һ����������ΪAPI���
                    foreach (ApiMethodAttribute apiMethodAttr in attrs)
                    {
                        var provider = methodInfo.IsStatic ? null : providerCreator(methodInfo);
                        var apiSetupInfo = AppendMethod(provider, methodInfo);

                        if (!string.IsNullOrEmpty(apiMethodAttr.Name))
                        {
                            apiSetupInfo.Name(apiMethodAttr.Name);
                        }

                        if (apiMethodAttr.CacheExpiration > 0)
                        {
                            apiSetupInfo.CacheExpiration(TimeSpan.FromSeconds(apiMethodAttr.CacheExpiration));
                        }

                        if (!string.IsNullOrEmpty(apiMethodAttr.CacheNamespace))
                        {
                            apiSetupInfo.CacheNamespace(apiMethodAttr.CacheNamespace);
                        }

                        if (apiMethodAttr.AutoCacheEnabled)
                        {
                            apiSetupInfo.EnableAutoCache();
                        }

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