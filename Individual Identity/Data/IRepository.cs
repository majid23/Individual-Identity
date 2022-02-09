using Individual_Identity.Core;
using System.Linq.Expressions;

namespace Individual_Identity.Data
{
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<TEntity> GetByIdAsync(int? id, bool includeDeleted = true);
        Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, bool includeDeleted = true);
        Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, bool includeDeleted = true);
        Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null, bool includeDeleted = true);
        IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, bool includeDeleted = true);
        Task InsertAsync(TEntity entity);
        Task InsertAsync(IList<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateAsync(IList<TEntity> entities);
        Task DeleteAsync(TEntity entity);
        Task DeleteAsync(IList<TEntity> entities);
        Task<TEntity> LoadOriginalCopyAsync(TEntity entity);
        IQueryable<TEntity> Table { get; }
    }
}
