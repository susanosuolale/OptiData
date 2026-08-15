using Microsoft.EntityFrameworkCore;
using OptiData.Domain.Entities;

namespace OptiData.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // These properties create the actual physical tables in our production database
        public DbSet<DataBundle> DataBundles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UsageRecord> UsageRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Production configuration for data tables
            modelBuilder.Entity<DataBundle>().HasKey(b => b.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<UsageRecord>().HasKey(r => r.Id);
        }
    }
}
