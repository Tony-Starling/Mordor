using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mordor.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Mordor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private ApplicationContext db;
        public RolesController(ApplicationContext context)
        {
            db = context;
        }
        public ActionResult Index()
        {
            var AppUsers = db.AppUsers
                .Include(b => b.AppUserRole)
                .ThenInclude(x => x.Role);

            return View(AppUsers.ToList());
        }

        public ActionResult Details(int id = 0)
        {
            AppUser student = db.AppUsers.Find(id);
            if (student == null)
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(student);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
       

      }
}