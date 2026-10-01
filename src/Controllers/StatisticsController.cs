using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

[Authorize(Roles = "Admin")]
public class StatisticsController : Controller
{
    private readonly AppDbContext _context;
    public StatisticsController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Revenue()
    {
        var today = DateTime.Today;

        // 1. Thống kê tổng quan
        ViewBag.TotalOrders = await _context.Orders.CountAsync(o => o.Status == "Đã thanh toán");
        ViewBag.TodayRevenue = await _context.Orders
            .Where(o => o.OrderTime.Date == today && o.Status == "Đã thanh toán")
            .SumAsync(o => o.TotalAmount);

        ViewBag.MonthRevenue = await _context.Orders
            .Where(o => o.OrderTime.Month == today.Month && o.OrderTime.Year == today.Year && o.Status == "Đã thanh toán")
            .SumAsync(o => o.TotalAmount);

        // 2. Lấy dữ liệu cho biểu đồ (7 ngày gần nhất)
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-i))
            .OrderBy(d => d)
            .ToList();

        var revenueData = await _context.Orders
            .Where(o => o.OrderTime >= today.AddDays(-7) && o.Status == "Đã thanh toán")
            .GroupBy(o => o.OrderTime.Date)
            .Select(g => new { Date = g.Key, Amount = g.Sum(x => x.TotalAmount) })
            .ToListAsync();

        ViewBag.ChartLabels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
        ViewBag.ChartValues = last7Days.Select(d => revenueData.FirstOrDefault(r => r.Date == d)?.Amount ?? 0).ToList();

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetRevenueDetail()
    {
        var today = DateTime.Today;

        // 1. Lấy danh sách đơn hàng đã thanh toán trong ngày
        var ordersToday = await _context.Orders
            .Where(o => o.OrderTime.Date == today && o.Status == "Đã thanh toán")
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .ToListAsync();

        // 2. Tính toán các con số tổng quát
        var totalOrders = ordersToday.Count;
        var totalSales = ordersToday.Sum(o => o.TotalAmount);

        // 3. Thống kê số lượng từng món bán ra
        var productStats = ordersToday
            .SelectMany(o => o.Details)
            .GroupBy(d => d.Product.Name)
            .Select(g => new
            {
                name = g.Key,
                qty = g.Sum(x => x.Quantity)
            })
            .ToList();

        return Json(new
        {
            date = today.ToString("dd/MM/yyyy"),
            totalOrders = totalOrders,
            totalSales = totalSales,
            netSales = totalSales, // Thực thu (có thể trừ chiết khấu nếu có)
            productStats = productStats
        });
    }
    [HttpPost]
    public async Task<IActionResult> SyncData()
    {
        try
        {
            // Logic đồng bộ: Tính toán lại TotalAmount cho các đơn hàng chưa có tổng tiền
            var orders = await _context.Orders
                .Include(o => o.Details)
                .Where(o => o.TotalAmount == 0)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.TotalAmount = order.Details.Sum(d => d.Quantity * d.Price);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Đồng bộ dữ liệu thành công!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}