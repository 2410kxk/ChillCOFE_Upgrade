using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace QuanLiQuanCaffee.Controllers;
[Authorize(Roles="Admin")]
[ApiExplorerSettings(IgnoreApi=true)]
public class ApiDashboardController : Controller
{
    [HttpGet("/ApiDashboard")]
    public IActionResult Index() => View();
}
