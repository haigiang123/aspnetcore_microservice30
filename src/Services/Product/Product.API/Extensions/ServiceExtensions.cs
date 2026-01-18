using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Product.API.Persistence;
using Product.API.Repositories;
using Product.API.Repositories.Interfaces;
using Shared.Configurations;
using Infrastructure.Extensions;

namespace Product.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
        {
            var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
                .Get<DatabaseSettings>();
            if (databaseSettings is null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
                throw new ArgumentNullException("Connection string is not configured.");

            services.AddSingleton(databaseSettings);

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configurations)
        {
            services.AddControllers();
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            services.AddConfigurationSettings(configurations);
            services.AddEndpointsApiExplorer();
            services.ConfigureProductDBContext(configurations);
            services.AddSwaggerGen();
            services.AddInfrastructureServices();
            services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));
            services.ConfigureHealthChecks();

            return services;
        }

        private static IServiceCollection ConfigureProductDBContext(this IServiceCollection services, IConfiguration configurations)
        {
            var databaseSettings = configurations.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
            if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
                throw new ArgumentNullException($"{nameof(databaseSettings)} is not configured properly");

            var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);
            services.AddDbContext<ProductContext>(x => x.UseMySql(builder.ConnectionString,
                ServerVersion.AutoDetect(builder.ConnectionString), e =>
                {
                    e.MigrationsAssembly("Product.API");
                    e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                    e.CommandTimeout(30);
                }));

            return services;
        }

        private static void ConfigureHealthChecks(this IServiceCollection services)
        {
            var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
            services.AddHealthChecks().AddMySql(
                //healthQuery: "SELECT 1;",
                connectionString: databaseSettings.ConnectionString,
                name: "MySql Health",
                failureStatus: HealthStatus.Unhealthy
            );
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                        .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                        .AddScoped(typeof(IProductRepository), typeof(ProductRepository))
                        ;
        }
    }
}
