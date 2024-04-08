using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.MVVM;
using TimeLogger.Views;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="IMainWindowViewModel" />
    [AsSingletone(typeof(IMainWindowViewModel))]
    public class MainWindowViewModel : BindableBase, IMainWindowViewModel
    {
        private readonly IWindowsService _windowsService;
        private readonly IActivityService _activityService;

        public string ActivityName => _activityService.SelectedActivity?.Name ?? "[Time Logger]";

        public ObservableCollection<ITimeViewModel> Indicators { get; } = new()
        {
            new TimeViewModel { Name = "Today"},
            new TimeViewModel { Name = "Task"},
            new TimeViewModel { Name = "Task today"},
            new TimeViewModel { Name = "Period"}
        };

        public ICommand ShowActivitiesCommand { get; }

        public MainWindowViewModel(IWindowsService windowsService, IActivityService activityService)
            : base()
        {
            _windowsService = windowsService;
            _activityService = activityService;

            _activityService.ActivitySelected += () => RaisePropertyChanged(nameof(ActivityName));

            ShowActivitiesCommand = new RelayCommand(ShowActivities);
        }

        private void ShowActivities()
        {
            _windowsService.Show<ActivitiesWindow>();
        }
    }
}
