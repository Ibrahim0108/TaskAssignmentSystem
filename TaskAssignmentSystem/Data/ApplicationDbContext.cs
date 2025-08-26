using Microsoft.EntityFrameworkCore;
using TaskAssignmentSystem.Models.Teams;
using TaskAssignmentSystem.Models.Users;
using TaskAssignmentSystem.Models.Workspaces;

namespace TaskAssignmentSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Workspace> Workspaces => Set<Workspace>();

        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
        public DbSet<TeamProgressUpdate> TeamProgressUpdates => Set<TeamProgressUpdate>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Username).IsUnique();
                b.Property(u => u.Username).HasMaxLength(64).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.Property(u => u.FullName).HasMaxLength(128).IsRequired();
                b.Property(u => u.Role).IsRequired();
                b.Property(u => u.IsApproved).HasDefaultValue(false);
                b.Property(u => u.Department).IsRequired();
            });
        }
    }
}