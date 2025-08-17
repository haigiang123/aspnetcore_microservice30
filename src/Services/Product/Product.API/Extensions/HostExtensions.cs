using Microsoft.EntityFrameworkCore;

namespace Product.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<T>(this IHost host, Action<T, IServiceProvider> seeder) 
            where T : DbContext
        {
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<T>>();
                var context = services.GetRequiredService<T>();

                try
                {
                    logger.LogInformation("Mygrating mysql database");
                    ExecuteMigrations(context);
                    logger.LogInformation("Mygrated mysql database");
                    InvokeSeeder(seeder, context, services);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the mysql database");
                }

            }

            return host;
        }

        private static void ExecuteMigrations<T>(T context)
            where T : DbContext
        {
            context.Database.Migrate();
        }

        private static void InvokeSeeder<T>(Action<T, IServiceProvider> seeder, T context, IServiceProvider provider)
            where T : DbContext
        {
            seeder(context, provider);
        }

    }
}
