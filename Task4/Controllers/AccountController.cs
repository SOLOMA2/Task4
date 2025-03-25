using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Task4.Models;
using Task4.ViewsModels;

namespace Task4.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        private readonly UserManager<AppUser> userManager;
        public AccountController(SignInManager<AppUser> _signInManager, UserManager<AppUser> userManager)
        {
            this._signInManager = _signInManager;
            this.userManager = userManager;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);

                if (user == null)
                {
                    ModelState.AddModelError("", "User not found");
                    return View(model);
                }
                if (user.IsBlocked)
                {
                    ModelState.AddModelError("", "Account is blocked");
                    return View(model);
                }
                var passwordValid = await userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                {
                    ModelState.AddModelError("", "Invalid password");
                    return View(model);
                }
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    user.LastLoginTime = DateTime.UtcNow;
                    await userManager.UpdateAsync(user);
                    return RedirectToAction("Index", "UserManagement");
                }

                ModelState.AddModelError("", "Unexpected error");
            }
            return View(model);
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account"); 
        }
    }
}
