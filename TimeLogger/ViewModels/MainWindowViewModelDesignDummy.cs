using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeLogger.Abstractions;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc/>
    public class MainWindowViewModelDesignDummy : BindableBase, IMainWindowViewModel
    {
        public string ActivityName => "TSK-42 че-то там про главный вопрос туды-сюды";

        public ObservableCollection<ITimeViewModel> Indicators { get; } = new()
        {
            new TimeViewModel() { Name = "Today"},
            new TimeViewModel() { Name = "Task"},
            new TimeViewModel() { Name = "Task today"},
            new TimeViewModel() { Name = "Period"}
        };

        public ICommand ShowActivitiesCommand => throw new NotImplementedException();

    }
}
