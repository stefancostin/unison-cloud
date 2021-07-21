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

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SyncAgent> SyncAgent { get; set; }
        public DbSet<SyncEntity> SyncEntities { get; set; }
        public DbSet<SyncLog> SyncLog { get; set; }
        public DbSet<SyncNode> SyncNodes { get; set; }

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

            modelBuilder.Entity<SyncAgent>()
                .Property(p => p.NodeId)
                .HasDefaultValue(1);
            modelBuilder.Entity<SyncAgent>()
                .HasIndex(p => p.InstanceId)
                .IsUnique();
            modelBuilder.Entity<SyncAgent>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<SyncAgent>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<SyncAgent>()
                .HasOne(a => a.Node)
                .WithMany(n => n.Agents)
                .HasForeignKey(a => a.NodeId);

            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.NodeId)
                .HasDefaultValue(1);
            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.Version)
                .HasDefaultValue(1);
            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.Fields)
                .HasJsonValueConversion();
            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<SyncEntity>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

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

            modelBuilder.Entity<SyncNode>()
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<SyncNode>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<SyncNode>()
                .HasMany(n => n.Agents)
                .WithOne(a => a.Node)
                .HasForeignKey(n => n.NodeId);
            modelBuilder.Entity<SyncNode>()
                .Navigation(b => b.Agents)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
