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
        Task<T> GetByFilter(Expression<Func<T, bool>> predicate);

        Task<IList<T>> ListByFilter(Expression<Func<T, bool>> predicate);

        // non-persistent methods
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
