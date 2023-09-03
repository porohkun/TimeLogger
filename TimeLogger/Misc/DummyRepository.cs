using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeLogger.Shared.Abstractions;

namespace TimeLogger.Misc
{
    public class DummyRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        public Task<TResult> Query<TResult>(Func<IQueryable<T>, TResult> expression) => throw new NotImplementedException();

        public Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? expression = null) => throw new NotImplementedException();

        public Task<T?> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>>? expression = null) => throw new NotImplementedException();

        public Task<long> AddAsync(T entity, bool commit = true) => throw new NotImplementedException();

        public Task AddRangeAsync(IEnumerable<T> entities, bool commit = true) => throw new NotImplementedException();

        public Task UpdateAsync(T entity, bool commit = true) => throw new NotImplementedException();

        public Task<long> AddOrUpdateAsync(T entity, bool commit = true) => throw new NotImplementedException();

        public Task RemoveByIdAsync(long id, bool commit = true) => throw new NotImplementedException();

        public Task<int> RemoveAsync(Func<IQueryable<T>, IQueryable<T>>? expression = null, bool commit = true) => throw new NotImplementedException();

        public Task<int> ClearAsync() => throw new NotImplementedException();

        public Task CommitAsync() => throw new NotImplementedException();
    }
}
