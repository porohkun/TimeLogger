namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Вьюмодель активности
    /// </summary>
    public interface IActivityViewModel : IViewModel
    {
        long Id { get; }
        string Key { get; }
        string Name { get; }
        bool Archived { get; }
        bool Pinned { get; }
    }
}
