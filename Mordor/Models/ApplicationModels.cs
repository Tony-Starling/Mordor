using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mordor.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }
        public string Password { get; set; }
        public virtual List<AppUserRole> AppUserRole { get; set; }

    }
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        public virtual List<AppUserRole> AppUserRole { get; set; }
    }

    public class AppUserRole
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }

    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }

    }
}
