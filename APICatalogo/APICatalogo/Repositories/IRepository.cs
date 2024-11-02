using System.Linq.Expressions;

namespace APICatalogo.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> Get();
        T? Get(Expression<Func<T, bool>> predicate);
        T Create(T entity);
        IEnumerable<T> Create(IEnumerable<T> entities);
        T Update(T entity);
        T Delete (T entity);
    }
}
