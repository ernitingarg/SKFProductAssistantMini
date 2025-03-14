using System;

namespace SKFProductAssistant.Function.Configs
{
    public class CacheConfig
    {
        /// <summary>
        /// Gets or sets an absolute expiration time, relative to now
        /// after which entry from the cache will be removed.
        /// If null, the cache entry does not expire.
        /// </summary>
        public double? AbsoluteExpirationRelativeToNowInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the value in minutes for how long
        /// a cache entry can be inactive before removing it from the cache.
        /// If null, the cache entry does not have sliding expiration.
        /// </summary>
        public double? SlidingExpirationInMinutes { get; set; }

        /// <summary>
        /// Validates and ensures that cache expiration values
        /// are within acceptable ranges.
        /// </summary>
        public void Validate()
        {
            if (AbsoluteExpirationRelativeToNowInMinutes is <= 0)
            {
                throw new ArgumentException(
                    $"{nameof(AbsoluteExpirationRelativeToNowInMinutes)} " +
                    $"must be greater than zero.");
            }

            if (SlidingExpirationInMinutes is <= 0)
            {
                throw new ArgumentException(
                    $"{nameof(SlidingExpirationInMinutes)} " +
                    $"must be greater than zero.");
            }
        }
    }
}
