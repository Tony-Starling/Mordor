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


namespace Mordor
{
    public class DbWorker
    {
        private readonly ApplicationContext _context;

        public DbWorker(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<List<AppUser>> GetAllUsersAsync()
        {
            var Users =  _context.AppUsers
                .Include(b => b.AppUserRole)
                .ThenInclude(x => x.Role).ToListAsync();

            return await Users;
        }


        public object GetUserById(int? id)
        {
            var appUser =  _context.AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            return appUser;
        }

        public object GetUserByIdWithPermissons(int? id)
        {
            var User =  _context.AppUsers
                .Include(b => b.AppUserRole)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            return User;
        }

        public object GetAllRoles()
        {
            var AllRoles = _context.Roles.ToList();
            return AllRoles;
        }
    }
}
