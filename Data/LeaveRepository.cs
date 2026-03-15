using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Data
{
    public interface ILeaveRepository : IRepository<Leave>
    {
        Task<IEnumerable<Leave>> GetUserLeavesAsync(int deliveryUserId);
        Task<IEnumerable<Leave>> GetPendingLeavesAsync(int deliveryUserId);
        Task<IEnumerable<Leave>> GetApprovedLeavesAsync(int deliveryUserId);
        Task<IEnumerable<Leave>> GetRejectedLeavesAsync(int deliveryUserId);
        Task<IEnumerable<Leave>> GetLeavesByStatusAsync(int deliveryUserId, string status);
        Task<int> GetApprovedLeaveDaysAsync(int deliveryUserId, string leaveType, int year);
    }

    public class LeaveRepository : Repository<Leave>, ILeaveRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Get all leaves for a user
        public async Task<IEnumerable<Leave>> GetUserLeavesAsync(int deliveryUserId)
        {
            return await _context.Leaves
                .Where(l => l.DeliveryUserId == deliveryUserId)
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();
        }

        // Get pending leaves for a user
        public async Task<IEnumerable<Leave>> GetPendingLeavesAsync(int deliveryUserId)
        {
            return await _context.Leaves
                .Where(l => l.DeliveryUserId == deliveryUserId && l.Status == "Pending")
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();
        }

        // Get approved leaves for a user
        public async Task<IEnumerable<Leave>> GetApprovedLeavesAsync(int deliveryUserId)
        {
            return await _context.Leaves
                .Where(l => l.DeliveryUserId == deliveryUserId && l.Status == "Approved")
                .OrderByDescending(l => l.ApprovedDate)
                .ToListAsync();
        }

        // Get rejected leaves for a user
        public async Task<IEnumerable<Leave>> GetRejectedLeavesAsync(int deliveryUserId)
        {
            return await _context.Leaves
                .Where(l => l.DeliveryUserId == deliveryUserId && l.Status == "Rejected")
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();
        }

        // Get leaves by status
        public async Task<IEnumerable<Leave>> GetLeavesByStatusAsync(int deliveryUserId, string status)
        {
            return await _context.Leaves
                .Where(l => l.DeliveryUserId == deliveryUserId && l.Status == status)
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();
        }

        // Get total approved leave days for a specific leave type in a year
        public async Task<int> GetApprovedLeaveDaysAsync(int deliveryUserId, string leaveType, int year)
        {
            var approvedLeaves = await _context.Leaves
                .Where(l => l.DeliveryUserId == deliveryUserId &&
                           l.LeaveType == leaveType &&
                           l.Status == "Approved" &&
                           l.StartDate.Year == year)
                .ToListAsync();

            int totalDays = 0;
            foreach (var leave in approvedLeaves)
            {
                totalDays += (int)(leave.EndDate - leave.StartDate).TotalDays + 1;
            }

            return totalDays;
        }
    }
}
