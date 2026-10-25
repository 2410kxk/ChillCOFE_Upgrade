using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.Services;
using QuanLiQuanCaffee.ViewModels;
namespace QuanLiQuanCaffee.Controllers;
[AutoValidateAntiforgeryToken]
public class ReviewsController(AppDbContext db,ImageUpload upload):Controller
{
    [AllowAnonymous] public async Task<IActionResult> Index(int page=1)=>View(await List(page,false));
    [Authorize(Roles="Admin")] public async Task<IActionResult> Manage(int page=1)=>View("Index",await List(page,true));
    private async Task<ReviewsVm> List(int page,bool manage)
    {
        var count=await db.Reviews.CountAsync();var pages=Math.Max(1,(count+9)/10);page=Math.Clamp(page,1,pages);
        var items=await db.Reviews.AsNoTracking().Include(r=>r.Comments).OrderByDescending(r=>r.CreatedTime).ThenByDescending(r=>r.ReviewID).Skip((page-1)*10).Take(10).ToListAsync();
        var ids=items.Select(r=>r.UserId).Concat(items.SelectMany(r=>r.Comments.Select(c=>c.UserId))).Distinct().ToList();
        var users=await db.Users.AsNoTracking().Where(u=>ids.Contains(u.Id)).ToListAsync();
        var profiles=await db.UserProfiles.AsNoTracking().Where(p=>ids.Contains(p.UserId)).ToDictionaryAsync(p=>p.UserId);
        var roles=await (from ur in db.UserRoles join role in db.Roles on ur.RoleId equals role.Id where ids.Contains(ur.UserId) select new{ur.UserId,role.Name}).ToListAsync();
        var people=users.ToDictionary(u=>u.Id,u=>{
            profiles.TryGetValue(u.Id,out var p);var rs=roles.Where(r=>r.UserId==u.Id).Select(r=>r.Name).ToList();
            return new PersonLabel(string.IsNullOrWhiteSpace(p?.DisplayName)?u.UserName??"Thành viên":p.DisplayName,rs.Contains("Admin")?"Manager":rs.Contains("Staff")?"Staff":"",p?.AvatarUrl);
        });
        return new ReviewsVm{Items=items,People=people,Page=page,Pages=pages,Count=count,Average=await db.Reviews.Select(r=>(double?)r.Rating).AverageAsync()??0,Manage=manage};
    }
    [HttpPost,Authorize,RequestSizeLimit(6*1024*1024)]
    public async Task<IActionResult> Create(ReviewInput input)
    {
        if(string.IsNullOrWhiteSpace(input.Content))ModelState.AddModelError("Content","Hãy nhập nội dung đánh giá.");
        if(!ModelState.IsValid){TempData["Error"]="Nhập nội dung tối đa 2.000 ký tự và chọn 1–5 sao.";return RedirectToAction(nameof(Index));}
        string? image=null;
        try{image=await upload.Save(input.Image);db.Reviews.Add(new Review{UserId=User.FindFirstValue(ClaimTypes.NameIdentifier)!,Rating=input.Rating,Content=input.Content.Trim(),ImageUrl=image,CreatedTime=CafeClock.Now});await db.SaveChangesAsync();}
        catch(ArgumentException e){TempData["Error"]=e.Message;return RedirectToAction(nameof(Index));}
        catch{upload.Delete(image);throw;}
        TempData["Success"]="Cảm ơn bạn đã chia sẻ trải nghiệm.";return RedirectToAction(nameof(Index));
    }
    [HttpPost,Authorize]
    public async Task<IActionResult> Comment(int id,string? content)
    {
        if(!await db.Reviews.AnyAsync(r=>r.ReviewID==id))return NotFound();
        if(string.IsNullOrWhiteSpace(content)||content.Trim().Length>1000){TempData["Error"]="Bình luận từ 1 đến 1.000 ký tự.";return RedirectToAction(nameof(Index));}
        db.ReviewComments.Add(new ReviewComment{ReviewID=id,UserId=User.FindFirstValue(ClaimTypes.NameIdentifier)!,Content=content.Trim(),CreatedTime=CafeClock.Now});await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    [HttpGet,Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var r=await db.Reviews.FindAsync(id);if(r==null)return NotFound();if(r.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        ViewBag.Id=id;return View(new ReviewInput{Content=r.Content,Rating=r.Rating});
    }
    [HttpPost,Authorize,RequestSizeLimit(6*1024*1024)]
    public async Task<IActionResult> Edit(int id,ReviewInput input)
    {
        var r=await db.Reviews.FindAsync(id);if(r==null)return NotFound();if(r.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        ViewBag.Id=id;if(!ModelState.IsValid||string.IsNullOrWhiteSpace(input.Content))return View(input);
        string? image=null;var old=r.ImageUrl;
        try{image=await upload.Save(input.Image);r.Content=input.Content.Trim();r.Rating=input.Rating;r.ImageUrl=image??old;await db.SaveChangesAsync();}
        catch(ArgumentException e){ModelState.AddModelError("",e.Message);return View(input);}catch{upload.Delete(image);throw;}
        if(image!=null)upload.Delete(old);return RedirectToAction(nameof(Index));
    }
    [HttpPost,Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var r=await db.Reviews.Include(r=>r.Comments).FirstOrDefaultAsync(r=>r.ReviewID==id);if(r==null)return NotFound();
        if(!User.IsInRole("Admin")&&r.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        db.ReviewComments.RemoveRange(r.Comments);db.Reviews.Remove(r);await db.SaveChangesAsync();upload.Delete(r.ImageUrl);
        TempData["Success"]="Đã xóa đánh giá.";return RedirectToAction(User.IsInRole("Admin")?nameof(Manage):nameof(Index));
    }
    [HttpPost,Authorize]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var c=await db.ReviewComments.FindAsync(id);if(c==null)return NotFound();
        if(!User.IsInRole("Admin")&&c.UserId!=User.FindFirstValue(ClaimTypes.NameIdentifier))return Forbid();
        db.ReviewComments.Remove(c);await db.SaveChangesAsync();return RedirectToAction(User.IsInRole("Admin")?nameof(Manage):nameof(Index));
    }
}
