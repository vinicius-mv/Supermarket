using Supermarket.API.Models;
using Supermarket.API.ResourceModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<CategoryProduct>> GetCategoriesWithProducts();
    }
}
