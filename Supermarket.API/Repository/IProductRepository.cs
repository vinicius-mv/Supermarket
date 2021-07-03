using Supermarket.API.Models;
using Supermarket.API.Helpers.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<PagedList<Product>> GetProducts(PaginationParameters productsParameters);
        Task<IEnumerable<Product>> GetProductsByPrice();
    }
}
