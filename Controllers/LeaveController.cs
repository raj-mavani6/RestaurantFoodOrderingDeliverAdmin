using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class LeaveController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDeliveryUserRepository _userRepository;

        public LeaveController(ApplicationDbContext context, IDeliveryUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        // List all leaves
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var leaves = await _context.Leaves
                .Include(l => l.DeliveryUser)
                .OrderByDescending(l => l.AppliedDate)
                .ToListAsync();

            // Stats
            ViewBag.TotalLeaves = leaves.Count;
            ViewBag.PendingLeaves = leaves.Count(l => l.Status == "Pending");
            ViewBag.ApprovedLeaves = leaves.Count(l => l.Status == "Approved");
            ViewBag.RejectedLeaves = leaves.Count(l => l.Status == "Rejected");

            return View(leaves);
        }

        // View leave details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var leave = await _context.Leaves
                .Include(l => l.DeliveryUser)
                .FirstOrDefaultAsync(l => l.LeaveId == id);

            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
        }

        // Create leave form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View();
        }

        // Create leave POST
        [HttpPost]
        public async Task<IActionResult> Create(Leave leave)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            leave.AppliedDate = DateTime.Now;
            if (string.IsNullOrEmpty(leave.Status))
            {
                leave.Status = "Pending";
            }

            _context.Leaves.Add(leave);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Leave request created successfully!";
            return RedirectToAction("Index");
        }

        // Edit leave form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var leave = await _context.Leaves
                .Include(l => l.DeliveryUser)
                .FirstOrDefaultAsync(l => l.LeaveId == id);

            if (leave == null)
            {
                return NotFound();
            }

            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View(leave);
        }

        // Edit leave POST
        [HttpPost]
        public async Task<IActionResult> Edit(Leave leave)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingLeave = await _context.Leaves.FindAsync(leave.LeaveId);
            if (existingLeave == null)
            {
                return NotFound();
            }

            existingLeave.DeliveryUserId = leave.DeliveryUserId;
            existingLeave.StartDate = leave.StartDate;
            existingLeave.EndDate = leave.EndDate;
            existingLeave.LeaveType = leave.LeaveType;
            existingLeave.Reason = leave.Reason;
            existingLeave.Status = leave.Status;
            existingLeave.AdminNotes = leave.AdminNotes;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Leave request updated successfully!";
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

            var leave = await _context.Leaves
                .Include(l => l.DeliveryUser)
                .FirstOrDefaultAsync(l => l.LeaveId == id);

            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
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

            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            _context.Leaves.Remove(leave);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Leave request deleted successfully!";
            return RedirectToAction("Index");
        }

        // Approve leave
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = "Approved";
            leave.ApprovedBy = HttpContext.Session.GetString("AdminEmail");
            leave.ApprovedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Leave request approved!";
            return RedirectToAction("Index");
        }

        // Reject leave
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = "Rejected";
            leave.ApprovedBy = HttpContext.Session.GetString("AdminEmail");
            leave.ApprovedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Leave request rejected!";
            return RedirectToAction("Index");
        }
    }
}
