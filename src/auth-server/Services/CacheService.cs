using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace AuthServer.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheService(IMemoryCache memoryCache)
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public T? GetAsync<T>(string cacheKey)
        {
            memoryCache.TryGetValue(cacheKey, out var cacheValue);
            return string.IsNullOrEmpty(cacheValue?.ToString())
                ? default
                : JsonSerializer.Deserialize<T>(cacheValue.ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheValue"></param>
        public void SetAsync<T>(string cacheKey, T cacheValue)
        {
            var obj = JsonSerializer.Serialize(cacheValue);
            memoryCache.Set(cacheKey, obj);
        }
    }
}