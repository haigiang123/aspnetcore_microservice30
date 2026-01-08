using Ocelot;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using System.Configuration;

namespace OcelotApiGw.Extensions
{
    public static class ServicesExtension
    {
        public static void AddServiceExtentions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot().AddPolly().AddCacheManager(x => x.WithDictionaryHandle());
            services.AddSwaggerForOcelot(configuration, x => { x.GenerateDocsForGatewayItSelf = false; });
        }
    }
}
