using System.Collections.ObjectModel;
using System.Linq;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="ITagViewModel" />
    [AsTransient(typeof(ITagViewModel))]
    public class TagViewModel : BindableBase, ITagViewModel
    {
        private readonly Tag _tag;
        private readonly ObservableCollection<IActivityViewModel> _activities;

        public string Name => _tag.Name ?? "<Null>";
        public int ActivitiesCount => WithArchived ? _activities.Count : _activities.Count(a => !a.Archived);
        public bool WithArchived { get; set; }

        public TagViewModel(Tag tag, ObservableCollection<IActivityViewModel> activities)
        {
            _tag = tag;
            _activities = activities;

            _activities.CollectionChanged += (_, _) => RaisePropertyChanged(nameof(ActivitiesCount));
        }
    }
}
