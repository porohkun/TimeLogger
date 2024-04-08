using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="IActivityViewModel"/>>
    [AsTransient(typeof(IActivityViewModel))]
    public class ActivityViewModel : BindableBase, IActivityViewModel
    {
        private readonly IActivityService _activityService;
        private readonly Activity _activity;

        public long Id => _activity.Id;
        public string Key => _activity.Key ?? "<Null>";
        public string Name => _activity.Name ?? "<Null>";
        public bool Archived
        {
            get => _activity.Archived;
            set => SetProperty(_activity, a => a.Archived, value);
        }
        public bool Pinned
        {
            get => _activity.Pinned;
            set => SetProperty(_activity, a => a.Pinned, value);
        }

        public ICommand SelectCommand { get; }
        public ICommand ArchiveCommand { get; }
        public ICommand UnArchiveCommand { get; }

        public ActivityViewModel(
            Activity activity,
            IActivityService activityService)
        {
            _activityService = activityService;
            _activity = activity;

            _activityService.ActivityUpdated += _activityService_ActivityUpdated;

            SelectCommand = new AsyncRelayCommand(SelectActivity);
            ArchiveCommand = new AsyncRelayCommand(ArchiveActivity);
            UnArchiveCommand = new AsyncRelayCommand(UnArchiveActivity);
        }

        private void _activityService_ActivityUpdated(long id)
        {
            if (Id != id) return;

            RaisePropertyChanged(nameof(Key));
            RaisePropertyChanged(nameof(Name));
            RaisePropertyChanged(nameof(Archived));
            RaisePropertyChanged(nameof(Pinned));
        }

        private async Task SelectActivity()
        {
            await _activityService.SelectActivity(Id);
        }

        private async Task ArchiveActivity()
        {
            await _activityService.ArchiveActivity(Id, true);
        }

        private async Task UnArchiveActivity()
        {
            await _activityService.ArchiveActivity(Id, false);
        }
    }
}
