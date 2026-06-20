using System.Linq.Expressions;

namespace InventoryFlow.Repositories.Interfaces
{
    /// <summary>
    /// Generic data-access contract shared by all entity repositories.
    /// Keeps EF Core specifics (DbContext, DbSet) out of the Service layer.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> SaveChangesAsync();
    }
}
