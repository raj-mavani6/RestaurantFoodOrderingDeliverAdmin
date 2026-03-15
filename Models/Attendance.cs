using System;

namespace RestaurantFoodOrderingDeliverAdmin.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int DeliveryUserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? IntermediateStartTime { get; set; }
        public string? IntermediateEndTime { get; set; }
        public string? InTimeReason { get; set; }
        public string? OutTimeReason { get; set; }
        public string? IntermediateStartReason { get; set; }
        public string? IntermediateEndReason { get; set; }
        public string? Status { get; set; } // Present, Absent, Late, Half Day
        public string? Notes { get; set; }
        public int OrdersCompleted { get; set; }
        public double DistanceCovered { get; set; } // in kilometers

        // Navigation Property
        public virtual DeliveryUser? DeliveryUser { get; set; }
    }
}
