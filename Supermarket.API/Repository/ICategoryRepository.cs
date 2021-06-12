using Supermarket.API.Models;
using Supermarket.API.Pagination;
using Supermarket.API.ResourceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<PagedList<Category>> GetCategories(PaginationParameters parameter);
        Task<PagedList<CategoryProducts>> GetCategoriesWithProducts(PaginationParameters parameters);
    }
}
