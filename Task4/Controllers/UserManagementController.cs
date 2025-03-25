using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task4.Data;
using Task4.Models;

namespace Task4.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public UserManagementController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) 
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager; 
        }

        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) ||
                    u.Email.Contains(search)
                );
            }

            var users = await query
                .OrderByDescending(u => u.LastLoginTime)
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> BlockUsers(List<string> userIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool currentUserBlocked = false;

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = true;
                    await _userManager.UpdateAsync(user);

                    if (user.Id == currentUser?.Id)
                    {
                        currentUserBlocked = true;
                    }
                }
            }

            if (currentUserBlocked)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnblockUsers(List<string> userIds)
        {
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.IsBlocked = false;
                    await _userManager.UpdateAsync(user);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUsers(List<string> userIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool currentUserDeleted = false;

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);

                    if (user.Id == currentUser?.Id)
                    {
                        currentUserDeleted = true;
                    }
                }
            }

            if (currentUserDeleted)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index");
        }
    }
}