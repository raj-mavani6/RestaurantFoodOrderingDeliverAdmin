using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IDeliveryUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public AttendanceController(
            IAttendanceRepository attendanceRepository, 
            IDeliveryUserRepository userRepository,
            ApplicationDbContext context)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _context = context;
        }

        // List all attendance records
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var attendances = await _context.Attendances
                .Include(a => a.DeliveryUser)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();
            
            // Get summary stats
            var today = DateTime.Today;
            ViewBag.TotalRecords = attendances.Count;
            ViewBag.TodayPresent = attendances.Count(a => a.AttendanceDate.Date == today && a.Status == "Present");
            ViewBag.TodayLate = attendances.Count(a => a.AttendanceDate.Date == today && a.Status == "Late");
            ViewBag.TodayAbsent = attendances.Count(a => a.AttendanceDate.Date == today && a.Status == "Absent");
            
            return View(attendances);
        }

        // View attendance details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var attendance = await _context.Attendances
                .Include(a => a.DeliveryUser)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
            
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // Add attendance form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View(new Attendance { AttendanceDate = DateTime.Today, Status = "Present" });
        }

        // Add attendance POST
        [HttpPost]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            await _attendanceRepository.AddAsync(attendance);
            await _attendanceRepository.SaveChangesAsync();
            TempData["Success"] = "Attendance record added successfully!";
            return RedirectToAction("Index");
        }

        // Edit attendance form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View(attendance);
        }

        // Edit attendance POST
        [HttpPost]
        public async Task<IActionResult> Edit(Attendance attendance)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingAttendance = await _attendanceRepository.GetByIdAsync(attendance.AttendanceId);
            if (existingAttendance == null)
            {
                return NotFound();
            }

            existingAttendance.DeliveryUserId = attendance.DeliveryUserId;
            existingAttendance.AttendanceDate = attendance.AttendanceDate;
            existingAttendance.CheckInTime = attendance.CheckInTime;
            existingAttendance.CheckOutTime = attendance.CheckOutTime;
            existingAttendance.IntermediateStartTime = attendance.IntermediateStartTime;
            existingAttendance.IntermediateEndTime = attendance.IntermediateEndTime;
            existingAttendance.InTimeReason = attendance.InTimeReason;
            existingAttendance.OutTimeReason = attendance.OutTimeReason;
            existingAttendance.IntermediateStartReason = attendance.IntermediateStartReason;
            existingAttendance.IntermediateEndReason = attendance.IntermediateEndReason;
            existingAttendance.Status = attendance.Status;
            existingAttendance.Notes = attendance.Notes;
            existingAttendance.OrdersCompleted = attendance.OrdersCompleted;
            existingAttendance.DistanceCovered = attendance.DistanceCovered;

            _attendanceRepository.Update(existingAttendance);
            await _attendanceRepository.SaveChangesAsync();
            TempData["Success"] = "Attendance record updated successfully!";
            return RedirectToAction("Index");
        }

        // Delete confirmation
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var attendance = await _context.Attendances
                .Include(a => a.DeliveryUser)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
            
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // Delete POST
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null)
            {
                return NotFound();
            }

            _attendanceRepository.Remove(attendance);
            await _attendanceRepository.SaveChangesAsync();
            TempData["Success"] = "Attendance record deleted successfully!";
            return RedirectToAction("Index");
        }

        // View attendance by user
        [HttpGet]
        public async Task<IActionResult> ByUser(int userId, int? month, int? year)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var attendances = await _context.Attendances
                .Where(a => a.DeliveryUserId == userId)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();

            ViewBag.User = user;
            ViewBag.SelectedMonth = month ?? DateTime.Now.Month;
            ViewBag.SelectedYear = year ?? DateTime.Now.Year;
            return View(attendances);
        }
    }
}
