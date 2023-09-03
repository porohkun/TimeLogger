using TimeLogger.Domain.Data;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Вьюмодель окна создания новой активности
    /// </summary>
    public interface INewActivityViewModel : IViewModel
    {
        long Id { get; }
        string Key { get; set; }
        string Name { get; set; }

        void Clear();
        void Show(Activity activity);
    }
}
