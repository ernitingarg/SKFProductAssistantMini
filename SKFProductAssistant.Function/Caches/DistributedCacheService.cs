using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SKFProductAssistant.Function.Codecs;

namespace SKFProductAssistant.Function.Caches
{
    /// <inheritdoc cref="IDistributedCacheService"/>
    public class DistributedCacheService : IDistributedCacheService
    {
        readonly ICodec _codec;
        readonly IDistributedCache _cache;
        readonly ILogger<DistributedCacheService> _logger;
        readonly DistributedCacheEntryOptions _options;

        public DistributedCacheService(
            ICodec codec,
            IDistributedCache cache,
            ILogger<DistributedCacheService> logger,
            DistributedCacheEntryOptions options = null)
        {
            _codec = codec ?? throw new ArgumentNullException(nameof(codec));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // default ttl
            _options = options ?? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            };
        }

        /// <inheritdoc cref="IDistributedCacheService.GetAsync{T}"/>
        public async Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning(
                    "Attempted to fetch cache with an empty or null key.");
                return default;
            }

            try
            {
                string value = await _cache.GetStringAsync(key);
                if (value == null)
                {
                    _logger.LogInformation($"Cache miss for key [{key}].");
                    return default;
                }

                _logger.LogInformation($"Cache hit for key [{key}].");
                return _codec.Decode<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while retrieving from cache.");
            }

            return default;
        }

        /// <inheritdoc cref="IDistributedCacheService.SetAsync{T}"/>
        public async Task SetAsync<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning(
                    "Attempted to set cache with an empty or null key.");
                return;
            }

            if (value == null)
            {
                _logger.LogWarning(
                    $"Skipping cache set as value is null for key [{key}].");
                return;
            }

            try
            {
                _logger.LogDebug($"Saving data for key [{key}].");

                string serializeData = _codec.Encode(value);
                await _cache.SetStringAsync(key, serializeData, _options);

                _logger.LogInformation($"Successfully cached data for key [{key}].");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while saving into cache.");
            }
        }

        /// <inheritdoc cref="IDistributedCacheService.RemoveAsync"/>
        public async Task RemoveAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning(
                    "Attempted to remove cache with an empty or null key.");
                return;
            }

            try
            {
                _logger.LogDebug($"Removing cached data for key [{key}].");

                await _cache.RemoveAsync(key);
                _logger.LogInformation(
                    $"Successfully removed cache data for key [{key}].");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while removing from cache.");
            }
        }
    }
}
