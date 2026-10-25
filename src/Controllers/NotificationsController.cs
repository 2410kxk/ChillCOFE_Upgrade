using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.Services;
using QuanLiQuanCaffee.ViewModels;
namespace QuanLiQuanCaffee.Controllers;
[Authorize,AutoValidateAntiforgeryToken]
public class NotificationsController(AppDbContext db):Controller
{
    public async Task<IActionResult> Index(int page=1)
    {
        var id=User.FindFirstValue(ClaimTypes.NameIdentifier)!;var q=NotificationAccess.Visible(db,User).AsNoTracking();
        var pages=Math.Max(1,((await q.CountAsync())+19)/20);page=Math.Clamp(page,1,pages);
        var items=await q.OrderByDescending(n=>n.Id).Skip((page-1)*20).Take(20).ToListAsync();var ids=items.Select(n=>n.Id).ToList();
        return View(new NotificationsVm{Items=items,ReadIds=(await db.NotificationReads.Where(r=>r.UserId==id&&ids.Contains(r.NotificationId)).Select(r=>r.NotificationId).ToListAsync()).ToHashSet(),Page=page,Pages=pages});
    }
    [HttpPost]
    public async Task<IActionResult> Read(int id)
    {
        if(!await NotificationAccess.Visible(db,User).AnyAsync(n=>n.Id==id))return NotFound();
        var uid=User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        // Unique key plus serializable transaction makes repeated reads idempotent.
        await using var tx=await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        if(!await db.NotificationReads.AnyAsync(r=>r.NotificationId==id&&r.UserId==uid)){db.NotificationReads.Add(new NotificationRead{NotificationId=id,UserId=uid,ReadAt=CafeClock.Now});await db.SaveChangesAsync();}
        await tx.CommitAsync();return RedirectToAction(nameof(Index));
    }
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> Manage(int page=1)
    {
        var pages=Math.Max(1,((await db.Notifications.CountAsync())+19)/20);page=Math.Clamp(page,1,pages);
        ViewBag.Page=page;ViewBag.Pages=pages;return View(await db.Notifications.AsNoTracking().OrderByDescending(n=>n.Id).Skip((page-1)*20).Take(20).ToListAsync());
    }
    [HttpPost,Authorize(Roles="Admin")]
    public async Task<IActionResult> Create(string? title,string? content,string audience)
    {
        if(string.IsNullOrWhiteSpace(title)||title.Trim().Length>120||string.IsNullOrWhiteSpace(content)||content.Trim().Length>4000||!new[]{"All","User","Staff"}.Contains(audience)){TempData["Error"]="Tiêu đề tối đa 120 ký tự, nội dung tối đa 4.000 ký tự và nhóm nhận hợp lệ.";return RedirectToAction(nameof(Manage));}
        db.Notifications.Add(new CafeNotification{Title=title.Trim(),Content=content.Trim(),Audience=audience,AuthorId=User.FindFirstValue(ClaimTypes.NameIdentifier)!,CreatedAt=CafeClock.Now});await db.SaveChangesAsync();TempData["Success"]="Đã đăng thông báo.";return RedirectToAction(nameof(Manage));
    }
    [HttpPost,Authorize(Roles="Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var n=await db.Notifications.FindAsync(id);if(n==null)return NotFound();db.Notifications.Remove(n);await db.SaveChangesAsync();return RedirectToAction(nameof(Manage));
    }
}
