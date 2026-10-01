using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
namespace QuanLiQuanCaffee.Controllers;
[Authorize(Roles="Admin")]
[AutoValidateAntiforgeryToken]
public class CatalogAdminController : Controller
{
    private readonly AppDbContext db;
    public CatalogAdminController(AppDbContext db) { this.db=db; }
    public async Task<IActionResult> Index() => View(await db.Products.Include(p=>p.Category).OrderBy(p=>p.CategoryID).ThenBy(p=>p.Name).ToListAsync());
    [HttpPost]
    public async Task<IActionResult> Availability(int id, bool available)
    {
        var product=await db.Products.FindAsync(id); if(product==null) return NotFound();
        product.IsAvailable=available; await db.SaveChangesAsync(); return RedirectToAction(nameof(Index));
    }
}
