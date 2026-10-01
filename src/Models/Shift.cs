using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLiQuanCaffee.Models
{
    public class Shift
    {
        [Key]
        public int ShiftID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, 3)]
        public int ShiftNumber { get; set; } // 1, 2, 3

        public string? Note { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee? Employee { get; set; }
    }
}