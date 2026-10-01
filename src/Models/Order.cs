using System.ComponentModel.DataAnnotations;

namespace QuanLiQuanCaffee.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }

        public int? TableID { get; set; }
        public Table? Table { get; set; }

        public DateTime OrderTime { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Đang xử lý";

        public double TotalAmount { get; set; }

        public List<OrderDetail> Details { get; set; } = new();
        public string? UserId { get; set; }
        public double ChangeAmount { get; set; }
        public double CustomerPaid { get; set; }
        public double DiscountAmount { get; set; }
        public int GuestCount { get; set; } = 1;
        public bool IsPaid { get; set; }
        public string? CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
