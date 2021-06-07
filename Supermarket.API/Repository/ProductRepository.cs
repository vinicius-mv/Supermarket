using Microsoft.EntityFrameworkCore;
using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(Context.AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(int categoryId)
        {
            return await ListByFilter(p => p.CategoryId == categoryId);
        }

        public async Task<IEnumerable<Product>> GetProductsOrderedByPrice()
        {
            return await Get().OrderBy(p => p.Price).ToListAsync();
        }
    }
}
