using System;
using Microsoft.EntityFrameworkCore;
using PaintStore.Models.Methods;

namespace PaintStore.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaintProduct> PaintProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaintProduct>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PaintProduct>()
                .OwnsOne(p => p.Specification);
        }
    }
}