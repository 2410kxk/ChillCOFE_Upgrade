using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLiQuanCaffee.Controllers;
public class InfoController : Controller
{
    public IActionResult About()
    {
        return View();
    }

    public IActionResult Store()
    {
        return View();
    }

    public IActionResult News()
    {
        return View();
    }
}