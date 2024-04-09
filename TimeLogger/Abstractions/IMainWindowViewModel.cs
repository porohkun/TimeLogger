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
        bool ActivitySelected { get; }
        bool ActivityStarted { get; }
        ObservableCollection<ITimeViewModel> Indicators { get; }
        ICommand ShowActivitiesCommand { get; }
        ICommand StartActivityCommand { get; }
        ICommand StopActivityCommand { get; }
    }
}
