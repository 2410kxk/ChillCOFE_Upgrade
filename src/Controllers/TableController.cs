using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TableController : Controller
    {
        private readonly AppDbContext _context;
        public TableController(AppDbContext context) => _context = context;

        // Hiển thị danh sách bàn trong trang Admin
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tables.OrderBy(t => t.Name).ToListAsync());
        }

        // Tạo bàn mới
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                _context.Tables.Add(new Table { Name = name, Status = "Trống" });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Xóa bàn
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null)
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}