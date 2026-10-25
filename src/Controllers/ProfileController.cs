using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.Services;
using QuanLiQuanCaffee.ViewModels;
namespace QuanLiQuanCaffee.Controllers;
[Authorize,AutoValidateAntiforgeryToken]
public class ProfileController(AppDbContext db,UserManager<IdentityUser> users,SignInManager<IdentityUser> signIn,ImageUpload upload):Controller
{
    public async Task<IActionResult> Index()
    {
        var u=await users.GetUserAsync(User);if(u==null)return Challenge();var p=await db.UserProfiles.FindAsync(u.Id);
        return View(new ProfileInput{DisplayName=p?.DisplayName??u.UserName??"",Phone=u.PhoneNumber,AvatarUrl=p?.AvatarUrl,Email=u.Email});
    }
    [HttpPost,RequestSizeLimit(6*1024*1024)]
    public async Task<IActionResult> Index(ProfileInput input)
    {
        var u=await users.GetUserAsync(User);if(u==null)return Challenge();var p=await db.UserProfiles.FindAsync(u.Id);
        input.Email=u.Email;input.AvatarUrl=p?.AvatarUrl;
        if(!ModelState.IsValid)return View(input);
        string? image=null;var old=p?.AvatarUrl;
        try{
            image=await upload.Save(input.Avatar);
            await using var tx=await db.Database.BeginTransactionAsync();
            var phoneResult=await users.SetPhoneNumberAsync(u,input.Phone?.Trim());
            if(!phoneResult.Succeeded){upload.Delete(image);foreach(var e in phoneResult.Errors)ModelState.AddModelError("",e.Description);return View(input);}
            if(p==null){p=new UserProfile{UserId=u.Id};db.UserProfiles.Add(p);}
            p.DisplayName=input.DisplayName.Trim();p.AvatarUrl=image??p.AvatarUrl;await db.SaveChangesAsync();await tx.CommitAsync();
        }catch(ArgumentException e){ModelState.AddModelError("",e.Message);return View(input);}catch{upload.Delete(image);throw;}
        if(image!=null)upload.Delete(old);await signIn.RefreshSignInAsync(u);TempData["Success"]="Đã lưu hồ sơ.";return RedirectToAction(nameof(Index));
    }
    public IActionResult Password()=>View(new PasswordInput());
    [HttpPost]
    public async Task<IActionResult> Password(PasswordInput input)
    {
        if(!ModelState.IsValid)return View(input);var u=await users.GetUserAsync(User);if(u==null)return Challenge();
        var result=await users.ChangePasswordAsync(u,input.CurrentPassword,input.NewPassword);
        if(!result.Succeeded){foreach(var e in result.Errors)ModelState.AddModelError("",e.Code=="PasswordMismatch"?"Mật khẩu hiện tại chưa đúng.":e.Description);return View(input);}
        await signIn.RefreshSignInAsync(u);TempData["Success"]="Đã đổi mật khẩu.";return RedirectToAction(nameof(Index));
    }
}
