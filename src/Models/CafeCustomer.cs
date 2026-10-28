using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLiQuanCaffee.Models;
[Table("CafeCustomers", Schema = "dbo")]
public class CafeCustomer
{
    [Key] public int CafeCustomerId { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [Required, MaxLength(20)] public string Phone { get; set; } = "";
    [MaxLength(200)] public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
