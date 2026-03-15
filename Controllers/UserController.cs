using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using ViewModels = RestaurantFoodOrderingDeliverAdmin.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class UserController : Controller
    {
        private readonly IDeliveryUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public UserController(IDeliveryUserRepository userRepository, ApplicationDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        // List all delivery users with real delivery and earnings data
        [HttpGet]
        public async Task<IActionResult> DeliveryUsers()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Fetch all users
            var users = await _userRepository.GetAllAsync();
            
            // Create view model with computed data
            var viewModel = new ViewModels.DeliveryUsersListViewModel();
            
            foreach (var user in users)
            {
                // Get completed deliveries count for this user
                var completedDeliveries = await _context.DeliveryOrders
                    .Where(o => o.DeliveryUserId == user.DeliveryUserId && o.Status == "Delivered")
                    .CountAsync();
                
                // Get total earnings for this user
                var totalEarnings = await _context.DeliveryEarnings
                    .Where(e => e.DeliveryUserId == user.DeliveryUserId)
                    .SumAsync(e => e.DeliveryFee + e.TipAmount + e.Bonus + e.Incentive - e.Deduction);
                
                viewModel.Users.Add(new ViewModels.DeliveryUserViewModel
                {
                    DeliveryUserId = user.DeliveryUserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    City = user.City,
                    PermanentAddress = user.PermanentAddress,
                    CurrentAddress = user.CurrentAddress,
                    State = user.State,
                    Pincode = user.Pincode,
                    VehicleType = user.VehicleType,
                    VehicleNumber = user.VehicleNumber,
                    Status = user.Status,
                    IsActive = user.IsActive,
                    JoinDate = user.JoinDate,
                    LastLoginDate = user.LastLoginDate,
                    CompletedDeliveries = completedDeliveries,
                    TotalEarnings = totalEarnings
                });
            }
            
            // Calculate summary counts
            viewModel.TotalUsers = viewModel.Users.Count;
            viewModel.ActiveUsers = viewModel.Users.Count(u => u.IsActive && u.Status == "Active");
            viewModel.OnLeaveUsers = viewModel.Users.Count(u => u.Status == "On Leave");
            viewModel.InactiveUsers = viewModel.Users.Count(u => !u.IsActive || u.Status == "Inactive");
            
            return View(viewModel);
        }

        // View user details
        [HttpGet]
        public async Task<IActionResult> UserDetails(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Edit user form
        [HttpGet]
        public async Task<IActionResult> UserEdit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id == 0)
            {
                // New user
                return View(new DeliveryUser 
                { 
                    Status = "Active", 
                    IsActive = true,
                    JoinDate = System.DateTime.Now
                });
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Create new user
        [HttpPost]
        public async Task<IActionResult> UserCreate(DeliveryUser user)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            user.JoinDate = System.DateTime.Now;
            user.Rating = 0;
            user.TotalDeliveries = 0;
            user.TotalEarnings = 0;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            TempData["Success"] = "User created successfully!";
            return RedirectToAction("DeliveryUsers");
        }

        // Update existing user
        [HttpPost]
        public async Task<IActionResult> UserUpdate(DeliveryUser user)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingUser = await _userRepository.GetByIdAsync(user.DeliveryUserId);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Update fields
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;
            existingUser.Phone = user.Phone;
            existingUser.PermanentAddress = user.PermanentAddress;
            existingUser.CurrentAddress = user.CurrentAddress;
            existingUser.City = user.City;
            existingUser.State = user.State;
            existingUser.Pincode = user.Pincode;
            existingUser.VehicleType = user.VehicleType;
            existingUser.VehicleNumber = user.VehicleNumber;
            existingUser.Status = user.Status;
            existingUser.IsActive = user.IsActive;

            _userRepository.Update(existingUser);
            await _userRepository.SaveChangesAsync();
            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("UserDetails", new { id = user.DeliveryUserId });
        }

        // Delete user confirmation
        [HttpGet]
        public async Task<IActionResult> UserDelete(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Delete user confirmed
        [HttpPost]
        public async Task<IActionResult> UserDeleteConfirm(int DeliveryUserId, string confirmText)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (confirmText?.ToUpper() != "DELETE")
            {
                TempData["Error"] = "Please type DELETE to confirm.";
                return RedirectToAction("UserDelete", new { id = DeliveryUserId });
            }

            var user = await _userRepository.GetByIdAsync(DeliveryUserId);
            if (user == null)
            {
                return NotFound();
            }

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
            TempData["Success"] = "User deleted successfully!";
            return RedirectToAction("DeliveryUsers");
        }

        // Activate User
        [HttpPost]
        public async Task<IActionResult> ActivateUser(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = true;
            user.Status = "Active";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            TempData["Success"] = $"{user.FullName} has been activated!";
            return RedirectToAction("DeliveryUsers");
        }

        // Deactivate User
        [HttpPost]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsActive = false;
            user.Status = "Inactive";
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            TempData["Success"] = $"{user.FullName} has been deactivated!";
            return RedirectToAction("DeliveryUsers");
        }
    }
}
