using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mordor.Models;

namespace Mordor.ViewModels
{
    public class PermissionsEdit
    {
        public AppUser User { get; set; }
        public List<Role> AllRoles {get;set; }
    }
}
