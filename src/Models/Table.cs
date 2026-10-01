using System.ComponentModel.DataAnnotations;

namespace QuanLiQuanCaffee.Models
{
    public class Table
    {
        [Key]
        public int TableID { get; set; }

        public string Name { get; set; } = "";   // Bàn 1, Bàn 2

        public string Status { get; set; } = "Trống"; // Trống / Đang dùng
       
    }
}
