using CargoTrack.DTO.DTOs.UserDtos;
using CargoTrack.Entity.Entities;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CargoTrack.WebUI.Controllers
{
    public class LoginController(SignInManager<AppUser> _signInManager,
                                 UserManager<AppUser> _userManager) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginUserDto loginUserDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUserDto.UserName, loginUserDto.Password, false, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
                return View(loginUserDto);
            }

            var user = await _userManager.FindByNameAsync(loginUserDto.UserName);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(Roles.Admin))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else if (roles.Contains(Roles.Manager))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Manager" });
                }
                else if (roles.Contains(Roles.User))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "User" });
                }
            }

            return RedirectToAction("Index", "Default");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Default");
        }
    }
}
