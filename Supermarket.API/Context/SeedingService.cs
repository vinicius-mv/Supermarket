using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Context
{
    public class SeedingService
    {
        private AppDbContext _context;

        public SeedingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Seed()
        {
            await SeedCategories();
            await SeedProducts();
        }

        private async Task SeedCategories()
        {
            if (_context.Categories.Any())
                return;

            var c1 = new Category() { Name = "Beverages", ImageUrl = "images/beverages.jpg" };
            var c2 = new Category() { Name = "Food takeout", ImageUrl = "images/food-takeout.jpg" };
            var c3 = new Category() { Name = "Groceries", ImageUrl = "images/groceries.jpg" };

            _context.Categories.AddRange(c1, c2, c3);
            await _context.SaveChangesAsync();
        }

        private async Task SeedProducts()
        {
            if (_context.Products.Any())
                return;

            var p1 = new Product() { Name = "Diet-Coke", Description = "Diet-Coke 350 ml", Price = 1.3m, CategoryId = 1 };
            var p2 = new Product() { Name = "Sandwich", Description = "Sandwich 300 g", Price = 4.50m, CategoryId = 2 };
            var p3 = new Product() { Name = "Pudding", Description = "Pudding 400 g", Price = 6.0m, CategoryId = 3 };

            _context.Products.AddRange(p1, p2, p3);
            await _context.SaveChangesAsync();
        }
    }
}
