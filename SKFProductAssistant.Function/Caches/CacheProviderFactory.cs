using Microsoft.Extensions.Configuration;
using SKFProductAssistant.Function.Caches.Providers;
using System;

namespace SKFProductAssistant.Function.Caches
{
    public static class CacheProviderFactory
    {
        public static ICacheProvider GetCacheProvider(IConfiguration configuration)
        {
            string cacheProvider = configuration["CacheProvider"]?.ToLower() 
                                   ?? "memory";

            return cacheProvider switch
            {
                "redis" => new RedisCacheProvider(),
                "memory" => new MemoryCacheProvider(),
                _ => throw new InvalidOperationException(
                    $"Invalid cache provider: {cacheProvider}")
            };
        }
    }
}
