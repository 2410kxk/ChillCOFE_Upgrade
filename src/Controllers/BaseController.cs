using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLiQuanCaffee.Models;

public class BaseController : Controller
{
    protected readonly AppDbContext _context;

    public BaseController(AppDbContext context)
    {
        _context = context;
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        ViewBag.Categories = _context.Categories
           .Include(c => c.Products)
           .ToList();
        base.OnActionExecuting(context);

    }
}