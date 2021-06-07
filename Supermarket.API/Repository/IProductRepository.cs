using Supermarket.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsOrderedByPrice();

        Task<IEnumerable<Product>> GetProductsByCategory(int categoryId);
    }
}
