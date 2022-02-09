using Individual_Identity.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Transactions;

namespace Individual_Identity.Data
{
    public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DataContext _context;

        public EntityRepository(DataContext context)
        {
            _context = context;
        }

        protected virtual IQueryable<TEntity> AddDeletedFilter(IQueryable<TEntity> query, in bool includeDeleted)
        {
            if (includeDeleted)
                return query;

            if (typeof(TEntity).GetInterface(nameof(ISoftDeletedEntity)) == null)
                return query;

            return query.OfType<ISoftDeletedEntity>().Where(entry => !entry.Deleted).OfType<TEntity>();
        }

        public virtual async Task<TEntity> GetByIdAsync(int? id, bool includeDeleted = true)
        {
            if (!id.HasValue || id == 0)
                return null;

            async Task<TEntity> getEntityAsync()
            {
                return await AddDeletedFilter(Table, includeDeleted).FirstOrDefaultAsync(entity => entity.Id == Convert.ToInt32(id));
            }
            return await getEntityAsync();
        }

        public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, bool includeDeleted = true)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            async Task<IList<TEntity>> getByIdsAsync()
            {
                var query = AddDeletedFilter(Table, includeDeleted);

                var entries = await query.Where(entry => ids.Contains(entry.Id)).ToListAsync();

                var sortedEntries = new List<TEntity>();
                foreach (var id in ids)
                {
                    var sortedEntry = entries.Find(entry => entry.Id == id);
                    if (sortedEntry != null)
                        sortedEntries.Add(sortedEntry);
                }

                return sortedEntries;
            }
            return await getByIdsAsync();
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, bool includeDeleted = true)
        {
            async Task<IList<TEntity>> getAllAsync()
            {
                var query = AddDeletedFilter(Table, includeDeleted);
                query = func != null ? func(query) : query;

                return await query.ToListAsync();
            }

            return await getAllAsync();
        }
        public virtual IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null, bool includeDeleted = true)
        {
            IList<TEntity> getAll()
            {
                var query = AddDeletedFilter(Table, includeDeleted);
                query = func != null ? func(query) : query;

                return query.ToList();
            }

            return getAll();
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null, bool includeDeleted = true)
        {
            async Task<IList<TEntity>> getAllAsync()
            {
                var query = AddDeletedFilter(Table, includeDeleted);
                query = func != null ? await func(query) : query;

                return await query.ToListAsync();
            }

            return await getAllAsync();
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task InsertAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _context.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            transaction.Complete();
        }

        public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
        {
            return await _context.Set<TEntity>()
                .FirstOrDefaultAsync(e => e.Id == Convert.ToInt32(entity.Id));
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (entities.Count == 0)
                return;

            _context.Update(entities);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            switch (entity)
            {
                case null:
                    throw new ArgumentNullException(nameof(entity));

                case ISoftDeletedEntity softDeletedEntity:
                    softDeletedEntity.Deleted = true;
                    _context.Update(entity);
                    break;

                default:
                    _context.Remove(entity);
                    break;
            }
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (entities.OfType<ISoftDeletedEntity>().Any())
            {
                foreach (var entity in entities)
                {
                    if (entity is ISoftDeletedEntity softDeletedEntity)
                    {
                        softDeletedEntity.Deleted = true;
                        _context.Update(entity);
                    }
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                _context.RemoveRange(entities);
                await _context.SaveChangesAsync();
                transaction.Complete();
            }
        }

        public virtual IQueryable<TEntity> Table => _context.Set<TEntity>();
    }
}
