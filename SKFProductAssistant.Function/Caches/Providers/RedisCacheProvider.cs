using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SKFProductAssistant.Function.Caches.Providers
{
    /// <summary>
    /// Configures the application to use a redis distributed cache.
    /// </summary>
    public class RedisCacheProvider : ICacheProvider
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            string cacheConnection = configuration.GetConnectionString("CacheConnection");
            if (string.IsNullOrEmpty(cacheConnection))
            {
                throw new InvalidOperationException("Redis connection string is required.");
            }

            Console.WriteLine("Using Redis for distributed caching.");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheConnection;
                options.InstanceName = "RedisCacheInstance";
            });
        }
    }
}
