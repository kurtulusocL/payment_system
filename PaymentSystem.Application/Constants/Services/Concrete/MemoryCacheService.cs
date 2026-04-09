using Microsoft.Extensions.Caching.Memory;
using PaymentSystem.Application.Constants.Services.Abstract;

namespace PaymentSystem.Application.Constants.Services.Concrete
{
    public class MemoryCacheService:ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key) => _memoryCache.Get<T>(key);

        public void Set<T>(string key, T value, TimeSpan expirationTime) =>
            _memoryCache.Set(key, value, expirationTime);

        public void Remove(string key) => _memoryCache.Remove(key);

        public bool Any(string key) => _memoryCache.TryGetValue(key, out _);
    }
}
