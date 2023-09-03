using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    [AsTransient(typeof(IActivityViewModel))]
    public class ActivityViewModel : BindableBase, IActivityViewModel
    {
        private readonly Activity _activity;
        private readonly Action<IActivityViewModel>? _selectActivityCallback;

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
            Action<IActivityViewModel>? selectActivityCallback = null)
        {
            _activity = activity;
            _selectActivityCallback = selectActivityCallback;
            SelectCommand = new RelayCommand(SelectActivity);
        }

        private void SelectActivity()
        {
            _selectActivityCallback?.Invoke(this);
        }
    }
}
