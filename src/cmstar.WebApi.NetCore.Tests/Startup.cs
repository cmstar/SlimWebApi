using cmstar.WebApi.Routing;
using Common.Logging;
using Common.Logging.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace cmstar.WebApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // 设置为开发环境。
                ApiEnvironment.IsDevelopment = true;
            }
            
            LogManager.Configure(new LogConfiguration
            {
                FactoryAdapter = new FactoryAdapterConfiguration
                {
                    Type = typeof(LocalLoggerFactory).AssemblyQualifiedName,
                    Arguments = new NameValueCollection()
                }
            });
            
            app.MapApiRoute<SlimApiExample>("api/auto/{~method}/");
            app.MapApiRoute<SlimApiExample>("api/auto2");

            app.Run(async context => await context.Response.WriteAsync("hello"));
        }
    }
}
