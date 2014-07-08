using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using cmstar.WebApi;
using cmstar.WebApi.Slim;

namespace cmstar.WebApi.Tests
{
    public class Server : SlimApiHttpHandler
    {
        public override void Setup(ApiSetup setup)
        {
            // setup cache base
            var cacheProvider = new HttpRuntimeApiCacheProvider();
            setup.SetupCacheBase(cacheProvider, TimeSpan.FromSeconds(5));

            var serviceProvider = new ServiceMethodProvider();

            // singleton methods
            setup.Method<int>(serviceProvider.SetIntValue);
            setup.Method<int>(serviceProvider.GetIntValue);
            setup.Method<PlainData, PlainData>(serviceProvider.GetSameObject);
            setup.Method<Guid>(serviceProvider.GetGuid);

            // register more than once
            setup.Method<int>(serviceProvider.SetIntValue).Name("SetInt");
            setup.Method<int>(serviceProvider.GetIntValue).Name("GetInt");

            // auto renaming
            setup.Method((Func<string, string, string>)serviceProvider.Concat); //concat
            setup.Method((Func<string, string, string, string>)serviceProvider.Concat); //concat2
            setup.Method((Func<string[], string>)serviceProvider.Concat); //concat3

            // property get
            var prop = serviceProvider.GetType().GetProperty("FloatValue");
            setup.Method(serviceProvider, prop.GetGetMethod()).Name("GetFloat");
            setup.Method(serviceProvider, prop.GetSetMethod()).Name("SetFloat");

            // non-public method
            var nonPublicMethod = serviceProvider.GetType()
                .GetMethod("NonPublicPlus", BindingFlags.NonPublic | BindingFlags.Instance);
            setup.Method(serviceProvider, nonPublicMethod);

            // static methods
            setup.Method<DateTime, DateTime>(ServiceMethodProvider.NextDay);
            setup.Method<ComplexData, object>(ServiceMethodProvider.ConvertToAnonymous);

            // test non-singleton provider
            int id = 0;
            Func<ServiceMethodProvider> ctor = () => new ServiceMethodProvider(++id);
            setup.Method(ctor, x => (Func<Guid>)x.GetGuid).Name("Guid");
            setup.Method(ctor, x => (Func<int>)x.GetId).Name("IdNoCache");
            setup.Method(ctor, x => (Func<int>)x.GetId).Name("IdCached").EnableCache();

            // methods from .net framework directly
            setup.Method((Func<string, string, string>)string.Concat).Name("NetConcat");

            // anonymous method
            Func<int, int, int> plus = (x, y) => x + y;
            setup.Method(plus).Name("fun");

            // cache
            setup.Method((Func<PlainData, Guid>)serviceProvider.Random).EnableCache();
        }
    }
}