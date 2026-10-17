using System;
using Microsoft.EntityFrameworkCore;
using PaintStore.Models.Methods;
using PaintStore.Models.Enums;

namespace PaintStore.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<PaintProduct> PaintProducts { get; set; }

        public DbSet<User> Users {get; set;}

        public DbSet<Order> Orders {get; set;}

        public DbSet<PaintBrand> PaintBrands {get; set;}

        public DbSet<PaintSeries> PaintSeries {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaintProduct>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        

            modelBuilder.Entity<User>().Ignore(u=>u.OrderHistory);
            modelBuilder.Entity<User>().Ignore(u => u.PaymentHistory);
            modelBuilder.Entity<User>().Ignore(u=> u.PaymentList);


            modelBuilder.Entity<Order>().Property(o => o.TotalPrice).HasPrecision(18,2);

            // brand -> many series
            modelBuilder.Entity<PaintSeries>()
                .HasOne(s => s.Brand)
                .WithMany(b=>b.PaintSeries)
                .HasForeignKey(s => s.BrandId);

            // series -> many products
            modelBuilder.Entity<PaintProduct>()
                .HasOne(p=>p.Series)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SeriesId); 

            // user -> many orders
            modelBuilder.Entity<Order>()
                .HasOne<User>()
                .WithMany(u=> u.OrderList)
                .HasForeignKey(o =>o.UserId);
            
            // extra constraints
            modelBuilder.Entity<PaintBrand>().Property(b => b.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<PaintSeries>().Property(s => s.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().HasIndex(u => u.UserEmail).IsUnique();
            modelBuilder.Entity<PaintProduct>().OwnsOne(p => p.Specification);


        
        
        }
    }
}
