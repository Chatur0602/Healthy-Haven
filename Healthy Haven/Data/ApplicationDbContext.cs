using Healthy_Haven.Models;
using Microsoft.EntityFrameworkCore;

namespace Healthy_Haven.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<UserEntity> Users { get; set; }
    }
}
