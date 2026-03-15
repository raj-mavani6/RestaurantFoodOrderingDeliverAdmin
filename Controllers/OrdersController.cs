using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Data;
using RestaurantFoodOrderingDeliverAdmin.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverAdmin.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDeliveryUserRepository _userRepository;

        public OrdersController(ApplicationDbContext context, IDeliveryUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        // List all orders (Assigned + Unassigned)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // 1. Get all Assigned Orders (DeliveryOrders)
            var assignedOrders = await _context.DeliveryOrders
                .Include(o => o.DeliveryUser)
                .ToListAsync();

            // 2. Get all Unassigned Orders (Main Orders not in DeliveryOrders)
            var assignedOrderIds = assignedOrders.Select(d => d.OrderId).ToList();
            
            var unassignedOrders = await _context.Orders
                .Include(o => o.Customer)
                .Where(o => !assignedOrderIds.Contains(o.OrderId))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // 3. Convert Unassigned Orders to DeliveryOrder objects for display
            var pendingDeliveryOrders = unassignedOrders.Select(o => new DeliveryOrder
            {
                DeliveryOrderId = 0, // Not saved yet
                OrderId = o.OrderId,
                CustomerName = o.Customer?.FullName ?? "Guest",
                CustomerPhone = o.Customer?.Phone ?? o.ContactPhone ?? "N/A",
                DeliveryAddress = o.DeliveryAddress,
                TotalAmount = o.TotalAmount,
                Status = "Pending",
                PaymentStatus = o.PaymentStatus ?? "Pending",
                DeliveryFee = 0,
                TipAmount = 0,
                OrderTime = o.OrderDate,
                DeliveryUser = null // Unassigned
            }).ToList();

            // 4. Merge lists
            var allOrders = assignedOrders.Concat(pendingDeliveryOrders)
                .OrderByDescending(o => o.OrderTime)
                .ToList();

            // Stats
            ViewBag.TotalOrders = allOrders.Count;
            ViewBag.PendingOrders = allOrders.Count(o => o.Status == "Pending"); // Unassigned
            ViewBag.CompletedOrders = allOrders.Count(o => o.Status == "Delivered");
            ViewBag.CancelledOrders = allOrders.Count(o => o.Status == "Cancelled");
            
            // Also count "Assigned" as a separate stat if needed, or include in Pending logic based on requirements
            // For now keeping ViewBag.PendingOrders as strict "Pending" status

            return View(allOrders);
        }

        // View order details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var order = await _context.DeliveryOrders
                .Include(o => o.DeliveryUser)
                .FirstOrDefaultAsync(o => o.DeliveryOrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Edit order form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var order = await _context.DeliveryOrders
                .Include(o => o.DeliveryUser)
                .FirstOrDefaultAsync(o => o.DeliveryOrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View(order);
        }

        // Edit order POST
        [HttpPost]
        public async Task<IActionResult> Edit(DeliveryOrder order)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingOrder = await _context.DeliveryOrders.FindAsync(order.DeliveryOrderId);
            if (existingOrder == null)
            {
                return NotFound();
            }

            existingOrder.CustomerName = order.CustomerName;
            existingOrder.CustomerPhone = order.CustomerPhone;
            existingOrder.DeliveryAddress = order.DeliveryAddress;
            existingOrder.Status = order.Status;
            existingOrder.PaymentStatus = order.PaymentStatus;
            existingOrder.DeliveryFee = order.DeliveryFee;
            existingOrder.TipAmount = order.TipAmount;
            existingOrder.SpecialInstructions = order.SpecialInstructions;
            existingOrder.DeliveryNotes = order.DeliveryNotes;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Order updated successfully!";
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

            var order = await _context.DeliveryOrders
                .Include(o => o.DeliveryUser)
                .FirstOrDefaultAsync(o => o.DeliveryOrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
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

            var order = await _context.DeliveryOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.DeliveryOrders.Remove(order);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Order deleted successfully!";
            return RedirectToAction("Index");
        }

        // Assign order form - show orders from Orders table that are not yet assigned
        [HttpGet]
        public async Task<IActionResult> AssignOrder()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get all order IDs that are already assigned in DeliveryOrders
            var assignedOrderIds = await _context.DeliveryOrders
                .Where(d => d.DeliveryUserId > 0)
                .Select(d => d.OrderId)
                .ToListAsync();

            // Get order IDs from Orders table that are NOT yet assigned
            var unassignedOrderIds = await _context.Orders
                .Where(o => !assignedOrderIds.Contains(o.OrderId))
                .Select(o => o.OrderId)
                .OrderByDescending(o => o)
                .ToListAsync();

            ViewBag.UnassignedOrderIds = unassignedOrderIds;
            ViewBag.DeliveryUsers = await _userRepository.GetAllAsync();
            return View();
        }

        // Assign order POST
        // Assign order POST
        [HttpPost]
        public async Task<IActionResult> AssignOrder(int orderId, int deliveryUserId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // 1. Check if DeliveryOrder already exists for this Main Order ID
                var existingDeliveryOrder = await _context.DeliveryOrders.FirstOrDefaultAsync(d => d.OrderId == orderId);
                
                if (existingDeliveryOrder != null)
                {
                    // Update existing delivery order
                    existingDeliveryOrder.DeliveryUserId = deliveryUserId;
                    existingDeliveryOrder.Status = "Assigned";
                    existingDeliveryOrder.AssignedTime = DateTime.Now;
                    
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Order reassigned successfully!";
                    return RedirectToAction("Assignments", new { orderId = existingDeliveryOrder.DeliveryOrderId });
                }

                // 2. Fetch Main Order details to create new DeliveryOrder
                var mainOrder = await _context.Orders
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (mainOrder == null)
                {
                    TempData["Error"] = "Original order not found in main database!";
                    return RedirectToAction("AssignOrder");
                }

                // 3. Create NEW DeliveryOrder
                var newDeliveryOrder = new DeliveryOrder
                {
                    OrderId = mainOrder.OrderId,
                    DeliveryUserId = deliveryUserId,
                    CustomerName = mainOrder.Customer?.FullName ?? "Guest User",
                    CustomerPhone = mainOrder.Customer?.Phone ?? mainOrder.ContactPhone ?? "N/A",
                    DeliveryAddress = mainOrder.DeliveryAddress,
                    TotalAmount = mainOrder.TotalAmount,
                    Status = "Assigned",
                    PaymentStatus = mainOrder.PaymentStatus ?? "Pending",
                    DeliveryFee = 0, // Default or calculate
                    TipAmount = 0,
                    AssignedTime = DateTime.Now,
                    OrderTime = mainOrder.OrderDate,
                    SpecialInstructions = mainOrder.SpecialInstructions,
                    Distance = 0 // Default
                };

                _context.DeliveryOrders.Add(newDeliveryOrder);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Order assigned successfully!";
                return RedirectToAction("Assignments", new { orderId = newDeliveryOrder.DeliveryOrderId });
            }
            catch (Exception ex)
            {
                // Log the exception (in a real app)
                Console.WriteLine($"Error assigning order: {ex.Message}");
                TempData["Error"] = $"Failed to assign order: {ex.Message}";
                return RedirectToAction("AssignOrder");
            }
        }

        // View assignments - can filter by orderId
        [HttpGet]
        public async Task<IActionResult> Assignments(int? orderId)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                return RedirectToAction("Login", "Auth");
            }

            IQueryable<DeliveryOrder> query = _context.DeliveryOrders
                .Include(o => o.DeliveryUser)
                .Where(o => o.DeliveryUserId > 0);

            // If orderId is provided, filter by that order
            if (orderId.HasValue && orderId.Value > 0)
            {
                query = query.Where(o => o.DeliveryOrderId == orderId.Value);
                ViewBag.FilteredOrderId = orderId.Value;
            }

            var assignments = await query
                .OrderByDescending(o => o.AssignedTime)
                .ToListAsync();

            return View(assignments);
        }
    }
}
