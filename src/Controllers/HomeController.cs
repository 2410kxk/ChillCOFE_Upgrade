using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

namespace QuanLiQuanCaffee.Controllers
{
    public class HomeController : BaseController
    {
        private new  readonly AppDbContext _context;

        public HomeController(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // 👉 TRANG CHỦ
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Category).ToList();

            ViewBag.Categories = _context.Categories.ToList();

            // 🔥 giả lập BestSeller (lấy 6 món đầu)
            ViewBag.BestSeller = products.Take(8).ToList();

            return View(products);
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}