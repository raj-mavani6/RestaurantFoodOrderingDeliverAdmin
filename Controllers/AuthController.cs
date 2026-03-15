using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class AuthController : Controller
    {
        // Login Page (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login Authentication (POST)
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Mock authentication for admin
            if (email == "admin@tastybites.com" && password == "admin123")
            {
                // Set session
                HttpContext.Session.SetString("AdminEmail", email);
                HttpContext.Session.SetString("AdminName", "Admin User");
                HttpContext.Session.SetInt32("AdminId", 1);

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Invalid email or password!";
            return View();
        }

        // Logout
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
