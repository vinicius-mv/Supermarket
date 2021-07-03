using Microsoft.EntityFrameworkCore;
using Supermarket.API.Models;
using Supermarket.API.Helpers.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(Context.SupermarketContext context) : base(context)
        {
        }

        public async Task<PagedList<Product>> GetProducts(PaginationParameters parameters)
        {
            var products = Get().OrderBy(p => p.ProductId);
            return await PagedList<Product>.ToPagedList(products, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<PagedList<Product>> GetProductsByPrice(PaginationParameters parameters)
        {
            var products = Get().OrderBy(p => p.Price);
            return await PagedList<Product>.ToPagedList(products, parameters.PageNumber, parameters.PageSize);
        }
    }
}
