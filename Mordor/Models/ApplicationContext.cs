using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mordor.Models
{
    public class ApplicationContext : DbContext
    {
        public enum AllRoles
        { 
            Admin=1,
            Blocked=2    
        };

        //public Role AdminRole = new Role { Id = 1, RoleName = "Admin" };
        //public Role BlockedRole = new Role { Id = 2, RoleName = "Blocked" };

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateDefaultRoles(modelBuilder);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<AppUserRole>()
                .HasKey(t => new { t.AppUserId, t.RoleId });

            modelBuilder.Entity<AppUserRole>()
                .HasOne(pt => pt.AppUser)
                .WithMany(p => p.AppUserRole)
                .HasForeignKey(pt => pt.AppUserId);

            modelBuilder.Entity<AppUserRole>()
                .HasOne(pt => pt.Role)
                .WithMany(t => t.AppUserRole)
                .HasForeignKey(pt => pt.RoleId);
        }

        private void CreateDefaultRoles(ModelBuilder modelBuilder)
        {
            foreach(String roleName in Enum.GetNames(typeof(AllRoles)))
            {
                modelBuilder.Entity<Role>().HasData(GetRoleFromEnumByName(roleName));
            }
            
        }
        
        public Role GetRoleFromEnumByName(string roleName)
        {
            return new Role { Id = (int)Enum.Parse(typeof(AllRoles), roleName), RoleName = roleName };
        }
        
        public Role GetRoleById(int id)
        {
            return new Role { Id = id, RoleName = (string)Enum.GetName(typeof(AllRoles), id) };
        }

        public async Task<AppUser> GetUserByIdWithRolesAsync(int? id)
        {
            var User = await AppUsers
                .Include(b => b.AppUserRole)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            return User;
        }

        public async Task<List<AppUser>> GetUserListAsync()
        {
            var Users = await AppUsers
              .Include(b => b.AppUserRole)
              .ThenInclude(x => x.Role).ToListAsync();

            return Users;
        }

    }

}
