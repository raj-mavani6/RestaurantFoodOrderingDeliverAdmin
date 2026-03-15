using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Data
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance> GetTodayAttendanceAsync(int deliveryUserId);
        Task<IEnumerable<Attendance>> GetMonthlyAttendanceAsync(int deliveryUserId, int month, int year);
        Task<IEnumerable<Attendance>> GetUserAttendanceAsync(int deliveryUserId, int limit = 30);
        Task<Attendance> GetLatestAttendanceAsync(int deliveryUserId);
    }

    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Get today's attendance record
        public async Task<Attendance> GetTodayAttendanceAsync(int deliveryUserId)
        {
            var today = DateTime.Now.Date;
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.DeliveryUserId == deliveryUserId && 
                                         a.AttendanceDate.Date == today);
        }

        // Get monthly attendance records
        public async Task<IEnumerable<Attendance>> GetMonthlyAttendanceAsync(int deliveryUserId, int month, int year)
        {
            return await _context.Attendances
                .Where(a => a.DeliveryUserId == deliveryUserId &&
                           a.AttendanceDate.Month == month &&
                           a.AttendanceDate.Year == year)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();
        }

        // Get user's attendance records with limit
        public async Task<IEnumerable<Attendance>> GetUserAttendanceAsync(int deliveryUserId, int limit = 30)
        {
            return await _context.Attendances
                .Where(a => a.DeliveryUserId == deliveryUserId)
                .OrderByDescending(a => a.AttendanceDate)
                .Take(limit)
                .ToListAsync();
        }

        // Get latest attendance record
        public async Task<Attendance> GetLatestAttendanceAsync(int deliveryUserId)
        {
            return await _context.Attendances
                .Where(a => a.DeliveryUserId == deliveryUserId)
                .OrderByDescending(a => a.AttendanceDate)
                .FirstOrDefaultAsync();
        }
    }
}
