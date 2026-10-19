using System.Linq.Expressions;
using CargoTrack.Entity.Entities.Common;

namespace CargoTrack.DataAccess.Repositories.GenericRepositories
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(Guid id);
        Task CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);        // Soft delete
        Task HardDeleteAsync(TEntity entity);    // Kalıcı silme

        // Gelişmiş sorgular (soft delete uygulanmış kayıtlar hariç tutulur)
        IQueryable<TEntity> Query();
        Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null);

        // Sayfalama
        Task<PagedResult<TEntity>> GetPagedAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? predicate = null);

        Task SaveChangesAsync();
    }
}
