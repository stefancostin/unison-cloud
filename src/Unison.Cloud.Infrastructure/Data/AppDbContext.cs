using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Cloud.Core.Data.Entities;

namespace Unison.Cloud.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options) 
        {
            _config = config;
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<SyncEntity> SyncEntities { get; set; }
        public DbSet<SyncLog> SyncLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("Unison"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.NodeId)
                .HasDefaultValue(1);
            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.Version)
                .HasDefaultValue(1);
            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.Schema)
                .HasJsonValueConversion();

            modelBuilder.Entity<SyncLog>()
                .Property(p => p.AddedRecords)
                .HasDefaultValue(0);
            modelBuilder.Entity<SyncLog>()
                .Property(p => p.UpdatedRecords)
                .HasDefaultValue(0);
            modelBuilder.Entity<SyncLog>()
               .Property(p => p.DeletedRecords)
               .HasDefaultValue(0);
            modelBuilder.Entity<SyncLog>()
             .Property(p => p.CreatedAt)
             .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<SyncLog>()
             .Property(p => p.UpdatedAt)
             .HasDefaultValueSql("GETDATE()");
        }
    }
}
