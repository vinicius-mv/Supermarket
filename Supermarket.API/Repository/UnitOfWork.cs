using Supermarket.API.Context;
using System;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository ProductRepository => _productRepository ?? new ProductRepository(_context);
        public ICategoryRepository CategoryRepository => _categoryRepository ?? new CategoryRepository(_context);

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
