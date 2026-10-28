using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[Authorize(Roles="Admin"),AutoValidateAntiforgeryToken]
public class StoreSettingsController(AppDbContext db):Controller
{
    public async Task<IActionResult> Index()=>View(await db.StoreSettings.OrderBy(s=>s.Id).FirstOrDefaultAsync()??new StoreSetting());
    [HttpPost]
    public async Task<IActionResult> Index([Bind("StoreName,Address,Phone,Email,OpeningHours")]StoreSetting input)
    {
        if(!ModelState.IsValid)return View(input);
        var s=await db.StoreSettings.OrderBy(s=>s.Id).FirstOrDefaultAsync();if(s==null){s=new StoreSetting();db.StoreSettings.Add(s);}
        s.StoreName=input.StoreName.Trim();s.Address=input.Address?.Trim()??"";s.Phone=input.Phone?.Trim()??"";s.Email=input.Email?.Trim();s.OpeningHours=input.OpeningHours?.Trim();await db.SaveChangesAsync();TempData["Success"]="Đã lưu thông tin quán.";return RedirectToAction(nameof(Index));
    }
}
