using AutoMapper;
using Infrastructure.Extensions;
using MongoDB.Driver;

namespace Inventory.Product.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddConfigurationSettings(this IServiceCollection services, IConfiguration configurations) 
        {
            var databaseSettings = configurations.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();

            services.AddSingleton(databaseSettings);
        }

        private static string GetMonggoConnectionString(this IServiceCollection services)
        {
            var settings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
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
        }

        public static void ConfigureMonggoDbClient(this IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(GetMonggoConnectionString(services)))
                .AddScoped(x => x.GetService<IMongoClient>()?.StartSession());
        }
    }
}
