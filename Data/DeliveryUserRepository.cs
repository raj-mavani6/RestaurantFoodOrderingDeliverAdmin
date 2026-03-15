using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Data
{
    public interface IDeliveryUserRepository : IRepository<DeliveryUser>
    {
        Task<DeliveryUser> GetByEmailAsync(string email);
        Task<DeliveryUser> GetWithAttendanceAsync(int id);
        Task<DeliveryUser> GetWithLeavesAsync(int id);
        Task<IEnumerable<DeliveryUser>> GetActiveUsersAsync();
    }

    public class DeliveryUserRepository : Repository<DeliveryUser>, IDeliveryUserRepository
    {
        private readonly ApplicationDbContext _context;

        public DeliveryUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Get user by email
        public async Task<DeliveryUser> GetByEmailAsync(string email)
        {
            return await _context.DeliveryUsers
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        // Get user with attendance records
        public async Task<DeliveryUser> GetWithAttendanceAsync(int id)
        {
            return await _context.DeliveryUsers
                .Include(u => u.Attendances)
                .FirstOrDefaultAsync(u => u.DeliveryUserId == id);
        }

        // Get user with leave records
        public async Task<DeliveryUser> GetWithLeavesAsync(int id)
        {
            return await _context.DeliveryUsers
                .Include(u => u.Leaves)
                .FirstOrDefaultAsync(u => u.DeliveryUserId == id);
        }

        // Get all active users
        public async Task<IEnumerable<DeliveryUser>> GetActiveUsersAsync()
        {
            return await _context.DeliveryUsers
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.Rating)
                .ToListAsync();
        }
    }
}
