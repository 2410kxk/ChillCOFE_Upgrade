using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.ViewModels;
public class CafeMenuVm
{
    public List<Product> Products { get; set; } = new();
    public List<Category> Categories { get; set; } = new();
}
public class PosVm : CafeMenuVm
{
    public Table Table { get; set; } = new();
}
