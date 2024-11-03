using Microsoft.Extensions.Caching.Memory;
using SEOChecker.Application.Interfaces;

namespace SEOChecker.Infrastructure.Services
{
    public class MemoryCacheService : IMemoryCacheService
    {
        private readonly IMemoryCache memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public void Set<T>(string key, T value, TimeSpan expiration) => memoryCache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });

        public bool TryGetValue<T>(string key, out T? value) => memoryCache.TryGetValue(key, out value);
    }
}
