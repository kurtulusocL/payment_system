using System.Linq.Expressions;

namespace PaymentSystem.Infrastructure.GenericRepository
{
    public interface IAzureService
    {
        Task<IQueryable<T>> GetAllFromAzureAsync<T>(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes) where T : class;
        Task<T> GetFromAzureWithIncludesAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class;
        Task<T> GetFromAzureAsync<T>(object id) where T : class;
        T GetFromAzureWithIncludes<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes) where T : class;
    }
}