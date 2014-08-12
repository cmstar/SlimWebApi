using System.Reflection;
using cmstar.WebApi.Slim;

namespace cmstar.WebApi
{
    /// <summary>
    /// Example 2：
    /// 此示例演示通过反射进行API注册，包括属性、私有方法和批量注册。
    /// </summary>
    public class ReflectionExample : SlimApiHttpHandler
    {
        public override void Setup(ApiSetup setup)
        {
            // 使用反射的方式批量获取MethodInfo并用于方法注册
            var serviceInstance = new SimpleServiceProvider();
            var methods = serviceInstance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var methodInfo in methods)
            {
                setup.Method(serviceInstance, methodInfo);
            }

            // 反射私有方法
            var privateMethod = serviceInstance.GetType().GetMethod("Random", BindingFlags.NonPublic | BindingFlags.Instance);
            setup.Method(null, privateMethod);

            // 反射静态方法
            var staticMethods = typeof(SimpleServiceProvider).GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var methodInfo in staticMethods)
            {
                setup.Method(null, methodInfo);
            }

            // 反射属性
            var propInfo = serviceInstance.GetType().GetProperty("PropValue");
            setup.Method(serviceInstance, propInfo.GetGetMethod()).Name("GetPropValue");
            setup.Method(serviceInstance, propInfo.GetSetMethod()).Name("SetPropValue");
        }
    }
}