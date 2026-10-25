using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Controllers
{
    [Authorize(Roles = "Admin")]
    [AutoValidateAntiforgeryToken]
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
        public async Task<IActionResult> Create(string name, string? note)
        {
            if (!string.IsNullOrWhiteSpace(name) && name.Length <= 80 && (note?.Length ?? 0) <= 300)
            {
                _context.Tables.Add(new Table { Name = name.Trim(), Note = note?.Trim(), Status = "Trống" });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNote(int id, string? note)
        {
            if ((note?.Length ?? 0) > 300) return BadRequest("Ghi chú tối đa 300 ký tự.");
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return NotFound();
            table.Note = note?.Trim(); await _context.SaveChangesAsync();
            TempData["Success"] = "Đã lưu ghi chú bàn.";
            return RedirectToAction(nameof(Index));
        }
        // Xóa bàn
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table != null && await _context.Orders.AnyAsync(o => o.TableID == id))
            {
                TempData["Error"] = "Bàn có lịch sử đơn hàng, không thể xóa.";
                return RedirectToAction(nameof(Index));
            }
            if (table != null)
            {
                _context.Tables.Remove(table);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}