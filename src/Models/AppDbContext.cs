using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace QuanLiQuanCaffee.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
              : base(options)
        {
        }

        public DbSet<CafeCustomer> CafeCustomers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<ReviewComment> ReviewComments => Set<ReviewComment>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<CafeNotification> Notifications => Set<CafeNotification>();
        public DbSet<NotificationRead> NotificationReads => Set<NotificationRead>();
        public DbSet<StoreSetting> StoreSettings => Set<StoreSetting>();

        // --- PHẦN CẬP NHẬT THÊM ---
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Order>().HasIndex(o=>o.CheckoutKey).IsUnique().HasFilter("[CheckoutKey] IS NOT NULL");
            builder.Entity<Review>().Property(x=>x.CreatedTime).HasColumnType("datetime");
            builder.Entity<ReviewComment>().Property(x=>x.CreatedTime).HasColumnType("datetime");
            builder.Entity<Review>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ReviewComment>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ReviewComment>().HasOne(x=>x.Review).WithMany(x=>x.Comments).HasForeignKey(x=>x.ReviewID).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<UserProfile>().HasOne(x=>x.User).WithOne().HasForeignKey<UserProfile>(x=>x.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<CafeNotification>().ToTable("CafeNotifications");
            builder.Entity<CafeNotification>().HasOne(x=>x.Author).WithMany().HasForeignKey(x=>x.AuthorId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<NotificationRead>().HasKey(x=>new{x.NotificationId,x.UserId});
            builder.Entity<NotificationRead>().HasOne(x=>x.Notification).WithMany(x=>x.Reads).HasForeignKey(x=>x.NotificationId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<NotificationRead>().HasOne(x=>x.User).WithMany().HasForeignKey(x=>x.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CafeCustomer>().HasIndex(c => c.Phone).IsUnique();
            builder.Entity<Order>().HasOne(o => o.CafeCustomer).WithMany()
                .HasForeignKey(o => o.CafeCustomerId).OnDelete(DeleteBehavior.Restrict);

            // 1. Cấu hình để OrderDetail biết nó thuộc về Order nào (Quan hệ 1-N)
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade); // Xóa hóa đơn thì tự xóa chi tiết

            // 2. Cấu hình để OrderDetail kết nối với Product (Để lấy tên món ăn)
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductID);

            // 3. Định nghĩa kiểu dữ liệu tiền tệ cho chính xác
            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("float");

            builder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("float");
        }
    }
}
