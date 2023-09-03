namespace TimeLogger.Shared.Abstractions
{
    public interface IRepository<T> where T : class, IBaseEntity
    {
        Task<TResult> Query<TResult>(Func<IQueryable<T>, TResult> expression);
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? expression = null);
        Task<T?> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>>? expression = null);
        Task<long> AddAsync(T entity, bool commit = true);
        Task AddRangeAsync(IEnumerable<T> entities, bool commit = true);
        Task UpdateAsync(T entity, bool commit = true);
        Task<long> AddOrUpdateAsync(T entity, bool commit = true);
        Task RemoveByIdAsync(long id, bool commit = true);
        Task<int> RemoveAsync(Func<IQueryable<T>, IQueryable<T>>? expression = null, bool commit = true);
        Task<int> ClearAsync();
        Task CommitAsync();
    }
}
