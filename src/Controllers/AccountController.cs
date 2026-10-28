using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuanLiQuanCaffee.Models;

[AutoValidateAntiforgeryToken]
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
            var assigned = await _userManager.AddToRoleAsync(user, "User");
            if (!assigned.Succeeded) { await _userManager.DeleteAsync(user); ViewBag.Error="Chưa thể tạo tài khoản. Vui lòng thử lại."; return View(); }

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }


        ViewBag.Error = "Đăng ký thất bại";
        return View();
    }
}