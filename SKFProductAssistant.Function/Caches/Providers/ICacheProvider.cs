using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SKFProductAssistant.Function.Caches.Providers
{
    /// <summary>
    /// Interface for configuring and setting up a cache provider.
    /// </summary>
    public interface ICacheProvider
    {
        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}
