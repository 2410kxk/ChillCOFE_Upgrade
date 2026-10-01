using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Staff")]
public class StaffController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}