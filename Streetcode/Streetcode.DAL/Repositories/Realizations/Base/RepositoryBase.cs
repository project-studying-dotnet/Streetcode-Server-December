using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using MimeKit;
using Streetcode.DAL.Caching.RedisCache;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Specification;
using Streetcode.DAL.Specification.Evaluator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Streetcode.DAL.Repositories.Realizations.Base
{
    public class RepositoryBase<T> : IRepositoryBase<T>
        where T : class
    {
        protected readonly DbSet<T> _dbSet;
        private readonly StreetcodeDbContext _dbContext;
        private readonly IRedisCacheService _redisCacheService;

        protected RepositoryBase(StreetcodeDbContext context, IRedisCacheService redisCacheService = null!)
        {
            _dbContext = context;
            _dbSet = _dbContext.Set<T>();
            _redisCacheService = redisCacheService;
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>>? predicate = default)
        {
            return GetQueryable(predicate).AsNoTracking();
        }

        public T Create(T entity)
        {
            return _dbContext.Set<T>().Add(entity).Entity;
        }

        public async Task<T> CreateAsync(T entity)
        {
            var tmp = await _dbContext.Set<T>().AddAsync(entity);
            return tmp.Entity;
        }

        public Task CreateRangeAsync(IEnumerable<T> items)
        {
            return _dbContext.Set<T>().AddRangeAsync(items);
        }

        public EntityEntry<T> Update(T entity)
        {
            return _dbContext.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> items)
        {
            _dbContext.Set<T>().UpdateRange(items);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            _dbContext.Set<T>().RemoveRange(items);
        }

        public void Attach(T entity)
        {
            _dbContext.Set<T>().Attach(entity);
        }

        public EntityEntry<T> Entry(T entity)
        {
            return _dbContext.Entry(entity);
        }

        public void Detach(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        public Task ExecuteSqlRaw(string query)
        {
            return _dbContext.Database.ExecuteSqlRawAsync(query);
        }

        public IQueryable<T> Include(params Expression<Func<T, object>>[] includes)
        {
            IIncludableQueryable<T, object>? query = default;

            if (includes.Any())
            {
                query = _dbContext.Set<T>().Include(includes[0]);
            }

            for (int queryIndex = 1; queryIndex < includes.Length; ++queryIndex)
            {
                query = query!.Include(includes[queryIndex]);
            }

            return (query is null) ? _dbContext.Set<T>() : query.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include).ToListAsync();
        }

        public async Task<IEnumerable<T>?> GetAllAsync(
            Expression<Func<T, T>> selector,
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include, selector).ToListAsync() ?? new List<T>();
        }

        public async Task<T?> GetSingleOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include).SingleOrDefaultAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include).FirstOrDefaultAsync();
        }

        public async Task<T?> GetFirstOrDefaultAsync(
            Expression<Func<T, T>> selector,
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default)
        {
            return await GetQueryable(predicate, include, selector).FirstOrDefaultAsync();
        }

        // Specification Pattern Methods
        public async Task<IEnumerable<T>> GetAllBySpecAsync(IBaseSpecification<T>? specification = null)
        {
            if (specification != null && specification.CacheKey != string.Empty)
            {
                var dataFromCache = await _redisCacheService.GetCachedDataAsync<IEnumerable<T>>(specification.CacheKey);
                if (dataFromCache != null)
                {
                    return dataFromCache;
                }

                var dataFromDb = ApplySpecificationForList(specification);

                if (!dataFromDb.Any()) 
                {
                    return dataFromDb!;
                }

                await _redisCacheService.SetCachedDataAsync(specification.CacheKey, dataFromDb, specification.CacheMinutes);

                return dataFromDb;
            }
            else
            {
                return ApplySpecificationForList(specification);
            }
        }

        public async Task<T?> GetFirstOrDefaultBySpecAsync(IBaseSpecification<T>? specification = null)
        {
            if (specification != null && specification.CacheKey != string.Empty)
            {
                var dataFromCache = await _redisCacheService.GetCachedDataAsync<T>(specification.CacheKey);
                if (dataFromCache != null)
                {
                    return dataFromCache;
                }

                var dataFromDb = await ApplySpecificationForList(specification).FirstOrDefaultAsync();

                if (dataFromDb == null)
                {
                    return dataFromDb!;
                }

                await _redisCacheService.SetCachedDataAsync(specification.CacheKey, dataFromDb, specification.CacheMinutes);

                return dataFromDb;
            }
            else
            {
                return await ApplySpecificationForList(specification).FirstOrDefaultAsync();
            }
        }

        private IQueryable<T> ApplySpecificationForList(IBaseSpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), specification);
        }

        // End of Specification Pattern Methods

        private IQueryable<T> GetQueryable(
            Expression<Func<T, bool>>? predicate = default,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = default,
            Expression<Func<T, T>>? selector = default)
        {
            var query = _dbContext.Set<T>().AsNoTracking();

            if (include is not null)
            {
                query = include(query);
            }

            if (predicate is not null)
            {
                query = query.Where(predicate);
            }

            if (selector is not null)
            {
                query = query.Select(selector);
            }

            return query.AsNoTracking();
        }
    }
}