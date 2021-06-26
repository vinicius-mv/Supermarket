using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Models;

namespace Supermarket.API.Context
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(ce =>
            {
                ce.HasKey(c => c.CategoryId);
                ce.ToTable("categories");
            });

            // relation (1 to many) Product - Category
            modelBuilder.Entity<Product>(pe =>
            {
                pe.HasKey(pe => pe.ProductId);
                pe.Property(p => p.CategoryId).IsRequired();

                pe.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(p => p.CategoryId);
                pe.ToTable("products");
            });
        }

    }
}
