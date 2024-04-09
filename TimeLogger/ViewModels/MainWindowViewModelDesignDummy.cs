using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeLogger.Abstractions;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="IMainWindowViewModel" />
    public class MainWindowViewModelDesignDummy : BindableBase, IMainWindowViewModel
    {
        public string ActivityName => "TSK-42 че-то там про главный вопрос туды-сюды";

        public bool ActivitySelected => true;

        public bool ActivityStarted => false;

        public ObservableCollection<ITimeViewModel> Indicators { get; } = new()
        {
            new TimeViewModel() { Name = "Today"},
            new TimeViewModel() { Name = "Task"},
            new TimeViewModel() { Name = "Task today"},
            new TimeViewModel() { Name = "Period"}
        };

        public ICommand ShowActivitiesCommand => throw new NotImplementedException();

        public ICommand StartActivityCommand => throw new NotImplementedException();

        public ICommand StopActivityCommand => throw new NotImplementedException();
    }
}
