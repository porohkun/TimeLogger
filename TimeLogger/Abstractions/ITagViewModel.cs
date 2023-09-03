namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Вьюмодель тега
    /// </summary>
    public interface ITagViewModel : IViewModel
    {
        string Name { get; }
        int ActivitiesCount { get; }
        bool WithArchived { get; set; }
    }
}
