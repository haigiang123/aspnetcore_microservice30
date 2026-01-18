using Infrastructure.Extensions;
using Inventory.API.Services;
using Inventory.API.Services.Interfaces;
using Shared.Configurations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Product.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configurations) 
        {
            var databaseSettings = configurations.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            services.AddSingleton(databaseSettings);
        }

        private static string GetMonggoConnectionString(this IServiceCollection services)
        {
            var settings = services.GetOptions<MongoDbSettings>(nameof(MongoDbSettings));
            if(settings == null || string.IsNullOrEmpty(settings.ConnectionString))
            {
                throw new ArgumentNullException("DatabaseSettings is not configured");
            }

            var databaseName = settings.DatabaseName;
            var mongoDbConnectionString = settings.ConnectionString + "/" + databaseName + "?authSource=admin";

            return mongoDbConnectionString;
        }

        public static void AddInfrastrutureServices(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
            services.AddScoped<IInventoryEntryService, InventoryEntryService>();
        }

        public static void ConfigureMonggoDbClient(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(GetMonggoConnectionString(services)))
                .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
        }

        public static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
            services.AddHealthChecks().AddMongoDb(mongodbConnectionString: databaseSettings.ConnectionString,
                    name: "Inventory MongoDb Health",
                    failureStatus: HealthStatus.Degraded);
        }
    }
}
