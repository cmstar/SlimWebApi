using System;
using System.Collections.Generic;
using System.IO;
using cmstar.WebApi.Slim;

namespace cmstar.WebApi
{
    /// <summary>
    /// Example 1：
    /// 此示例演示通过委托的方式进行API注册及进行方法重命名。
    /// 包括单例、非单例模式，实例、静态方法及匿名方法的注册。
    /// </summary>
    public class DelegateExample : SlimApiHttpHandler
    {
        public override void Setup(ApiSetup setup)
        {
            // 使用单例方式注册API方法
            var serviceInstance = new SimpleServiceProvider();

            // 注册无返回值的方法
            setup.Method((Action<DateTime, DateTime?>)serviceInstance.DoNothingWith);
            setup.Method((Action<int>)serviceInstance.Error);

            // 注册有返回值的方法
            setup.Method((Func<int, int, int>)serviceInstance.PlusRandom);

            // 可以注册一个方法多次，调用时方法名称将变为 PlusRandom2, PlusRandom3, ... 依此类推
            setup.Method((Func<int, int, int>)serviceInstance.PlusRandom);

            // 也可以直接重命名API方法的调用名称
            setup.Method((Func<SimpleObject, SimpleObject>)serviceInstance.GetSelf).Name("Self");

            // 使用带入输入流的方法
            setup.Method((Func<string, Stream, string, string>)serviceInstance.InputStream);

            // 注册静态方法
            setup.Method((Func<IList<double>, double>)SimpleServiceProvider.Sum);

            // 使用非单例模式注册API方法
            Func<SimpleServiceProvider> serviceProvider = () => new SimpleServiceProvider();
            setup.Method(serviceProvider, x => (Func<Guid>)x.GetGuid);

            // 也可以使用lambda表达式注册静态方法，但不推荐
            setup.Method((Func<object>)null, x => (Func<string, string, string>)string.Concat);

            // 注册匿名方法（并取个名字）
            Func<int, int, int> plus = (x, y) => x + y;
            setup.Method(plus).Name("Plus");

            // 可以用匿名方法的方式对属性进行取值赋值
            Func<string> getPropValue = () => serviceInstance.PropValue;
            Action<string> setPropValue = v => serviceInstance.PropValue = v;
            setup.Method(getPropValue).Name("GetPropValue");
            setup.Method(setPropValue).Name("SetPropValue");
        }
    }
}