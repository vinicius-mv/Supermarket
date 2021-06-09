using Microsoft.EntityFrameworkCore;
using Supermarket.API.Controllers;
using Supermarket.API.Models;
using Supermarket.API.ResourceModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(Context.AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CategoryProducts>> GetCategoriesWithProducts()
        {
            var categoriesProducts = _context.Categories.Select(c => new CategoryProducts
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                Products = _context.Products.Where(p =>  p.CategoryId == c.CategoryId).ToList()
            });

            return await categoriesProducts.ToListAsync();
        }
    }
}
