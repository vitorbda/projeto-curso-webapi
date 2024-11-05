using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APICatalogo.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        public Repository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public virtual async Task<IEnumerable<T>> GetAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public virtual T Create(T entity)
        {
            _context.Set<T>().Add(entity);

            return entity;
        }

        public virtual IEnumerable<T> Create(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);

            return entities;
        }

        public virtual T Update(T entity)
        {
            _context.Set<T>().Update(entity);

            return entity;
        }

        public virtual T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);

            return entity;
        }
    }
}
