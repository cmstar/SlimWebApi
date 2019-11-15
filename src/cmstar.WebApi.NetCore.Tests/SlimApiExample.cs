using System.Reflection;
using cmstar.WebApi.Slim;

namespace cmstar.WebApi
{
    public class SlimApiExample : SlimApiHttpHandler
    {
        public override void Setup(ApiSetup setup)
        {
            // 此方式等同于使用反射获取MethodInfo并批量注册，写法上更为便利
            setup.Auto(new SimpleServiceProvider(), false, BindingFlags.Public | BindingFlags.Static);

            // 注册利用ApiMethodAttribute标记的方法，该特性上已经定义了方法注册配置的相关信息
            setup.Auto(new AttributedServiceProvider());

            // 注册抽象类的静态方法
            setup.FromType(typeof(AbstractServiceProvider), parseAttribute: false);

            // 注册异步方法
            setup.FromType(typeof(AsyncServiceProvider));
        }
    }
}
