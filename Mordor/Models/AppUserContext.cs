using Microsoft.EntityFrameworkCore;

namespace Mordor.Models
{
    public class AppUserContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public AppUserContext(DbContextOptions<AppUserContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
