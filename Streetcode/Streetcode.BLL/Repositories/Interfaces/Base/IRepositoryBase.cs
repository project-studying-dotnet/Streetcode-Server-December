using Streetcode.BLL.Specifications;
using System.Linq.Expressions;

namespace Streetcode.BLL.Repositories.Interfaces.Base
{
    public interface IRepositoryBase<T>
        where T : class
    {
        IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = default);

        T Create(T entity);

        Task<T> CreateAsync(T entity);

        Task CreateRangeAsync(IEnumerable<T> items);

        T Update(T entity);

        public void UpdateRange(IEnumerable<T> items);

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> items);

        void Attach(T entity);

        void Detach(T entity);

        T Entry(T entity);

        public Task ExecuteSqlRaw(string query);

        IQueryable<T> Include(params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = default,
            List<string>? include = default);

        Task<IEnumerable<T>?> GetAllAsync(
            Expression<Func<T, T>> selector,
            Expression<Func<T, bool>>? predicate = default,
            List<string>? include = default);

        Task<T?> GetSingleOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = default,
            List<string>? include = default);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = default,
            List<string>? include = default);

        Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, T>> selector,
            Expression<Func<T, bool>>? predicate = default,
            List<string>? include = default);

        Task<IEnumerable<T>> GetAllBySpecAsync(IBaseSpecification<T>? specification = null);

        Task<T?> GetFirstOrDefaultBySpecAsync(IBaseSpecification<T>? specification = null);
    }
}
