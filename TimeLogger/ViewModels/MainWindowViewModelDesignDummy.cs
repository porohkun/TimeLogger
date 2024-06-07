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
            new TimeViewModelDesignDummy { Name = "Today", Time = new TimeSpan(1, 15, 36)},
            new TimeViewModelDesignDummy { Name = "Task", Time = new TimeSpan(1, 3, 10, 15, 327)},
            new TimeViewModelDesignDummy { Name = "Task today", Time = new TimeSpan(0, 0, 15)},
            new TimeViewModelDesignDummy { Name = "Period", }
        };

        public ICommand ShowActivitiesCommand => throw new NotImplementedException();

        public ICommand StartActivityCommand => throw new NotImplementedException();

        public ICommand StopActivityCommand => throw new NotImplementedException();
    }
}
