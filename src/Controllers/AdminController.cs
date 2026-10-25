using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Data;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Controllers
{
    [Authorize(Roles = "Admin"), AutoValidateAntiforgeryToken]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index() => View();

        // Trang hiển thị thống kê chính
        public async Task<IActionResult> Revenue()
        {
            var today = DateTime.Today;

            // Chỉ ghi nhận doanh thu của đơn đã thanh toán.
            var validOrders = _context.Orders
                .Where(o => o.Status == "Đã thanh toán");

            // Thống kê card
            ViewBag.TotalOrders = await validOrders.CountAsync(o => (o.PaidAt ?? o.OrderTime).Date == today);

            ViewBag.TodayRevenue = await validOrders
                .Where(o => (o.PaidAt ?? o.OrderTime).Date == today)
                .SumAsync(o => o.TotalAmount);

            ViewBag.MonthRevenue = await validOrders
                .Where(o => (o.PaidAt ?? o.OrderTime).Month == DateTime.Now.Month && (o.PaidAt ?? o.OrderTime).Year == DateTime.Now.Year)
                .SumAsync(o => o.TotalAmount);

            // Dữ liệu biểu đồ 7 ngày
            var last7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).OrderBy(d => d).ToList();
            var chartLabels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
            var chartValues = new List<double>();

            foreach (var day in last7Days)
            {
                var dailyTotal = await validOrders
                    .Where(o => (o.PaidAt ?? o.OrderTime).Date == day.Date)
                    .SumAsync(o => o.TotalAmount);
                chartValues.Add(dailyTotal);
            }

            ViewBag.ChartLabels = chartLabels;
            ViewBag.ChartValues = chartValues;

            return View("~/Views/Statistics/Revenue.cshtml");
        }

        // API lấy dữ liệu chi tiết cho Modal
        [HttpGet]
        public async Task<IActionResult> GetRevenueDetail(string date)
        {
            DateTime targetDate;
            if (!DateTime.TryParse(date, out targetDate))
            {
                targetDate = DateTime.Today;
            }

            var orders = await _context.Orders
                .Include(o => o.Details)
                    .ThenInclude(d => d.Product)
                .Where(o => (o.PaidAt ?? o.OrderTime).Date == targetDate.Date &&
                           (o.Status == "Đã thanh toán"))
                .ToListAsync();

            var totalSales = orders.Sum(o => o.TotalAmount);
            var discount = 0;
            var netSales = totalSales - discount;

            var productStats = orders
                .SelectMany(o => o.Details)
                .GroupBy(d => d.Product?.Name ?? "Món đã lưu")
                .Select(group => new
                {
                    name = group.Key,
                    qty = group.Sum(x => x.Quantity),
                    total = group.Sum(x => x.Price * x.Quantity)
                })
                .OrderByDescending(x => x.qty)
                .ToList();

            return Json(new
            {
                date = targetDate.ToString("dd/MM/yyyy"),
                totalOrders = orders.Count,
                totalSales = totalSales,
                netSales = netSales,
                productStats = productStats
            });
        }

        public IActionResult CreateStaff() => View();

        [HttpPost]
        public async Task<IActionResult> CreateStaff(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");
                ViewBag.Msg = "Tạo nhân viên thành công!";
            }
            else
            {
                ViewBag.Msg = "Lỗi tạo tài khoản!";
            }
            return View();
        }
    }
}