using Shared.Configurations;

namespace Customer.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
       IConfiguration configuration)
        {
            var hangFireSettings = configuration.GetSection(nameof(HangFireSettings))
                .Get<HangFireSettings>();
            services.AddSingleton(hangFireSettings);

            var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
                .Get<DatabaseSettings>();
            services.AddSingleton(databaseSettings);

            //var emailSettings = configuration.GetSection(nameof(SMTPEmailSetting))
            //    .Get<SMTPEmailSetting>();
            //services.AddSingleton(emailSettings);

            return services;
        }
    }
}
