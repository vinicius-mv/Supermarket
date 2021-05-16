using Microsoft.EntityFrameworkCore;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // relation (1 to many) Product - Category
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany();

        }

    }
}
