using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLiQuanCaffee.Controllers;
public class InfoController : Controller
{
    public IActionResult About()
    {
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Store()
    {
        return RedirectToAction("Index", "Home");
    }

    public IActionResult News()
    {
        return View();
    }
}