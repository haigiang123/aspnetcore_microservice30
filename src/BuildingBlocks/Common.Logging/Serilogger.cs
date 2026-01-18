using Serilog;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class Serilogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, config) =>
        {
            var applicationName = context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-");
            var environmentName = context.HostingEnvironment.EnvironmentName ?? "Development";
            var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            var username = context.Configuration.GetValue<string>("ElasticConfiguration:Username");
            var password = context.Configuration.GetValue<string>("ElasticConfiguration:Password");

            config.WriteTo.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
            {
                IndexFormat = $"MSlogs-{applicationName}-{environmentName}-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true,
                NumberOfReplicas = 1,
                NumberOfShards = 2,
                ModifyConnectionSettings = x => x.BasicAuthentication(username, password)
            })
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Environment", environmentName)
            .Enrich.WithProperty("Application", applicationName)
            .ReadFrom.Configuration(context.Configuration);
        };
    }
}