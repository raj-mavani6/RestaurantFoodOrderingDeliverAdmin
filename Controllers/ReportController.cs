using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        public async Task<IActionResult> DeliveryUsers(string period = "month")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var users = await _context.DeliveryUsers.ToListAsync();
            ViewBag.TotalUsers = users.Count;
            ViewBag.ActiveUsers = users.Count(u => u.Status == "Active");
            ViewBag.InactiveUsers = users.Count(u => u.Status == "Inactive");
            
            // Calculate average rating
            var ratedUsers = users.Where(u => u.Rating > 0).ToList();
            ViewBag.AvgRating = ratedUsers.Any() ? ratedUsers.Average(u => u.Rating) : 0;

            return View(users);
        }

        public async Task<IActionResult> Attendance(string period = "month")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var query = _context.Attendances.AsQueryable();
            DateTime startDate = DateTime.Today;

            switch (period)
            {
                // For trends, we want to see History grouped by requested unit.
                // Week/Month/Year/All -> Fetch all relevant history to show trends.
                case "week":
                    // For Weekly trends, maybe last 3 months or check all?
                    // User said "Week ma week mujab" -> Weekly. 
                    startDate = DateTime.MinValue; 
                    break;
                case "month":
                    startDate = DateTime.MinValue;
                    break;
                case "year":
                    startDate = DateTime.MinValue;
                    break;
                 case "all":
                    startDate = DateTime.MinValue;
                    break;
            }

            if (period != "all")
            {
                query = query.Where(a => a.AttendanceDate >= startDate);
            }

            var attendances = await query.Include(a => a.DeliveryUser).OrderByDescending(a => a.AttendanceDate).ToListAsync();
            
            ViewBag.Period = period;
            return View(attendances);
        }

        public async Task<IActionResult> Orders(string period = "month")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var query = _context.DeliveryOrders.AsQueryable();
            DateTime startDate = DateTime.Today;

           switch (period)
            {
                case "week":
                    startDate = DateTime.Today.AddDays(-7);
                    break;
                case "month":
                    startDate = DateTime.Today.AddMonths(-1);
                    break;
                case "year":
                    startDate = DateTime.Today.AddYears(-1);
                    break;
                 case "all":
                    startDate = DateTime.MinValue;
                    break;
            }

            if (period != "all")
            {
                query = query.Where(o => o.OrderTime >= startDate);
            }

            var orders = await query.Include(o => o.DeliveryUser).OrderByDescending(o => o.OrderTime).ToListAsync();
            ViewBag.Period = period;
            return View(orders);
        }

        public async Task<IActionResult> Earnings(string period = "month")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var query = _context.DeliveryEarnings.AsQueryable();
             DateTime startDate = DateTime.Today;

           switch (period)
            {
                case "week":
                    startDate = DateTime.Today.AddDays(-7);
                    break;
                case "month":
                    startDate = DateTime.Today.AddMonths(-1);
                    break;
                case "year":
                    startDate = DateTime.Today.AddYears(-1);
                    break;
                 case "all":
                    startDate = DateTime.MinValue;
                    break;
            }

             // Note: DeliveryEarning might not have a date field directly depending on design, 
             // assuming we filter by linked data or if there's a CreatedDate/PaymentDate.
             // Checking DeliveryEarning model in 4007, it doesn't show a date. 
             // We might need to join with Orders or assume logic.
             // For now, let's fetch all and filter in memory or ignore date if not present.
             // Looking at ApplicationDbContext, DeliveryEarning has no date. 
             // Let's assume we show all or link via DeliveryUser/Work logic?
             // Or maybe we should use DeliveryOrders to calculate potential earnings.
             // Wait, DeliveryEarning usually tracks payouts. Let's list them all for now.
            
            var earnings = await query.Include(e => e.DeliveryUser).ToListAsync();
            return View(earnings);
        }

        public async Task<IActionResult> Leave(string period = "month")
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var query = _context.Leaves.AsQueryable();
             DateTime startDate = DateTime.Today;

            switch (period)
            {
                 case "week":
                    startDate = DateTime.MinValue;
                    break;
                case "month":
                    startDate = DateTime.MinValue;
                    break;
                case "year":
                    startDate = DateTime.MinValue;
                    break;
                 case "all":
                    startDate = DateTime.MinValue;
                    break;
            }

            if (period != "all")
            {
                 query = query.Where(l => l.StartDate >= startDate);
            }

            var leaves = await query.Include(l => l.DeliveryUser).OrderByDescending(l => l.StartDate).ToListAsync();
            ViewBag.Period = period;
            return View(leaves);
        }
    }
}
