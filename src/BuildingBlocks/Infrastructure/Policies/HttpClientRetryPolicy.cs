using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Infrastructure.Policies
{
    public static class HttpClientRetryPolicy
    {
        /// <summary>
        /// Allow immediately retrying sibmitting another request once the previous one failed
        /// </summary>
        /// <param name="eventsAllowedBeforeBreaking"></param>
        /// <param name="fromSeconds"></param>
        /// <returns></returns>
        public static IHttpClientBuilder UseImmediateHttpRetryPolling(this IHttpClientBuilder  httpBuilder, int retryCount)
        {
            return httpBuilder.AddPolicyHandler(ConfigureImmediateHttpRetry(retryCount));
        }

        /// <summary>
        /// Allow immediately retrying sibmitting another request once the previous one failed in a particular timespan
        /// </summary>
        /// <param name="eventsAllowedBeforeBreaking"></param>
        /// <param name="fromSeconds"></param>
        /// <returns></returns>
        public static IHttpClientBuilder UseLinerHttpRetryPolling(this IHttpClientBuilder httpBuilder, int retryCount)
        {
            return httpBuilder.AddPolicyHandler(ConfigureLinearHttpRetry(retryCount));
        }

        public static IHttpClientBuilder UseExponencialHttpRetryPolling(this IHttpClientBuilder httpBuilder, int retryCount)
        {
            return httpBuilder.AddPolicyHandler(ConfigureExponentialHttpRetry(retryCount));
        }

        /// <summary>
        /// Dont allow creating a HttpClient request in 60s after 3 request failed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="eventsAllowedBeforeBreaking">3</param>
        /// <param name="fromSeconds">60</param>
        /// <returns></returns>
        public static IHttpClientBuilder UseCircuitBreakerPolicy(this IHttpClientBuilder builder,
        int eventsAllowedBeforeBreaking = 3, int fromSeconds = 60)
        {
            return builder.AddPolicyHandler(ConfigureCircuitBreakerPolicy(eventsAllowedBeforeBreaking, fromSeconds));
        }

        /// <summary>
        /// Requests will be timeout once exceeding 5 seconds
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="seconds">5</param>
        /// <returns></returns>
        public static IHttpClientBuilder ConfigureTimeoutPolicy(this IHttpClientBuilder builder, int seconds = 5)
        {
            return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(seconds));
        }

        private static IAsyncPolicy<HttpResponseMessage> ConfigureImmediateHttpRetry(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .RetryAsync(retryCount, (exception, retryCount, context) =>
                {
                    Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                              $"{context.OperationKey}, due to: {exception.Exception.Message}");
                });
        }
        
        private static IAsyncPolicy<HttpResponseMessage> ConfigureCircuitBreakerPolicy(int eventsAllowedBeforeBreaking,
        int fromSeconds)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .CircuitBreakerAsync(
                    eventsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(fromSeconds)
                );
        }

        private static IAsyncPolicy<HttpResponseMessage> ConfigureLinearHttpRetry(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(3),
                    (exception, retryCount, context) =>
                    {
                        Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                                  $"{context.OperationKey}, due to: {exception.Exception.Message}");
                    });
        }


        private static IAsyncPolicy<HttpResponseMessage> ConfigureExponentialHttpRetry(int retryCount)
        {
            // In this case will wait for
            //  2 ^ 1 = 2 seconds then
            //  2 ^ 2 = 4 seconds then
            //  2 ^ 3 = 8 seconds then
            //  2 ^ 4 = 16 seconds then
            //  2 ^ 5 = 32 seconds
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .WaitAndRetryAsync(retryCount, retryAttempt
                        => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, retryCount, context) =>
                    {
                        Log.Error($"Retry {retryCount} of {context.PolicyKey} at " +
                                  $"{context.OperationKey}, due to: {exception.Exception.Message}");
                    });
        }
    }
}
