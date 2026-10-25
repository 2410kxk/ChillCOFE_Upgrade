using System.ComponentModel.DataAnnotations;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.ViewModels;
public class ReviewInput
{
    [Required(ErrorMessage="Hãy chia sẻ trải nghiệm của bạn."),StringLength(2000)] public string Content { get; set; } = "";
    [Range(1,5,ErrorMessage="Chọn từ 1 đến 5 sao.")] public int Rating { get; set; } = 5;
    public IFormFile? Image { get; set; }
}
public record PersonLabel(string Name,string Role,string? Avatar);
public class ReviewsVm
{
    public List<Review> Items { get; set; } = new();
    public Dictionary<string,PersonLabel> People { get; set; } = new();
    public int Page { get; set; }
    public int Pages { get; set; }
    public int Count { get; set; }
    public double Average { get; set; }
    public bool Manage { get; set; }
}
public class ProfileInput
{
    [Required,StringLength(80)] public string DisplayName { get; set; } = "";
    [Phone,StringLength(30)] public string? Phone { get; set; }
    public IFormFile? Avatar { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Email { get; set; }
}
public class PasswordInput
{
    [Required,DataType(DataType.Password)] public string CurrentPassword { get; set; } = "";
    [Required,MinLength(6),DataType(DataType.Password)] public string NewPassword { get; set; } = "";
    [Compare(nameof(NewPassword)),DataType(DataType.Password)] public string ConfirmPassword { get; set; } = "";
}
public class NotificationsVm
{
    public List<CafeNotification> Items { get; set; } = new();
    public HashSet<int> ReadIds { get; set; } = new();
    public int Page { get; set; }
    public int Pages { get; set; }
}
public class SiteVm
{
    public List<Category> Categories { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public StoreSetting Store { get; set; } = new();
    public string Name { get; set; } = "";
    public string? Avatar { get; set; }
    public int Unread { get; set; }
}
