using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.ViewModels;
namespace QuanLiQuanCaffee.Services;
public static class CafeClock { public static DateTime Now => DateTime.UtcNow.AddHours(7); }
public static class NotificationAccess
{
    public static IQueryable<CafeNotification> Visible(AppDbContext db,ClaimsPrincipal user)
    {
        if(user.IsInRole("Admin")) return db.Notifications;
        var audience=user.IsInRole("Staff")?"Staff":"User";
        return db.Notifications.Where(n=>n.Audience=="All" || n.Audience==audience);
    }
}
public class ImageUpload(IWebHostEnvironment env)
{
    public async Task<string?> Save(IFormFile? image)
    {
        if(image==null || image.Length==0)return null;
        if(image.Length>5*1024*1024)throw new ArgumentException("Ảnh tối đa 5 MB.");
        await using var input=image.OpenReadStream();
        using var data=new MemoryStream();await input.CopyToAsync(data);
        var bytes=data.ToArray();string? ext=null;
        if(bytes.Length>=8 && bytes.Take(8).SequenceEqual(new byte[]{137,80,78,71,13,10,26,10}))ext=".png";
        else if(bytes.Length>=3&&bytes[0]==255&&bytes[1]==216&&bytes[2]==255)ext=".jpg";
        else if(bytes.Length>=12&&System.Text.Encoding.ASCII.GetString(bytes,0,4)=="RIFF"&&System.Text.Encoding.ASCII.GetString(bytes,8,4)=="WEBP")ext=".webp";
        if(ext==null)throw new ArgumentException("Chỉ nhận ảnh JPG, PNG hoặc WebP hợp lệ.");
        var name=Guid.NewGuid().ToString("N")+ext;
        var folder=Path.Combine(env.WebRootPath,"uploads","community");Directory.CreateDirectory(folder);
        await File.WriteAllBytesAsync(Path.Combine(folder,name),bytes);
        return "/uploads/community/"+name;
    }
    public void Delete(string? url)
    {
        if(url==null||!url.StartsWith("/uploads/community/",StringComparison.Ordinal))return;
        var name=url["/uploads/community/".Length..];
        if(name!=Path.GetFileName(name)||name.Contains('\\'))return;
        try{File.Delete(Path.Combine(env.WebRootPath,"uploads","community",name));}catch(IOException){}catch(UnauthorizedAccessException){}
    }
}
public class SiteData(AppDbContext db,IHttpContextAccessor accessor)
{
    private Task<SiteVm>? cached;
    public Task<SiteVm> Load()=>cached??=Read();
    private async Task<SiteVm> Read()
    {
        var vm=new SiteVm{Categories=await db.Categories.AsNoTracking().OrderBy(c=>c.Name).ToListAsync(),Products=await db.Products.AsNoTracking().Where(p=>p.IsAvailable).OrderBy(p=>p.Name).ToListAsync(),Store=await db.StoreSettings.AsNoTracking().OrderBy(s=>s.Id).FirstOrDefaultAsync()??new()};
        var user=accessor.HttpContext!.User;var id=user.FindFirstValue(ClaimTypes.NameIdentifier);
        if(id!=null){var profile=await db.UserProfiles.AsNoTracking().FirstOrDefaultAsync(p=>p.UserId==id);vm.Name=string.IsNullOrWhiteSpace(profile?.DisplayName)?user.Identity?.Name??"Tài khoản":profile.DisplayName;vm.Avatar=profile?.AvatarUrl;vm.Unread=await NotificationAccess.Visible(db,user).CountAsync(n=>!n.Reads.Any(r=>r.UserId==id));}
        return vm;
    }
}
