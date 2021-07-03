using Microsoft.EntityFrameworkCore;
using Supermarket.API.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Supermarket.API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected SupermarketContext _context;

        public Repository(SupermarketContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<T> GetBy(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                predicate = (x) => true;

            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public IQueryable<T> ListBy(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                predicate = (x) => true;

            return _context.Set<T>().AsNoTracking().Where(predicate);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
