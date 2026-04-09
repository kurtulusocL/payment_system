using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentSystem.Application.Constants.Services.Abstract;
using PaymentSystem.Infrastructure.Data.Context.Azure;

namespace PaymentSystem.Infrastructure.GenericRepository.Azure
{
    public class AzureBaseService : IAzureService
    {
        private readonly string _azureConnectionString;
        private readonly IEncryptionService _encryptionService;
        private readonly ICacheService _cacheService;
        private const int CacheMinutes = 30;

        public AzureBaseService(IConfiguration configuration, IEncryptionService encryptionService, ICacheService cacheService)
        {
            _azureConnectionString = configuration.GetConnectionString("AzureConnection");
            _encryptionService = encryptionService;
            _cacheService = cacheService;
        }

        private string BuildCacheKey<T>(string prefix, Expression<Func<T, bool>> filter = null)
        {
            return $"azure:{typeof(T).Name}:{prefix}:{filter?.ToString() ?? "all"}";
        }

        private DbContextOptions<AzureDbContext> BuildOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AzureDbContext>();
            optionsBuilder.UseSqlServer(_azureConnectionString);
            return optionsBuilder.Options;
        }

        public async Task<IQueryable<T>> GetAllFromAzureAsync<T>(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes) where T : class
        {
            var cacheKey = BuildCacheKey<T>("list", filter);

            var cached = _cacheService.Get<List<T>>(cacheKey);
            if (cached != null) return cached.AsQueryable();

            using var azureContext = new AzureDbContext(BuildOptions(), _encryptionService);
            IQueryable<T> query = azureContext.Set<T>();
            if (filter != null) query = query.Where(filter);
            foreach (var include in includes)
                query = query.Include(include);

            var result = await query.ToListAsync();
            if (result.Any())
                _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

            return result.AsQueryable();
        }

        public async Task<T> GetFromAzureWithIncludesAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class
        {
            var cacheKey = BuildCacheKey<T>("single", filter);

            var cached = _cacheService.Get<T>(cacheKey);
            if (cached != null) return cached;

            using var azureContext = new AzureDbContext(BuildOptions(), _encryptionService);
            IQueryable<T> query = azureContext.Set<T>();
            foreach (var include in includes)
                query = query.Include(include);

            var result = await query.FirstOrDefaultAsync(filter);
            if (result != null)
                _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

            return result;
        }

        public async Task<T> GetFromAzureAsync<T>(object id) where T : class
        {
            var cacheKey = $"azure:{typeof(T).Name}:id:{id}";

            var cached = _cacheService.Get<T>(cacheKey);
            if (cached != null) return cached;

            using var azureContext = new AzureDbContext(BuildOptions(), _encryptionService);
            var result = await azureContext.Set<T>().FindAsync(id);
            if (result != null)
                _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

            return result;
        }

        public T GetFromAzureWithIncludes<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class
        {
            var cacheKey = BuildCacheKey<T>("single-sync", filter);

            var cached = _cacheService.Get<T>(cacheKey);
            if (cached != null) return cached;

            using var azureContext = new AzureDbContext(BuildOptions(), _encryptionService);
            IQueryable<T> query = azureContext.Set<T>();
            foreach (var include in includes)
                query = query.Include(include);

            var result = query.FirstOrDefault(filter);
            if (result != null)
                _cacheService.Set(cacheKey, result, TimeSpan.FromMinutes(CacheMinutes));

            return result;
        }
    }
}
