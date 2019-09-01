using Fair.Models;
using Microsoft.EntityFrameworkCore;

namespace Fair.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Search> Searches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Username);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.IsAdmin).HasDefaultValue(false);
            modelBuilder.Entity<User>().Property(u => u.IsSysAdmin).HasDefaultValue(false);
            modelBuilder.Entity<File>().Property(f => f.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Comment>().Property(c => c.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Revision>().HasKey(r => new { r.DocumentId, r.Number });
            modelBuilder.Entity<Search>().Property(s => s.StartDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Search>().HasOne(s => s.DepartmentChair).WithMany().HasForeignKey(s => s.DepartmentChairId);
            modelBuilder.Entity<Search>().HasOne(s => s.CommitteeChair).WithMany().HasForeignKey(s => s.CommitteeChairId);
            modelBuilder.Entity<CommitteeMember>().HasKey(c => new { c.SearchId, c.UserId });
        }
    }
}
