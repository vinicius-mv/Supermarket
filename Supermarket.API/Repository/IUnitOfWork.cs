using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface IUnitOfWork
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        Task CommitAsync();
    }
}
