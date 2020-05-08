using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BookCatalogue.Contracts.Data.Repositories
{
    public interface IRepository<T>
    {
        public void Add(T entity);
        public void Remove(T entity);
        public void Update(T entity);
        public Task<T> GetAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        public Task SaveAsync();
    }
}
