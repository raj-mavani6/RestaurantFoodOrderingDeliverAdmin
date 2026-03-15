using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class EarningsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDeliveryUserRepository _userRepository;

        public EarningsController(ApplicationDbContext context, IDeliveryUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        // List all earnings
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var earnings = await _context.DeliveryEarnings
                .Include(e => e.DeliveryUser)
                .OrderByDescending(e => e.EarningDate)
                .ToListAsync();

            // Stats
            ViewBag.TotalEarnings = earnings.Sum(e => e.DeliveryFee + e.TipAmount + e.Bonus + e.Incentive - e.Deduction);
            ViewBag.TotalDeliveryFees = earnings.Sum(e => e.DeliveryFee);
            ViewBag.TotalTips = earnings.Sum(e => e.TipAmount);
            ViewBag.TotalBonus = earnings.Sum(e => e.Bonus + e.Incentive);
            ViewBag.TotalDeductions = earnings.Sum(e => e.Deduction);
            ViewBag.PaidCount = earnings.Count(e => e.PaymentStatus == "Paid");
            ViewBag.PendingCount = earnings.Count(e => e.PaymentStatus == "Pending");

            return View(earnings);
        }

        // View earning details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var earning = await _context.DeliveryEarnings
                .Include(e => e.DeliveryUser)
                .FirstOrDefaultAsync(e => e.EarningId == id);

            if (earning == null)
            {
                return NotFound();
            }

            return View(earning);
        }

        // Create earning form
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

        // Create earning POST
        [HttpPost]
        public async Task<IActionResult> Create(DeliveryEarning earning)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            earning.EarningDate = DateTime.Now;
            if (string.IsNullOrEmpty(earning.PaymentStatus))
            {
                earning.PaymentStatus = "Pending";
            }

            _context.DeliveryEarnings.Add(earning);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Earning record created successfully!";
            return RedirectToAction("Index");
        }

        // Edit earning form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var earning = await _context.DeliveryEarnings
                .Include(e => e.DeliveryUser)
                .FirstOrDefaultAsync(e => e.EarningId == id);

            if (earning == null)
            {
                return NotFound();
            }

            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View(earning);
        }

        // Edit earning POST
        [HttpPost]
        public async Task<IActionResult> Edit(DeliveryEarning earning)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingEarning = await _context.DeliveryEarnings.FindAsync(earning.EarningId);
            if (existingEarning == null)
            {
                return NotFound();
            }

            existingEarning.DeliveryUserId = earning.DeliveryUserId;
            existingEarning.DeliveryFee = earning.DeliveryFee;
            existingEarning.TipAmount = earning.TipAmount;
            existingEarning.Bonus = earning.Bonus;
            existingEarning.Incentive = earning.Incentive;
            existingEarning.Deduction = earning.Deduction;
            existingEarning.EarningType = earning.EarningType;
            existingEarning.Description = earning.Description;
            existingEarning.PaymentStatus = earning.PaymentStatus;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Earning record updated successfully!";
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

            var earning = await _context.DeliveryEarnings
                .Include(e => e.DeliveryUser)
                .FirstOrDefaultAsync(e => e.EarningId == id);

            if (earning == null)
            {
                return NotFound();
            }

            return View(earning);
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

            var earning = await _context.DeliveryEarnings.FindAsync(id);
            if (earning == null)
            {
                return NotFound();
            }

            _context.DeliveryEarnings.Remove(earning);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Earning record deleted successfully!";
            return RedirectToAction("Index");
        }

        // Mark as Paid
        [HttpPost]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var earning = await _context.DeliveryEarnings.FindAsync(id);
            if (earning == null)
            {
                return NotFound();
            }

            earning.PaymentStatus = "Paid";

            await _context.SaveChangesAsync();
            TempData["Success"] = "Payment marked as paid!";
            return RedirectToAction("Index");
        }
    }
}
