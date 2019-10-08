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
        public DbSet<Department> Departments { get; set; }
        public DbSet<Search> Searches { get; set; }
        public DbSet<ApplicationTemplate> ApplicationTemplates { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Username);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.IsAdmin).HasDefaultValue(false);
            modelBuilder.Entity<User>().Property(u => u.IsSysAdmin).HasDefaultValue(false);
            modelBuilder.Entity<File>().Property(f => f.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Revision>().HasAlternateKey(r => new { r.DocumentId, r.Number });
            modelBuilder.Entity<Revision>().Property(r => r.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Search>().Property(s => s.StartDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Search>().HasOne(s => s.DepartmentChair).WithMany().HasForeignKey(s => s.DepartmentChairId);
            modelBuilder.Entity<Search>().HasOne(s => s.CommitteeChair).WithMany().HasForeignKey(s => s.CommitteeChairId);
            modelBuilder.Entity<CommitteeMember>().HasKey(c => new { c.SearchId, c.UserId });
            modelBuilder.Entity<ApplicationTemplate>().Property(t => t.NumberOfReferences).HasDefaultValue(3);
            modelBuilder.Entity<ApplicationTemplate>().Property(t => t.IsObsolete).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationTemplate>().HasQueryFilter(t => !t.IsObsolete);
            modelBuilder.Entity<ApplicationTemplateDegree>().HasKey(d => new { d.ApplicationTemplateId, d.Index });
            modelBuilder.Entity<ApplicationTemplateDocument>().HasKey(d => new { d.ApplicationTemplateId, d.Index });
            modelBuilder.Entity<Application>().Property(a => a.IsWithdrawn).HasDefaultValue(false);
            modelBuilder.Entity<Application>().Property(a => a.DateCreated).HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<ApplicationDegree>().HasKey(d => new { d.ApplicationId, d.Index });
            modelBuilder.Entity<ApplicationDegree>().Property(d => d.IsExpected).HasDefaultValue(false);
            modelBuilder.Entity<ApplicationDocument>().HasKey(d => new { d.ApplicationId, d.Index });
            modelBuilder.Entity<ApplicationReference>().HasKey(r => new { r.ApplicationId, r.Index });
            modelBuilder.Entity<Evaluation>().HasIndex(e => new { e.ApplicationId, e.EvaluatorId }).IsUnique();
        }
    }
}
