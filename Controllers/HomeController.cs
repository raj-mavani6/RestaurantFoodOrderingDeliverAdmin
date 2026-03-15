using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDeliveryUserRepository _userRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ApplicationDbContext _context;

        public HomeController(
            IDeliveryUserRepository userRepository,
            IAttendanceRepository attendanceRepository,
            ILeaveRepository leaveRepository,
            ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _attendanceRepository = attendanceRepository;
            _leaveRepository = leaveRepository;
            _context = context;
        }

        // Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get real data from database
            var users = await _userRepository.GetAllAsync();
            var usersList = users.ToList();
            
            var totalUsers = usersList.Count;
            var activeUsers = usersList.Count(u => u.IsActive);
            
            // Get total deliveries from DeliveryOrders table (Delivered status)
            var totalDeliveries = await _context.DeliveryOrders
                .CountAsync(d => d.Status == "Delivered");
            
            // Get pending leaves
            var allLeaves = await _leaveRepository.GetAllAsync();
            var pendingLeaves = allLeaves.Count(l => l.Status == "Pending");

            // Get today's attendance
            var today = DateTime.Today;
            var todayAttendance = await _context.Attendances
                .Include(a => a.DeliveryUser)
                .Where(a => a.AttendanceDate.Date == today)
                .ToListAsync();
            
            ViewBag.LateToday = todayAttendance.Where(a => a.Status == "Late").ToList();
            ViewBag.AbsentToday = todayAttendance.Where(a => a.Status == "Absent").ToList();

            // Get Pending and Cancelled Orders
            var pendingOrders = await _context.DeliveryOrders
                .Include(d => d.DeliveryUser)
                .Where(d => d.Status == "Pending" || d.Status == "Assigned")
                .OrderByDescending(d => d.AssignedTime)
                .Take(10)
                .ToListAsync();
            
            var cancelledOrders = await _context.DeliveryOrders
                .Include(d => d.DeliveryUser)
                .Where(d => d.Status == "Cancelled")
                .OrderByDescending(d => d.AssignedTime)
                .Take(10)
                .ToListAsync();

            ViewBag.PendingOrders = pendingOrders;
            ViewBag.CancelledOrders = cancelledOrders;

            // Get Pending Earnings
            var pendingEarnings = await _context.DeliveryEarnings
                .Include(e => e.DeliveryUser)
                .Where(e => e.PaymentStatus == "Pending")
                .OrderByDescending(e => e.EarningDate)
                .Take(10)
                .ToListAsync();

            ViewBag.PendingEarnings = pendingEarnings;

            // Get Pending and Rejected Leaves
            var pendingLeavesList = await _context.Leaves
                .Include(l => l.DeliveryUser)
                .Where(l => l.Status == "Pending")
                .OrderByDescending(l => l.AppliedDate)
                .Take(10)
                .ToListAsync();

            var rejectedLeaves = await _context.Leaves
                .Include(l => l.DeliveryUser)
                .Where(l => l.Status == "Rejected")
                .OrderByDescending(l => l.AppliedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.PendingLeavesList = pendingLeavesList;
            ViewBag.RejectedLeaves = rejectedLeaves;

            ViewBag.TotalUsers = totalUsers;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.TotalDeliveries = totalDeliveries;
            ViewBag.PendingLeaves = pendingLeaves;

            return View(usersList);
        }

        // Delivery Users Management
        [HttpGet]
        public async Task<IActionResult> DeliveryUsers()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        // User Details
        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetWithAttendanceAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Attendance Management
        [HttpGet]
        public IActionResult Attendance()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Mock attendance data
            var attendanceList = new List<Attendance>
            {
                new Attendance { AttendanceId = 1, DeliveryUserId = 1, AttendanceDate = DateTime.Now.AddDays(-1), CheckInTime = "09:00 AM", CheckOutTime = "06:00 PM", Status = "Present", OrdersCompleted = 12, DistanceCovered = 45.5 },
                new Attendance { AttendanceId = 2, DeliveryUserId = 2, AttendanceDate = DateTime.Now.AddDays(-1), CheckInTime = "09:30 AM", CheckOutTime = "06:30 PM", Status = "Present", OrdersCompleted = 10, DistanceCovered = 38.2 },
                new Attendance { AttendanceId = 3, DeliveryUserId = 3, AttendanceDate = DateTime.Now.AddDays(-1), CheckInTime = "09:15 AM", CheckOutTime = "05:45 PM", Status = "Late", OrdersCompleted = 8, DistanceCovered = 35.0 }
            };

            return View(attendanceList);
        }

        // Leave Management
        [HttpGet]
        public IActionResult Leaves()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Mock leave data
            var leaveList = new List<Leave>
            {
                new Leave { LeaveId = 1, DeliveryUserId = 1, StartDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(7), LeaveType = "Casual", Reason = "Personal work", Status = "Pending", AppliedDate = DateTime.Now },
                new Leave { LeaveId = 2, DeliveryUserId = 2, StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-8), LeaveType = "Sick", Reason = "Medical checkup", Status = "Approved", ApprovedBy = "Admin", ApprovedDate = DateTime.Now.AddDays(-9), AppliedDate = DateTime.Now.AddDays(-10) },
                new Leave { LeaveId = 3, DeliveryUserId = 3, StartDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddDays(12), LeaveType = "Casual", Reason = "Family visit", Status = "Pending", AppliedDate = DateTime.Now }
            };

            return View(leaveList);
        }

        // Approve Leave
        [HttpPost]
        public IActionResult ApproveLeave(int leaveId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // TODO: Update leave status to Approved
            return RedirectToAction("Leaves");
        }

        // Reject Leave
        [HttpPost]
        public IActionResult RejectLeave(int leaveId, string adminNotes)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // TODO: Update leave status to Rejected with notes
            return RedirectToAction("Leaves");
        }
    }
}
