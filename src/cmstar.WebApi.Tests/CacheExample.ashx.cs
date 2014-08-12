using System;
using cmstar.WebApi.Slim;

namespace cmstar.WebApi
{
    /// <summary>
    /// Example 3：
    /// 此示例演示缓存的配置。
    /// </summary>
    public class CacheExample : SlimApiHttpHandler
    {
        public override void Setup(ApiSetup setup)
        {
            // 设置缓存提供器和缓存超时时间
            // 每个API方法都可以单独指定他们。但在没有指定时，若开启自动缓存，则套用这里的设置
            setup.SetupCacheBase(new HttpRuntimeApiCacheProvider(), TimeSpan.FromSeconds(10));

            // 开启方法的自动缓存。没有单独指定缓存的超时时间，将套用上面的值（10秒）
            var serviceInstance = new SimpleServiceProvider();
            setup.Method((Func<int, int, int>)serviceInstance.PlusRandom)
                .EnableAutoCache();

            // 单独指定缓存超时时间
            Func<SimpleServiceProvider> serviceProvider = () => new SimpleServiceProvider();
            setup.Method(serviceProvider, x => (Func<Guid>)x.GetGuid)
                .CacheExpiration(TimeSpan.FromSeconds(3))
                .EnableAutoCache();

            // 单独指定缓存提供器，这里设置一个空的缓存提供器（实际上就没有缓存了）
            var random = new Random();
            setup.Method(() => random.Next(100)).Name("Random")
                .CacheProvider(new NoCacheApiCacheProvider())
                .EnableAutoCache();
        }
    }
}