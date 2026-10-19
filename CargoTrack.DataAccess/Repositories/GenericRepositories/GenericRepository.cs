using System.Linq.Expressions;
using CargoTrack.DataAccess.Context;
using CargoTrack.Entity.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace CargoTrack.DataAccess.Repositories.GenericRepositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly CargoTrackDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public GenericRepository(CargoTrackDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task CreateAsync(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now;
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Soft delete - kayıt fiziksel olarak silinmez, IsDeleted=true yapılır.
        /// </summary>
        public async Task DeleteAsync(TEntity entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.Now;
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task HardDeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            return predicate is null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }

        public async Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _dbSet.AsQueryable();

            if (predicate is not null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<TEntity>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
