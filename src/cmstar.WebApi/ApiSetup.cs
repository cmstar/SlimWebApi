using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using cmstar.RapidReflection.Emit;

namespace cmstar.WebApi
{
    /// <summary>
    /// �ṩAPIע��ı����ڡ�
    /// </summary>
    public partial class ApiSetup
    {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

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
            var methods = GetMethods(typeof(TProvider), bindingFlags);
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
            var methods = GetMethods(typeof(TProvider), bindingFlags);
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

            var methods = GetMethods(providerType, bindingFlags);
            var methodSetups = AppendMethods(methods, providerCreator, parseAttribute);
            return methodSetups;
        }

        private MethodInfo[] GetMethods(Type type, BindingFlags bingBindingFlags)
        {
            var methods = type.GetMethods(bingBindingFlags);

            // Ĭ��ȥ�� .net �ײ������ķ��������ȷʵ��Ҫ��Щ������ֻ��һ��һ��ע�ᣬ����ͨ���Զ�ע�ᡣ
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

                    // ApiMethodAttribute�ǿ��Ա���ע��εģ���ʱ��һ����������ΪAPI���
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