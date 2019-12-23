using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mordor.Models;

namespace Mordor.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationContext db;
        public AccountController(ApplicationContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await db.AppUsers.FirstOrDefaultAsync(u => u.UserName == model.UserName && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(model.UserName); 

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await db.AppUsers.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    db.AppUsers.Add(new AppUser { UserName=model.UserName, Email = model.Email, Password = model.Password });
                    await db.SaveChangesAsync();
                    await Authenticate(model.UserName);
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(string UserName)
        {
            ClaimsIdentity id = new ClaimsIdentity(GetUserClaims(UserName), "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        public ICollection<AppUserRole> GetUserRolesByUserName(string UserName)
        {
            var AppUsers = db.AppUsers.Where(x => x.UserName == UserName)
               .Include(b => b.AppUserRole)
               .ThenInclude(x => x.Role);
            
            return AppUsers.ToList()[0].AppUserRole;
        }

        public List<Claim> GetUserClaims(String UserName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, UserName)
            };
            foreach (AppUserRole Role in GetUserRolesByUserName(UserName))
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.Role.RoleName));
            }
            return claims;
        }





    }

}