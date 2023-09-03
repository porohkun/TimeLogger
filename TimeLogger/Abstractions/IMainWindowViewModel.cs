using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Вьюмодель главного окна
    /// </summary>
    public interface IMainWindowViewModel : IViewModel
    {
        string ActivityName { get; }
        ObservableCollection<ITimeViewModel> Indicators { get; }
        ICommand ShowActivitiesCommand { get; }
    }
}
