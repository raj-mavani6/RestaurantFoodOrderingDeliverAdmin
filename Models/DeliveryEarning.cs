using System;

namespace RestaurantFoodOrderingDeliverAdmin.Models
{
    public class DeliveryEarning
    {
        public int EarningId { get; set; }
        public int DeliveryUserId { get; set; }
        public int? DeliveryOrderId { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TipAmount { get; set; }
        public decimal Bonus { get; set; }
        public decimal Incentive { get; set; }
        public decimal Deduction { get; set; }
        public DateTime EarningDate { get; set; }
        public string? EarningType { get; set; }
        public string? Description { get; set; }
        public string? PaymentStatus { get; set; }

        // Navigation Property
        public virtual DeliveryUser? DeliveryUser { get; set; }
    }
}
