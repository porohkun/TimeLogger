using System;
using System.Windows.Input;
using TimeLogger.Abstractions;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="IActivityViewModel"/>>
    public class ActivityViewModelDummy : BindableBase, IActivityViewModel
    {
        private readonly Activity _activity;

        public long Id => _activity.Id;
        public string Key => _activity.Key ?? "<Null>";
        public string Name => _activity.Name ?? "<Null>";
        public bool Archived => _activity.Archived;
        public bool Pinned => _activity.Pinned;

        public ICommand SelectCommand => throw new NotImplementedException();
        public ICommand ArchiveCommand => throw new NotImplementedException();
        public ICommand UnArchiveCommand => throw new NotImplementedException();

        public ActivityViewModelDummy(Activity activity)
        {
            _activity = activity;
        }
    }
}
