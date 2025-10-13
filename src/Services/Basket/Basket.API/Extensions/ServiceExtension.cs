using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Common;

namespace Basket.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection service)
        {
            return service.AddScoped<IBasketRepository, BasketRepository>()
                            .AddTransient<ISerializeService, SerializeService>();
        }

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("CacheSettings:ConnectionString").Value;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Redis connection is not configuted");
            }

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });
        }
    }
}
