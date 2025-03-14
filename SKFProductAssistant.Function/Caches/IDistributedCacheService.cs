using System.Threading.Tasks;

namespace SKFProductAssistant.Function.Caches
{
    /// <summary>
    /// Distributed cache service to manage cache data.
    /// </summary>
    public interface IDistributedCacheService
    {
        /// <summary>
        /// Asynchronously gets an object from the cache
        /// with the specified key.
        /// </summary>
        /// <typeparam name="T">
        /// Type of data to be retrieved.
        /// </typeparam>
        /// <param name="key">
        /// The key to get the stored data for.
        /// </param>
        /// <returns>
        /// The object from the cache.
        /// </returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Asynchronously sets an object in the cache
        /// with the specified key.
        /// </summary>
        /// <typeparam name="T">
        /// Type of data to be cached.
        /// </typeparam>
        /// <param name="key">
        /// The key to store the data in.
        /// </param>
        /// <param name="value">
        /// The data to store in the cache.
        /// </param>
        Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Asynchronously removes the value with the given key.
        /// </summary>
        /// <param name="key">
        /// The key to remove the data for.
        /// </param>
        Task RemoveAsync(string key);
    }
}
