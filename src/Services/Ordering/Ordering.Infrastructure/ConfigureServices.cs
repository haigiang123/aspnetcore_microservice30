using Contracts.Common.Interfaces;
using Contracts.Services;
using Infrastructure.Common;
using Infrastructure.Extensions;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Common.Interfaces;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using Shared.Configurations;

namespace Ordering.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config) 
        {
            var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));

            services.AddDbContext<OrderContext>(options =>
            {
                options.UseSqlServer(databaseSettings.ConnectionString,
                    builder => builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
            });

            services.AddScoped<OrderContextSeed>();

            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped(typeof(ISmtpEmailService), typeof(SmtpEmailService));

            return services;
        }
    }
}
