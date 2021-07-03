using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Supermarket.API.Context;
using Supermarket.API.Models;
using Supermarket.API.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermarket.API.Tests
{
    public class ContextInitializer
    {
        public IUnitOfWork UoW { get; }

        public ContextInitializer()
        {
            UoW = CreateUnitOfWork();
            Seed();
        }

        private UnitOfWork CreateUnitOfWork()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<SupermarketContext>()
                .UseInMemoryDatabase(databaseName: "Tests_With_In_Memory_Database")
                .Options;

            var context = new SupermarketContext(inMemoryDataContextOptions);

            return new UnitOfWork(context);
        }

        private void Seed()
        {
            var c1 = new Category() { Name = "Beverages", ImageUrl = "images/beverages.jpg" };
            var c2 = new Category() { Name = "Food takeout", ImageUrl = "images/food-takeout.jpg" };
            var c3 = new Category() { Name = "Groceries", ImageUrl = "images/groceries.jpg" };

            UoW.CategoryRepository.Add(c1);
            UoW.CategoryRepository.Add(c2);
            UoW.CategoryRepository.Add(c3);

            var p1 = new Product() { Name = "Diet-Coke", Description = "Diet-Coke 350 ml", Price = 1.3m, CategoryId = 1 };
            var p2 = new Product() { Name = "Sandwich", Description = "Sandwich 300 g", Price = 4.50m, CategoryId = 2 };
            var p3 = new Product() { Name = "Pudding", Description = "Pudding 400 g", Price = 6.0m, CategoryId = 3 };
            var p4 = new Product() { Name = "Orange Juice", Description = "Orange Juice 350 ml", Price = 1.5m, CategoryId = 1 };
          

            UoW.ProductRepository.Add(p1);
            UoW.ProductRepository.Add(p2);
            UoW.ProductRepository.Add(p3);

            UoW.CommitAsync();
        }
    }
}

