using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SKFProductAssistant.Function.Caches.Providers
{
    /// <summary>
    /// Configures the application to use an in-memory distributed cache.
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            Console.WriteLine("Using in-memory for distributed caching.");
            services.AddDistributedMemoryCache();
        }
    }
}
