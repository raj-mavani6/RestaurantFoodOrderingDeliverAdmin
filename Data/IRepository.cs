using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Data
{
    public interface IRepository<T> where T : class
    {
        // Get operations
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        // Add operations
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // Update operations
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        // Delete operations
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        // Save changes
        Task<int> SaveChangesAsync();
    }
}
