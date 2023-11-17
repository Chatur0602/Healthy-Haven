using Healthy_Haven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Healthy_Haven.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ForumModel> Forums { get; set; }

        public DbSet<ForumImages> ForumImages { get; set; }

        public DbSet<CourseImages> CourseImages { get; set; }

        public DbSet<CommentLikeModel> CommentLikes { get; set; }

        public DbSet<ForumLikeModel> ForumLikes { get; set; }

        public DbSet<CommentModel> Comments { get; set; }
        
        public DbSet<CoursesModel> Courses {  get; set; } 
 
    }
}
