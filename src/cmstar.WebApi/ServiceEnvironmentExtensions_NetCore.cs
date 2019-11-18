#if NETCORE
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace cmstar.WebApi
{
    public static class ServiceEnvironmentExtensions
    {
        /// <summary>
        /// 添加 WebAPI 框架所需的依赖项。
        /// </summary>
        public static IServiceCollection AddSlimApi(this IServiceCollection services, IHostingEnvironment env = null)
        {
            if (env != null && env.IsDevelopment())
            {
                ApiEnvironment.IsDevelopment = true;
            }

            services.AddRouting();

            // 目前暂未引入其他依赖项。
            return services;
        }
    }
}
#endif