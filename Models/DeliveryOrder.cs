using System;

namespace RestaurantFoodOrderingDeliverAdmin.Models
{
    public class DeliveryOrder
    {
        public int DeliveryOrderId { get; set; }
        public int OrderId { get; set; }
        public int DeliveryUserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? PickupAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TipAmount { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public string? SpecialInstructions { get; set; }
        public DateTime OrderTime { get; set; }
        public DateTime AssignedTime { get; set; }
        public DateTime? PickupTime { get; set; }
        public DateTime? DeliveredTime { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
        public decimal Distance { get; set; }
        public string? DeliveryNotes { get; set; }
        public int? CustomerRating { get; set; }
        public string? CustomerFeedback { get; set; }

        // Navigation Property
        public virtual DeliveryUser? DeliveryUser { get; set; }
    }
}
