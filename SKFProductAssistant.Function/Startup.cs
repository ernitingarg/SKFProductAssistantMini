using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SKFProductAssistant.Function;
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
            Configuration = builder.ConfigurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .Build();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
        }
    }
}
