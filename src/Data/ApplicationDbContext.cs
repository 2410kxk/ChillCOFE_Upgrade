using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Data
{
    // IdentityDbContext giúp quản lý tài khoản Admin/Staff đã có sẵn trong ASP.NET Core Identity
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- KHAI BÁO CÁC BẢNG DỮ LIỆU (DBSET) ---
        public DbSet<Employee> Employees { get; set; }      // Nhân viên
        public DbSet<Shift> Shifts { get; set; }            // Ca làm việc
        public DbSet<Table> Tables { get; set; }            // Bàn (Phòng)
        public DbSet<Product> Products { get; set; }        // Món ăn/Đồ uống
        public DbSet<Order> Orders { get; set; }            // Hóa đơn (Đơn hàng)
        public DbSet<OrderDetail> OrderDetails { get; set; } // Chi tiết hóa đơn

        // --- CẤU HÌNH QUAN HỆ GIỮA CÁC BẢNG (FLUENT API) ---
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1. Quan hệ giữa Order và OrderDetail (1 Hóa đơn có nhiều món)
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Order thì tự xóa Details

            // 2. Quan hệ giữa OrderDetail và Product
            builder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductID);

            // 3. Quan hệ giữa Order và Table (1 Bàn có thể có nhiều đơn hàng theo thời gian)
            builder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany()
                .HasForeignKey(o => o.TableID)
                .OnDelete(DeleteBehavior.SetNull);

            // 4. Quan hệ giữa Shift và Employee (1 Nhân viên có nhiều ca làm)
            builder.Entity<Shift>()
                .HasOne(s => s.Employee)
                .WithMany()
                .HasForeignKey(s => s.EmployeeID)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. Cấu hình kiểu dữ liệu tiền tệ cho chính xác (Decimal/Double)
            builder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("float"); // Nếu dùng double thì map với float trong SQL

            builder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasColumnType("float");
        }
    }
}