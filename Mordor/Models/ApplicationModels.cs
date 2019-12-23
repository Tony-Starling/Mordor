using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public virtual ICollection<AppUserRole> AppUserRole { get; set; }
        public virtual ICollection<Post> Posts { get; set; }

    }
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<AppUserRole> AppUserRole { get; set; }
    }

    public class AppUserRole
    {
        public int AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class Section
    {
        public int Id { get; set; }
        public string SectionName { get; set; }
        public virtual ICollection<Post>  Posts { get; set; }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public int AuthorId { get; set; }
        public virtual AppUser AppUser { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateTime { get; set; }
        public int SectionId { get; set; }
        public virtual Section Section { get; set; }
        public virtual ICollection<Chapter> PostChapters { get; set; }
    }

    public class Chapter
    {
        public int Id { get; set; }
        public string Head { get; set; }
        public string ImageUrl { get; set; }
        public string Text { get; set; }
        public Post Post { get; set; }
    }

}
