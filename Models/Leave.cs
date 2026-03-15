using System;

namespace RestaurantFoodOrderingDeliverAdmin.Models
{
    public class Leave
    {
        public int LeaveId { get; set; }
        public int DeliveryUserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? LeaveType { get; set; } // Sick, Casual, Emergency, Other
        public string? Reason { get; set; }
        public string? Status { get; set; } // Pending, Approved, Rejected
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime AppliedDate { get; set; }

        // Navigation Property
        public virtual DeliveryUser? DeliveryUser { get; set; }
    }
}
