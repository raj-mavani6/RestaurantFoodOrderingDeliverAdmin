using System;
using System.Collections.Generic;

namespace RestaurantFoodOrderingDeliverAdmin.Models
{
    public class DeliveryUser
    {
        public int DeliveryUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? PermanentAddress { get; set; }
        public string? CurrentAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public string? VehicleType { get; set; }
        public string? VehicleNumber { get; set; }
        public string Status { get; set; }
        public decimal Rating { get; set; }
        public int TotalDeliveries { get; set; }
        public decimal TotalEarnings { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();
    }
}

