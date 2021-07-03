using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        IQueryable<T> Get();
        Task<T> GetBy(Expression<Func<T, bool>> predicate = null);
        IQueryable<T> ListBy(Expression<Func<T, bool>> predicate = null);
        void Update(T entity);
    }
}