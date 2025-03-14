using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SKFProductAssistant.Function;
using SKFProductAssistant.Function.Caches;
using SKFProductAssistant.Function.Codecs;
using SKFProductAssistant.Function.Configs;
using SKFProductAssistant.Function.OpenAIs;
using SKFProductAssistant.Function.Products;
using System;
using System.IO;

[assembly: FunctionsStartup(typeof(Startup))]

namespace SKFProductAssistant.Function
{
    public class Startup : FunctionsStartup
    {
        IConfigurationRoot Configuration { get; set; }

        public override void ConfigureAppConfiguration(
            IFunctionsConfigurationBuilder builder)
        {
            string environment =
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Configuration = builder.ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            ConfigureDependencies(builder.Services);
            ConfigureOpenApi(builder.Services);
            ConfigureDistributedCache(builder.Services);
        }

        void ConfigureDependencies(IServiceCollection services)
        {
            services.AddSingleton<ICodec, JsonCodec>();
            services.AddTransient<IProductService, ProductService>();
        }

        void ConfigureOpenApi(IServiceCollection services)
        {
            var openAiConfig = new OpenAiConfig();
            Configuration.Bind(nameof(OpenAiConfig), openAiConfig);
            openAiConfig.Validate();
            services.AddSingleton(openAiConfig);

            services.AddTransient<IOpenAiService, OpenAiService>();
        }

        void ConfigureDistributedCache(IServiceCollection services)
        {
            var cacheConfig = new CacheConfig();
            Configuration.Bind(nameof(CacheConfig), cacheConfig);
            cacheConfig.Validate();
            services.AddSingleton(cacheConfig);

            var cacheProvider = CacheProviderFactory.GetCacheProvider(Configuration);
            cacheProvider.Configure(services, Configuration);
            services.AddSingleton(cacheProvider);

            var cacheEntryOption = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheConfig.AbsoluteExpirationRelativeToNowInMinutes.HasValue
                    ? TimeSpan.FromMinutes(cacheConfig.AbsoluteExpirationRelativeToNowInMinutes.Value)
                    : null,

                SlidingExpiration = cacheConfig.SlidingExpirationInMinutes.HasValue
                    ? TimeSpan.FromMinutes(cacheConfig.SlidingExpirationInMinutes.Value)
                    : null
            };
            services.AddSingleton(cacheEntryOption);

            services.AddTransient<IDistributedCacheService, DistributedCacheService>();
        }
    }
}
