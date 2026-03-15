using System.Collections.Generic;

namespace RestaurantFoodOrderingDeliverAdmin.Models.ViewModels
{
    public class DeliveryUserViewModel
    {
        public int DeliveryUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? City { get; set; }
        public string? PermanentAddress { get; set; }
        public string? CurrentAddress { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime JoinDate { get; set; }
        public System.DateTime? LastLoginDate { get; set; }
        
        // Computed from database
        public int CompletedDeliveries { get; set; }  // Count from DeliveryOrders where Status = 'Delivered'
        public decimal TotalEarnings { get; set; }    // Sum from DeliveryEarning
    }

    public class DeliveryUsersListViewModel
    {
        public List<DeliveryUserViewModel> Users { get; set; } = new List<DeliveryUserViewModel>();
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int OnLeaveUsers { get; set; }
        public int InactiveUsers { get; set; }
    }
}
