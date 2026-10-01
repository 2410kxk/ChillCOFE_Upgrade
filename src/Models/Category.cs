using System.ComponentModel.DataAnnotations;

namespace QuanLiQuanCaffee.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        public string Name { get; set; } = "";

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
