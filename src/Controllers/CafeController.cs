using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;
using QuanLiQuanCaffee.ViewModels;
namespace QuanLiQuanCaffee.Controllers;
public class CafeController : Controller
{
    private readonly AppDbContext db;
    public CafeController(AppDbContext db) { this.db = db; }
    public async Task<IActionResult> Menu() => View(new CafeMenuVm {
        Products = await db.Products.AsNoTracking().Include(p => p.Category).OrderBy(p => p.CategoryID).ThenBy(p => p.Name).ToListAsync(),
        Categories = await db.Categories.AsNoTracking().OrderBy(c => c.CategoryID).ToListAsync()
    });
}
