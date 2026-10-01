using System.ComponentModel.DataAnnotations;

namespace QuanLiQuanCaffee.Models
{
    public class OrderDetail
    {
        [Key]
        public int DetailID { get; set; }

        public int OrderID { get; set; }
        public Order? Order { get; set; }

        public int ProductID { get; set; }
        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }
        [MaxLength(1)] public string Size { get; set; } = "M";
        public int Sugar { get; set; } = 100;
        public int Ice { get; set; } = 100;
        [MaxLength(40)] public string Topping { get; set; } = "Không";
        [MaxLength(200)] public string Note { get; set; } = "";
    }
}
