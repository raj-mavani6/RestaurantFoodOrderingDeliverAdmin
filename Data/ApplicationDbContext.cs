using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverAdmin.Models;

namespace RestaurantFoodOrderingDeliverAdmin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for Delivery Admin Panel
        public DbSet<DeliveryUser> DeliveryUsers { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public DbSet<DeliveryEarning> DeliveryEarnings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DeliveryUser entity
            modelBuilder.Entity<DeliveryUser>(entity =>
            {
                entity.HasKey(e => e.DeliveryUserId);
                
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15);
                
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.PermanentAddress)
                    .HasMaxLength(500);
                
                entity.Property(e => e.CurrentAddress)
                    .HasMaxLength(500);
                
                entity.Property(e => e.City)
                    .HasMaxLength(100);
                
                entity.Property(e => e.State)
                    .HasMaxLength(100);
                
                entity.Property(e => e.Pincode)
                    .HasMaxLength(10);
                
                entity.Property(e => e.VehicleType)
                    .HasMaxLength(50);
                
                entity.Property(e => e.VehicleNumber)
                    .HasMaxLength(20);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");
                
                entity.Property(e => e.Rating)
                    .HasColumnType("decimal(3,1)")
                    .HasDefaultValue(0);
                
                entity.Property(e => e.TotalDeliveries)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.TotalEarnings)
                    .HasColumnType("decimal(12,2)")
                    .HasDefaultValue(0);
                
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                
                // Relationships
                entity.HasMany(e => e.Attendances)
                    .WithOne(a => a.DeliveryUser)
                    .HasForeignKey(a => a.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(e => e.Leaves)
                    .WithOne(l => l.DeliveryUser)
                    .HasForeignKey(l => l.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Attendance entity
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasKey(e => e.AttendanceId);
                entity.ToTable("Attendance");  // Map to singular table name                
                entity.Property(e => e.AttendanceDate)
                    .IsRequired();
                
                entity.Property(e => e.CheckInTime)
                    .HasMaxLength(20);
                
                entity.Property(e => e.CheckOutTime)
                    .HasMaxLength(20);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Present");
                
                entity.Property(e => e.Notes)
                    .HasMaxLength(500);
                
                entity.Property(e => e.OrdersCompleted)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.DistanceCovered)
                    .HasPrecision(8, 2)
                    .HasDefaultValue(0);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany(d => d.Attendances)
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Leave entity
            modelBuilder.Entity<Leave>(entity =>
            {
                entity.HasKey(e => e.LeaveId);
                entity.ToTable("Leaves");  // Try plural table name                
                entity.Property(e => e.StartDate)
                    .IsRequired();
                
                entity.Property(e => e.EndDate)
                    .IsRequired();
                
                entity.Property(e => e.LeaveType)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");
                
                entity.Property(e => e.ApprovedBy)
                    .HasMaxLength(100);
                
                entity.Property(e => e.AdminNotes)
                    .HasMaxLength(500);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany(d => d.Leaves)
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DeliveryOrder entity
            modelBuilder.Entity<DeliveryOrder>(entity =>
            {
                entity.HasKey(e => e.DeliveryOrderId);
                entity.ToTable("DeliveryOrders");
                
                entity.Property(e => e.CustomerName).HasMaxLength(100);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DeliveryFee).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TipAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Distance).HasColumnType("decimal(5,2)");
                
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DeliveryEarning entity
            modelBuilder.Entity<DeliveryEarning>(entity =>
            {
                entity.HasKey(e => e.EarningId);
                entity.ToTable("DeliveryEarning");
                
                entity.Property(e => e.DeliveryFee).HasColumnType("decimal(10,2)");
                entity.Property(e => e.TipAmount).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Bonus).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Incentive).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Deduction).HasColumnType("decimal(10,2)");
                entity.Property(e => e.PaymentStatus).HasMaxLength(50);
                
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Configure Order entity (Main Admin Order)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.ToTable("orders"); // Matches Admin project mapping
                
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Customer entity (Main Admin Customer)
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.ToTable("Customers"); // Matches Admin project convention
            });
        }
    }
}
