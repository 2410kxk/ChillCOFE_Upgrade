using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

[Authorize(Roles = "Admin")]
public class ShiftController : Controller
{
    private readonly AppDbContext _context;
    public ShiftController(AppDbContext context) => _context = context;

    // Hiển thị bảng lịch làm việc
    // Hiển thị bảng phân ca tuần
    public async Task<IActionResult> Index(DateTime? date)
    {
        DateTime selectedDate = date ?? DateTime.Today;
        ViewBag.TargetDate = selectedDate;

        // Lấy ngày đầu tuần (Thứ 2)
        int diff = (7 + (selectedDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateTime startOfWeek = selectedDate.AddDays(-1 * diff).Date;
        DateTime endOfWeek = startOfWeek.AddDays(7).Date;

        // Lấy tất cả ca làm trong tuần đó
        var shifts = await _context.Shifts
            .Include(s => s.Employee)
            .Where(s => s.Date >= startOfWeek && s.Date < endOfWeek)
            .ToListAsync();

        return View(shifts);
    }
    [HttpPost]
    public async Task<IActionResult> AssignMultipleEmployees(List<int> empIds, int shiftNum, DateTime date)
    {
        try
        {
            if (empIds == null || !empIds.Any())
                return Json(new { success = false, message = "Chưa chọn nhân viên nào!" });

            // 1. Xóa các nhân viên cũ trong ca này của ngày này để cập nhật mới
            var existingShifts = _context.Shifts
                .Where(s => s.Date.Date == date.Date && s.ShiftNumber == shiftNum);
            _context.Shifts.RemoveRange(existingShifts);

            // 2. Thêm danh sách mới
            foreach (var id in empIds)
            {
                _context.Shifts.Add(new Shift
                {
                    EmployeeID = id,
                    ShiftNumber = shiftNum,
                    Date = date,
                    Note = "Phân ca nhóm"
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

[HttpPost]
    public async Task<IActionResult> AssignEmployee(int empId, int shiftNum, DateTime date)
    {
        try
        {
            // Kiểm tra xem nhân viên này đã bị trùng ca đó chưa
            var exists = await _context.Shifts.AnyAsync(s => s.EmployeeID == empId && s.Date.Date == date.Date && s.ShiftNumber == shiftNum);
            if (exists) return Json(new { success = false, message = "Nhân viên này đã có lịch trong ca này rồi!" });

            var newShift = new Shift
            {
                EmployeeID = empId,
                ShiftNumber = shiftNum,
                Date = date,
                Note = "Phân ca từ bảng tuần"
            };

            _context.Shifts.Add(newShift);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpPost]
    public async Task<IActionResult> SyncData()
    {
        try
        {
            // 1. Lấy tất cả đơn hàng kèm chi tiết và thông tin bàn
            var orders = await _context.Orders
                .Include(o => o.Details)
                .Include(o => o.Table)
                .ToListAsync();

            foreach (var order in orders)
            {
                // A. Xóa đơn hàng rác (Không có món và đã quá 24h)
                if ((order.Details == null || !order.Details.Any()) && order.OrderTime < DateTime.Now.AddDays(-1))
                {
                    _context.Orders.Remove(order);
                    continue;
                }

                // B. Tính toán lại tổng tiền dựa trên giá thực tế trong OrderDetail
                if (order.Details != null && order.Details.Any())
                {
                    order.TotalAmount = order.Details.Sum(d => d.Quantity * d.Price);
                }

                // C. Đồng bộ trạng thái bàn
                if (order.Table != null)
                {
                    // Nếu đơn hàng cuối cùng của bàn này đã thanh toán, đặt bàn về Trống
                    if (order.Status == "Đã thanh toán")
                    {
                        order.Table.Status = "Trống";
                    }
                    else if (order.Status == "Đang xử lý")
                    {
                        order.Table.Status = "Đang dùng";
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đồng bộ dữ liệu hệ thống thành công!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Lỗi: " + ex.Message });
        }
    }
    [HttpPost]
    public async Task<IActionResult> AssignShift(int employeeId, int shiftNumber, DateTime date)
    {
        var emp = await _context.Employees.FindAsync(employeeId);
        if (emp == null) return Json(new { success = false, message = "Không tìm thấy nhân viên" });

        // 1. Kiểm tra nhân viên đã có lịch trong ca này chưa
        var exists = await _context.Shifts.AnyAsync(s => s.EmployeeID == employeeId && s.Date.Date == date.Date && s.ShiftNumber == shiftNumber);
        if (exists) return Json(new { success = false, message = "Nhân viên này đã có lịch trong ca này rồi!" });

        // 2. Kiểm tra định mức (2 Pha chế, 3 Phục vụ)
        var currentStaffInShift = await _context.Shifts
            .Include(s => s.Employee)
            .Where(s => s.Date.Date == date.Date && s.ShiftNumber == shiftNumber)
            .ToListAsync();

        int countPosition = currentStaffInShift.Count(s => s.Employee?.Position == emp.Position);

        if (emp.Position == "Pha chế" && countPosition >= 2)
            return Json(new { success = false, message = "Ca này đã đủ 2 nhân viên Pha chế!" });

        if (emp.Position == "Phục vụ" && countPosition >= 3)
            return Json(new { success = false, message = "Ca này đã đủ 3 nhân viên Phục vụ!" });

        // 3. Nếu hợp lệ thì lưu
        var newShift = new Shift { EmployeeID = employeeId, ShiftNumber = shiftNumber, Date = date };
        _context.Shifts.Add(newShift);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}