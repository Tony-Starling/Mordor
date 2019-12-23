using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mordor.Models
{
    public class ApplicationContext : DbContext
    {
        public enum AllRoles
        {
            /// <summary>The admin user</summary>
            Admin = 1,
            /// <summary>The blocked user</summary>
            Blocked = 2
        };

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Chapter> Chapters { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();

        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1"/> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.
        /// </param>
        /// <remarks>
        /// If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)"/>)
        /// then this method will not be run.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateDefaultRoles(modelBuilder);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>().Property(p => p.Id).ValueGeneratedOnAdd();
            CreateUsers_RolesRelationship(modelBuilder);
            CreatePost_ChapterRelationship(modelBuilder);
            CreatePost_Section_AppuserRelationship(modelBuilder);
        }

        private void CreatePost_Section_AppuserRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Section>()
                .HasMany(r => r.Posts)
                .WithOne(c => c.Section)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>()
                .Property(p => p.DateTime)
                .HasColumnType("datetime2");

            modelBuilder.Entity<AppUser>()
                .HasMany(x => x.Posts)
                .WithOne(x => x.AppUser)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void CreatePost_ChapterRelationship(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chapter>()
                .HasOne(e => e.Post)
                .WithMany(c => c.PostChapters)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
        private void CreateUsers_RolesRelationship(ModelBuilder modelBuilder)
        {
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

        /// <summary>Gets the name of the role from enum by.</summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public Role GetRoleFromEnumByName(string roleName)
        {
            return new Role { Id = (int)Enum.Parse(typeof(AllRoles), roleName), RoleName = roleName };
        }
        
        public Role GetRoleById(int id)
        {
            return new Role { Id = id, RoleName = (string)Enum.GetName(typeof(AllRoles), id) };
        }

        /// <summary>Gets the user by identifier with roles asynchronous.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<AppUser> GetUserByIdWithRolesAsync(int? id)
        {
            var User = await AppUsers
                .Include(b => b.AppUserRole)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            return User;
        }
        
        /// <summary>Gets the user by identifier asynchronous.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<AppUser> GetUserByIdAsync(int? id)
        {
            var User = await AppUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            return User;
        }

        /// <summary>Gets the user list asynchronous.</summary>
        /// <returns></returns>
        public async Task<List<AppUser>> GetUserListAsync()
        {
            var Users = await AppUsers
              .Include(b => b.AppUserRole)
              .ThenInclude(x => x.Role).ToListAsync();
            return Users;
        }

        /// <summary>Gets the sections list asynchronous.</summary>
        /// <returns></returns>
        public async Task<List<Section>> GetSectionsListAsync()
        {
            return await Sections.ToListAsync();
        }

        /// <summary>Gets the user with posts asynchronous.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> GetUserWithPostsAsync(int? id)
        {
            var User = await AppUsers
               .Include(b => b.Posts)
               .FirstOrDefaultAsync(m => m.Id == id);
            return User.Posts;
        }

        /// <summary>Gets the name of the user by user.</summary>
        /// <param name="UserName">Name of the user.</param>
        /// <returns></returns>
        public AppUser GetUserByUserName(string UserName)
        {
            var User = AppUsers
                .FirstOrDefault(m => m.UserName == UserName);
            return User;
        }

        /// <summary>Gets the post by identifier asynchronous.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Post> GetPostByIdAsync(int? id)
        {
            var Post = await Posts
               .FirstOrDefaultAsync(m => m.Id == id);
            return Post;
        }

        /// <summary>Gets the section by identifier asynchronous.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<Section> GetSectionByIdAsync(int? id)
        {
            var Section = await Sections
               .FirstOrDefaultAsync(m => m.Id == id);
            return Section;
        }

        /// <summary>Gets the post with chapters.</summary>
        /// <param name="PostId">The post identifier.</param>
        /// <returns></returns>
        public async Task<Post> GetPostWithChapters(int? PostId)
        {
            var Post = await Posts
                .Include(c => c.PostChapters)
                .FirstOrDefaultAsync(x => x.Id == PostId);
                return Post;
        }

        /// <summary>Gets the published posts.</summary>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> GetPublishedPosts()
        {
            var PostList = Posts
                .Include(c => c.AppUser);

            return PostList;
        }
        /// <summary>Searches the posts.</summary>
        /// <param name="SearchText">The search text.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Post>> SearchPosts(string SearchText)
        {
            List<Post> ListPosts = SearchInPosts(SearchText);
            List<Post> ListPostsFromChapters = SearchInChapters(SearchText);
            List<Post> ulist = ListPosts.Union(ListPostsFromChapters).ToList();
            return (ulist);
        }

        /// <summary>Searches the in posts.</summary>
        /// <param name="SearchText">The search text.</param>
        /// <returns></returns>
        private List<Post> SearchInPosts(string SearchText)
        {
             List<Post> PostList = Posts.Include(c => c.AppUser).Include(c => c.PostChapters)
               .Where(x
               => EF.Functions.FreeText(x.Description, SearchText)
               || EF.Functions.FreeText(x.Title, SearchText)
               || EF.Functions.FreeText(x.Tags, SearchText)
               ).ToList();
            return PostList;
        }
        /// <summary>Searches the in chapters.</summary>
        /// <param name="SearchText">The search text.</param>
        /// <returns></returns>
        private List<Post> SearchInChapters(string SearchText)
        {
            List<Post> Answerlist = new List<Post>();
            List<Chapter> ChapterList = Chapters.Include(c => c.Post).Include(c => c.Post.AppUser)
               .Where(x
               => EF.Functions.FreeText(x.Head, SearchText)
               || EF.Functions.FreeText(x.Text, SearchText)
               ).ToList();
            foreach (Chapter element in ChapterList)
            {
                Answerlist.Add(element.Post);
            }
            return Answerlist;
        }


    }

}
