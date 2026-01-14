using Saga.Orchestrator.HttpRepository;
using Saga.Orchestrator.HttpRepository.Interfaces;
using Saga.Orchestrator.Services;
using Saga.Orchestrator.Services.Interfaces;

namespace Saga.Orchestrator.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigServices(this IServiceCollection services)
        {
            services.AddTransient<ICheckoutSagaService, CheckoutSagaService>();
        }

        public static void ConfigHttpRepository(this IServiceCollection services)
        {
            services.AddScoped<IBasketHttpRepository, BasketHttpRepository>();
            services.AddScoped<IOrderHttpRepository, OrderHttpRepository>();
            services.AddScoped<IInventoryHttpRepository, InventoryHttpRepository>();
        }

        public static void ConfigHttpClient(this IServiceCollection services)
        {
            ConfigOrderHttpClient(services);
            ConfigBasketHttpClient(services);
            ConfigInventoryHttpClient(services);
        }

        public static void ConfigOrderHttpClient(this IServiceCollection services) 
        { 
            services.AddHttpClient<IOrderHttpRepository,  OrderHttpRepository>("OrdersAPI", (provider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:5005/api/v1/");
            });

            services.AddScoped(x => x.GetService<IHttpClientFactory>().CreateClient("OrdersAPI"));
        }

        public static void ConfigBasketHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IBasketHttpRepository, BasketHttpRepository>("BasketsAPI", (provider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:5004/api/");
            });

            services.AddScoped(x => x.GetService<IHttpClientFactory>().CreateClient("BasketsAPI"));
        }

        public static void ConfigInventoryHttpClient(this IServiceCollection services)
        {
            services.AddHttpClient<IInventoryHttpRepository, InventoryHttpRepository>("InventorysAPI", (provider, httpClient) =>
            {
                httpClient.BaseAddress = new Uri("http://localhost:5006/api/");
            });

            services.AddScoped(x => x.GetService<IHttpClientFactory>().CreateClient("InventorysAPI"));
        }

    }
}
