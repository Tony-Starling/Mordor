using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mordor.Models;
using Mordor.ViewModels;

namespace Mordor.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationContext _context;

        public AdminController(ApplicationContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            AdminPanelIndex var = new AdminPanelIndex
            {
                RegistredUserCount = _context.AppUsers.Count()
            };
            //var.PostsCount = _context.Posts.count();
            return View(var);
        }

        [HttpGet]
        public IActionResult UserList()
        {
            return View(_context.GetUserListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }
        public async Task<IActionResult> PermissionsEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PermissionsEdit var = new PermissionsEdit
            {
                User = await _context.GetUserByIdWithRolesAsync(id),
                AllRoles = _context.Roles.ToList()
            };
            if (var.User == null)
            {
                return NotFound();
            }
            return View(var);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,Email,EmailConfirmed,Password")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }
            return View(appUser);
        }

        public async Task<IActionResult> PermissionsSetRoles(int id, List<int> RoleAcceptId)
        {
            var User = await _context.GetUserByIdWithRolesAsync(id);
            if (User.AppUserRole != null)
            {
                User.AppUserRole.Clear();
            }
            foreach (int AcceptId in RoleAcceptId)
            {
                var Role = _context.GetRoleById(AcceptId);
                User.AppUserRole.Add(new AppUserRole { RoleId = Role.Id, AppUserId = id });
            }
            _context.AppUsers.Update(User);
            await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Admin", action = "UserList" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserName,Email,EmailConfirmed,Password")] AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(appUser);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appUser = await _context.AppUsers.FindAsync(id);
            _context.AppUsers.Remove(appUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(int id)
        {
            return _context.AppUsers.Any(e => e.Id == id);
        }
    }
}
