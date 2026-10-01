using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<JsonResult> GetEmployeesList()
        {
            // Lấy id, tên và chức vụ của tất cả nhân viên
            var list = await _context.Employees
                .Select(e => new { e.EmployeeID, e.FullName, e.Position })
                .ToListAsync();
            return Json(list);
        }
        // 1. HIỂN THỊ DANH SÁCH NHÂN VIÊN
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }
        // Action Hiển thị Chi tiết (Bạn đang thiếu cái này)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (employee == null) return NotFound();

            return View(employee);
        }
        // 2. TẠO MỚI (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. TẠO MỚI (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee emp)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emp);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(emp);
        }

        // 4. CHỈNH SỬA (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        // 5. CHỈNH SỬA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee emp)
        {
            if (id != emp.EmployeeID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emp);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(emp.EmployeeID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(emp);
        }

        // 6. XÓA (GET: Xác nhận xóa)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (employee == null) return NotFound();

            return View(employee);
        }

        // 7. XÓA (POST: Thực hiện xóa)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                // Lưu ý: Nếu nhân viên đã có lịch làm việc trong bảng Shifts, 
                // bạn cần xử lý xóa hoặc báo lỗi để tránh vi phạm khóa ngoại.
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Hàm kiểm tra tồn tại
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }
    }
}