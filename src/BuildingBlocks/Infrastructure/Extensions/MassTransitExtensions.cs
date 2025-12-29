using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class MassTransitExtensions
    {
        public static void ConfigureMassTransitWithRabbitMq(this IServiceCollection services)
        {
            var settings = services.GetOptions<EventBusSettings>(nameof(EventBusSettings));
            if(settings == null || string.IsNullOrEmpty(settings.HostAddress) || string.IsNullOrEmpty(settings.HostAddress))
                throw new ArgumentNullException("EventBusSettings is not configured properly!");

            var mqConnection = new Uri(settings.HostAddress);

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

            services.AddMassTransit(config =>
            {
                config.AddConsumers(Assembly.GetEntryAssembly());
                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(mqConnection);
                    cfg.ConfigureEndpoints(ctx,
                        new KebabCaseEndpointNameFormatter(settings.ServiceName, false));

                    cfg.UseMessageRetry(retryConfigurator => { retryConfigurator.Interval(3, TimeSpan.FromSeconds(5)); });
                });
            });
        }
    }
}
