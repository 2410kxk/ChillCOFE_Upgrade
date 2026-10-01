using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLiQuanCaffee.Models;

public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(UserManager<IdentityUser> userManager,
                             SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // VIEW REGISTER
    public IActionResult Register()
    {
        return View();
    }

    // POST REGISTER
    [HttpPost]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = username,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            // GÁN ROLE CUSTOMER
            await _userManager.AddToRoleAsync(user, "User");

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }


        ViewBag.Error = "Đăng ký thất bại";
        return View();
    }
}