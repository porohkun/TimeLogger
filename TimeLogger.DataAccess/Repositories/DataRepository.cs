using Microsoft.EntityFrameworkCore;
using TimeLogger.DataAccess.Data;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.DataAccess.Repositories
{
    public class DataRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly DataContext _dataContext;

        public DataRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<TResult> Query<TResult>(Func<IQueryable<T>, TResult> expression)
        {
            IQueryable<T> query = _dataContext.Set<T>();
            return await Task.Run(() => expression(query));
        }

        public async Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? expression = null)
        {
            IQueryable<T> query = _dataContext.Set<T>();

            if (expression != null)
                query = expression(query);

            return await Task.Run(() => query.ToArray());
        }

        public async Task<T?> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>>? expression = null)
        {
            IQueryable<T> query = _dataContext.Set<T>();

            if (expression != null)
                query = expression(query);

            return await Task.Run(() => query.SingleOrDefault(x => x.Id == id));
        }

        public async Task<long> AddAsync(T entity, bool commit = true)
        {
            _dataContext.Set<T>().Add(entity);

            if (commit)
            {
                await _dataContext.SaveChangesAsync();
                return entity.Id;
            }
            return -1;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities, bool commit = true)
        {
            _dataContext.Set<T>().AddRange(entities);

            if (commit)
                await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity, bool commit = true)
        {
            _dataContext.Set<T>().Update(entity);

            if (commit)
                await _dataContext.SaveChangesAsync();
        }

        public async Task<long> AddOrUpdateAsync(T entity, bool commit = true)
        {
            var isExists = await _dataContext.Set<T>().AnyAsync(q => q.Id == entity.Id);
            if (isExists)
                await UpdateAsync(entity, commit);
            else
                return await AddAsync(entity, commit);
            return entity.Id;
        }

        public async Task RemoveByIdAsync(long id, bool commit = true)
        {
            IQueryable<T> query = _dataContext.Set<T>();

            await query.Where(e => e.Id == id).ExecuteDeleteAsync();

            if (commit)
                await _dataContext.SaveChangesAsync();
        }

        public async Task<int> RemoveAsync(Func<IQueryable<T>, IQueryable<T>>? expression = null, bool commit = true)
        {
            IQueryable<T> query = _dataContext.Set<T>();

            if (expression != null)
                query = expression(query);

            return await Task.Run(() => query.ExecuteDeleteAsync());
        }

        public async Task<int> ClearAsync()
        {
            return await _dataContext.ClearSet<T>();
        }

        public async Task CommitAsync()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
