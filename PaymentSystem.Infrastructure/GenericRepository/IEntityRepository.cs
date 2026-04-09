using System.Linq.Expressions;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Infrastructure.GenericRepository
{
    public interface IEntityRepository<T> where T : class, IEntity, new()
    {
        Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task<IQueryable<T>> GetAllIncludeAsync(Expression<Func<T, bool>>[] filters, params Expression<Func<T, object>>[] includes);
        Task<IQueryable<T>> GetAllIncludeByIdAsync(object id, string foreignKeyPropertyName, Expression<Func<T, bool>>[] conditions, params Expression<Func<T, object>>[] includes);
        Task<IQueryable<T>> GetAllIncludingByPropertyPathAsync(object id, string foreignKeyPropertyPath, Expression<Func<T, bool>>[] conditions = null, params Expression<Func<T, object>>[] includes);
        Task<T> GetAsync(Expression<Func<T, bool>> filter);
        Task<T> GetIncludeAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteByIdsAsync(IEnumerable<object> ids);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null);
        IQueryable<T> GetAllInclude(Expression<Func<T, bool>>[] filters, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetAllIncludeById(object id, string foreignKeyPropertyName, Expression<Func<T, bool>>[] conditions, params Expression<Func<T, object>>[] includes);
        IQueryable<T> GetAllIncludingByPropertyPath(object id, string foreignKeyPropertyPath, Expression<Func<T, bool>>[] conditions = null, params Expression<Func<T, object>>[] includes);
        T Get(Expression<Func<T, bool>> filter);
        T GetInclude(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        Task<bool> SetDeletedAsync(object id);
        Task<bool> SetNotDeletedAsync(object id);
        Task<bool> SetActiveAsync(object id);
        Task<bool> SetDeActiveAsync(object id);
    }
}
