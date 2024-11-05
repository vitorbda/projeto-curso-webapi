using System.Linq.Expressions;

namespace APICatalogo.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        T Create(T entity);
        IEnumerable<T> Create(IEnumerable<T> entities);
        T Update(T entity);
        T Delete (T entity);
    }
}
