using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Username);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.IsAdmin).HasDefaultValue(false);
            modelBuilder.Entity<File>().Property(f => f.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
