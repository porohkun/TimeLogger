using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;
using TimeLogger.Shared.Abstractions;
using TimeLogger.Views;
using Activity = TimeLogger.Domain.Data.Activity;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="IActivitiesWindowViewModel" />
    [AsSingletone(typeof(IActivitiesWindowViewModel))]
    public class ActivitiesWindowViewModel : BindableBase, IActivitiesWindowViewModel
    {
        private readonly IUIDispatcherService _dispatcherService;
        private readonly IActivityService _activityService;
        private readonly IWindowsService _windowsService;
        private readonly ActivityViewModelFactory _activityFactory;
        private readonly TagViewModelFactory _tagFactory;
        private readonly IRepository<Activity> _activitiesRepo;
        private readonly IRepository<Tag> _tagsRepo;

        private readonly ICollectionView _activitiesView;

        private bool _showArchived = false;
        private bool _showNewTaskPane = false;
        private IActivityViewModel? _selectedActivity;

        public bool ShowArchived
        {
            get => _showArchived;
            set
            {
                if (SetProperty(ref _showArchived, value))
                    _activitiesView.Refresh();
            }
        }

        public bool ShowNewTaskPane
        {
            get => _showNewTaskPane;
            set => SetProperty(ref _showNewTaskPane, value);
        }
        public IActivityViewModel? SelectedActivity
        {
            get => _selectedActivity;
            set => SetProperty(ref _selectedActivity, value);
        }
        public INewActivityViewModel EditingActivity { get; } = new NewActivityViewModel();

        public ObservableCollection<IActivityViewModel> Activities { get; } = new();
        public ObservableCollection<ITagViewModel> Tags { get; } = new();

        public ICommand CreateNewActivityCommand { get; }
        public ICommand CancelNewActivityCommand { get; }
        public ICommand EditActivityCommand { get; }
        public ICommand SelectActivityCommand { get; }
        public ICommand CloseCommand { get; }

        public ActivitiesWindowViewModel(
            IUIDispatcherService dispatcherService,
            IActivityService activityService,
            IWindowsService windowsService,
            ActivityViewModelFactory activityFactory,
            TagViewModelFactory tagFactory,
            IRepository<Activity> activitiesRepo,
            IRepository<Tag> tagsRepo)
        {
            _dispatcherService = dispatcherService;
            _activityService = activityService;
            _windowsService = windowsService;
            _activityFactory = activityFactory;
            _tagFactory = tagFactory;
            _activitiesRepo = activitiesRepo;
            _tagsRepo = tagsRepo;

            _activityService.ActivitySelected += () => CloseCommand?.Execute(null);
            _activityService.ActivityUpdated += _ => _activitiesView?.Refresh();

            CreateNewActivityCommand = new AsyncRelayCommand(CreateNewActivity);
            CancelNewActivityCommand = new RelayCommand(CancelNewActivity);
            EditActivityCommand = new AsyncRelayCommand<IActivityViewModel>(EditActivity);
            SelectActivityCommand = new AsyncRelayCommand<IActivityViewModel>(SelectActivity);
            CloseCommand = new RelayCommand(_windowsService.Hide<ActivitiesWindow>);

            _activitiesView = (CollectionView)CollectionViewSource.GetDefaultView(Activities);
            _activitiesView.Filter = a =>
            {
                if (!(a is IActivityViewModel activity)) return false;
                if (ShowArchived != activity.Archived)
                    return false;
                return true;
            };

            Task.Run(LoadActivities);
        }

        private async void LoadActivities()
        {
            var activities = (await _activitiesRepo.GetAllAsync())
                .Select(a => _activityFactory.Create(a))
                .ToArray();
            var tags = (await _tagsRepo.GetAllAsync()).Select(_tagFactory.Create).ToArray();
            await _dispatcherService.InvokeAsync(() => Activities.AddRange(activities));
            await _dispatcherService.InvokeAsync(() => Tags.AddRange(tags));
        }

        private async Task CreateNewActivity()
        {
            var id = EditingActivity.Id;
            if (EditingActivity.Id != 0)
                await _activityService.UpdateActivity(
                    EditingActivity.Id,
                    EditingActivity.Key,
                    EditingActivity.Name,
                    Array.Empty<string>());
            else
            {
                id = await _activityService.CreateNewActivity(
                     EditingActivity.Key,
                     EditingActivity.Name,
                     Array.Empty<string>());
                var activity = await _activitiesRepo.GetByIdAsync(id);
                var activityView = _activityFactory.Create(activity!);
                Activities.Add(activityView);
            }

            SelectedActivity = Activities.FirstOrDefault(a => a.Id == id);

            CancelNewActivity();
        }

        private async Task EditActivity(IActivityViewModel? activityViewModel)
        {
            if (activityViewModel is null) return;
            var activity = await _activitiesRepo.GetByIdAsync(activityViewModel.Id);
            if (activity == null) return;
            EditingActivity.Show(activity);
            DialogHost.OpenDialogCommand.Execute(null, null);
        }

        private void CancelNewActivity()
        {
            EditingActivity.Clear();
            DialogHost.CloseDialogCommand.Execute(true, null);
        }

        private async Task SelectActivity(IActivityViewModel? activityViewModel)
        {
            if (activityViewModel is null) return;
            await _activityService.SelectActivity(activityViewModel.Id);
        }
    }
}
