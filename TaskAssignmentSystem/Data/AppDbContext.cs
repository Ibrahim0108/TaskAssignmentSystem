using Microsoft.EntityFrameworkCore;
using TaskAssignmentSystem.Models.Users;

namespace TaskAssignmentSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
