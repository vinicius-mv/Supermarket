using Microsoft.EntityFrameworkCore;
using Supermarket.API.Controllers;
using Supermarket.API.Dtos;
using Supermarket.API.Models;
using Supermarket.API.Pagination;
using Supermarket.API.ResourceModels;
using System;
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

        public Task<PagedList<Category>> GetCategories(PaginationParameters parameters)
        {
            var categories = Get().OrderBy(c => c.Name);
            return PagedList<Category>.ToPagedList(categories, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PagedList<CategoryProducts>> GetCategoriesWithProducts(PaginationParameters parameters)
        {
            // var categories = await _context.Categories.ToListAsync();
            var categories = _context.Categories.AsQueryable();
            var categoriesIds = categories.Select(c => c.CategoryId).AsQueryable();

            var products = _context.Products.Where(p => categoriesIds.Contains(p.CategoryId)).AsQueryable();

            var categoriesProducts = categories.Select(c => new CategoryProducts
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                ImageUrl = c.ImageUrl,
                Products = products.Where(p => p.CategoryId == c.CategoryId).ToList()
            }).AsQueryable();

            return await PagedList<CategoryProducts>.ToPagedList(categoriesProducts, parameters.PageNumber, parameters.PageSize);
        }
    }
}
