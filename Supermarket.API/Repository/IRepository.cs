using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();    // IQueryable allows async calls
        Task<T> GetBy(Expression<Func<T, bool>> predicate);

        Task<IList<T>> ListBy(Expression<Func<T, bool>> predicate = null);

        // non-persistent methods
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
