using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace QuanLiQuanCaffee.Models;

public class Review
{
    [Key] public int ReviewID { get; set; }
    [Required] public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }
    [Required, StringLength(2000)] public string Content { get; set; } = "";
    [Range(1,5)] public int Rating { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedTime { get; set; }
    public List<ReviewComment> Comments { get; set; } = new();
}
public class ReviewComment
{
    [Key] public int CommentID { get; set; }
    public int ReviewID { get; set; }
    public Review? Review { get; set; }
    [Required] public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }
    [Required, StringLength(1000)] public string Content { get; set; } = "";
    public DateTime CreatedTime { get; set; }
}
public class UserProfile
{
    [Key] public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }
    [StringLength(80)] public string DisplayName { get; set; } = "";
    public string? AvatarUrl { get; set; }
}
public class CafeNotification
{
    public int Id { get; set; }
    [Required,StringLength(120)] public string Title { get; set; } = "";
    [Required,StringLength(4000)] public string Content { get; set; } = "";
    [Required,StringLength(10)] public string Audience { get; set; } = "All";
    [Required] public string AuthorId { get; set; } = "";
    public IdentityUser? Author { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<NotificationRead> Reads { get; set; } = new();
}
public class NotificationRead
{
    public int NotificationId { get; set; }
    public CafeNotification? Notification { get; set; }
    public string UserId { get; set; } = "";
    public IdentityUser? User { get; set; }
    public DateTime ReadAt { get; set; }
}
public class StoreSetting
{
    public int Id { get; set; }
    public bool EnableKitchenPrinter { get; set; }
    public bool EnableReceiptPrinter { get; set; }
    public bool EnableNotificationSound { get; set; }
    [Required,StringLength(100)] public string StoreName { get; set; } = "Chill COFÉ";
    [StringLength(300)] public string Address { get; set; } = "";
    [StringLength(30)] public string Phone { get; set; } = "";
    public string? WifiPassword { get; set; }
    [EmailAddress,StringLength(150)] public string? Email { get; set; }
    [StringLength(150)] public string? OpeningHours { get; set; }
}
