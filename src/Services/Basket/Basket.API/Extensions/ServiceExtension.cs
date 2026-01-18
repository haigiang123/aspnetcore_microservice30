using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Shared.Configurations;
using System.Runtime;
using Infrastructure.Extensions;
using Basket.API.Services.Interfaces;
using Basket.API.Services;
using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Policies;

namespace Basket.API.Extensions
{
    public static class ServiceExtension
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var eventBusSettings = configuration.GetSection(nameof(EventBusSettings))
                .Get<EventBusSettings>();
            services.AddSingleton(eventBusSettings);

            var cacheSettings = configuration.GetSection(nameof(CacheSettings))
                .Get<CacheSettings>();
            services.AddSingleton(cacheSettings);

            var backgroundJobSettings = configuration.GetSection(nameof(BackgroundJobSettings))
                .Get<BackgroundJobSettings>();
            services.AddSingleton(backgroundJobSettings);

            return services;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            return services.AddScoped<IBasketRepository, BasketRepository>()
                .AddTransient<ISerializeService, SerializeService>()
                .AddTransient<IEmailTemplateService, BasketEmailTemplateService>()
                .AddTransient<LoggingDelegatingHandler>();
        }

        public static void ConfigureHttpClientService(this IServiceCollection services)
        {
            services.AddHttpClient<BackgroundJobHttpService>()
                .AddHttpMessageHandler<LoggingDelegatingHandler>()
                .UseLinerHttpRetryPolling(3)
                .UseCircuitBreakerPolicy()
                .ConfigureTimeoutPolicy();
        }

        public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = services.GetOptions<CacheSettings>(nameof(CacheSettings)).ConnectionString;
            //configuration.configuration.GetSection("CacheSettings:ConnectionString").Value;
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
