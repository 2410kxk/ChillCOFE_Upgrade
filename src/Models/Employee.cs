
using System.ComponentModel.DataAnnotations;

namespace QuanLiQuanCaffee.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        [Required]
        public string? FullName { get; set; }
        public string? Position { get; set; } // "Pha chế" hoặc "Phục vụ"
        public string? Phone { get; set; }
        public double HourlyRate { get; set; } = 27000; // Mức lương bạn đã đặt
        public string? Email { get; set; }
    }
}
