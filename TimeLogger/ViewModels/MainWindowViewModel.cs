using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        public bool ActivitySelected => _activityService.SelectedActivity is not null;

        public bool ActivityStarted => _activityService.IsStarted;

        public ObservableCollection<ITimeViewModel> Indicators { get; } = new();

        public ICommand ShowActivitiesCommand { get; }
        public ICommand StartActivityCommand { get; }
        public ICommand StopActivityCommand { get; }

        public MainWindowViewModel(
            IWindowsService windowsService,
            IActivityService activityService,
            IIndicatorsService indicatorsService,
            TimeViewModelFactory timeViewModelFactory)
        {
            _windowsService = windowsService;
            _activityService = activityService;

            _activityService.ActivitySelected += () =>
            {
                RaisePropertyChanged(nameof(ActivityName));
                RaisePropertyChanged(nameof(ActivitySelected));
            };
            _activityService.IsStartedChanged += _ => RaisePropertyChanged(nameof(ActivityStarted));

            indicatorsService.IndicatorsChanged += () =>
            {
                Indicators.Clear();
                Indicators.AddRange(indicatorsService.IndicatorsNames.Select(timeViewModelFactory.Create));
            };

            ShowActivitiesCommand = new RelayCommand(ShowActivities);
            StartActivityCommand = new AsyncRelayCommand(StartActivity);
            StopActivityCommand = new AsyncRelayCommand(StopActivity);
        }

        private void ShowActivities()
        {
            _windowsService.Show<ActivitiesWindow>();
        }

        private async Task StartActivity()
        {
            if (!ActivitySelected || ActivityStarted) return;

            await _activityService.StartActivity(_activityService.SelectedActivity!.Id);
        }

        private async Task StopActivity()
        {
            if (!ActivitySelected || !ActivityStarted) return;

            await _activityService.StopActivity(_activityService.SelectedActivity!.Id);
        }
    }
}
