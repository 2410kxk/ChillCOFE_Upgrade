    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace QuanLiQuanCaffee.Models
    {
        public class Product
        {
            [Key]
            public int ProductID { get; set; }

            [Required]
            public string Name { get; set; } = "";

            [Range(0, 10000000)]
            public double Price { get; set; }
            public bool IsAvailable { get; set; } = true;
            public bool SupportsCustomization { get; set; } = true;

            [Required(ErrorMessage = "Vui lòng chọn loại")]
            public int CategoryID { get; set; }

            [ForeignKey("CategoryID")]
            public Category? Category { get; set; } // 👈 CHO NULL
            public string? ImageUrl { get; set; }

            public string? Description { get; set; }
        }
    }