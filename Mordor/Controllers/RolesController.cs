﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly ApplicationContext _context;
        public RolesController(ApplicationContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            var AppUsers = _context.AppUsers
                .Include(b => b.AppUserRole)
                .ThenInclude(x => x.Role);

            return View(AppUsers.ToList());
        }

        public ActionResult Details(int id = 0)
        {
            AppUser student = _context.AppUsers.Find(id);
            if (student == null)
            {
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(student);
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            base.Dispose(disposing);
        }
       

      }
}