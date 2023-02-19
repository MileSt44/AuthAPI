using AuthAPI.Model.Model;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Infrastructure.Context
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>();
        }

        public DbSet<User> Users { get; set; }
    }
}
