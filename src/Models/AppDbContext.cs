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

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Shift> Shifts { get; set; }

        // --- PHẦN CẬP NHẬT THÊM ---
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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