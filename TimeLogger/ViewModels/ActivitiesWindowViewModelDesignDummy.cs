using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TimeLogger.Abstractions;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="IActivitiesWindowViewModel" />
    public class ActivitiesWindowViewModelDesignDummy : BindableBase, IActivitiesWindowViewModel
    {
        public bool ShowArchived { get; set; }
        public bool ShowNewTaskPane { get; set; } = false;
        public IActivityViewModel? SelectedActivity { get; set; }
        public INewActivityViewModel EditingActivity { get; } = new NewActivityViewModelDesignDummy();

        public ObservableCollection<IActivityViewModel> Activities { get; } = new();

        public ObservableCollection<ITagViewModel> Tags { get; } = new();

        public ICommand CreateNewActivityCommand => throw new NotImplementedException();
        public ICommand CancelNewActivityCommand => throw new NotImplementedException();
        public ICommand SelectActivityCommand => throw new NotImplementedException();
        public ICommand CloseCommand => throw new NotImplementedException();

        public ActivitiesWindowViewModelDesignDummy()
        {
            Tags.Add(new TagViewModel(new Tag
            {
                Id = 1,
                Name = "tag1"
            },
            Activities));
            Tags.Add(new TagViewModel(new Tag
            {
                Id = 2,
                Name = "tag2"
            },
            Activities));
            Tags.Add(new TagViewModel(new Tag
            {
                Id = 3,
                Name = "tag3"
            },
            Activities));

            Activities.Add(new ActivityViewModelDummy(new Activity
            {
                Id = 1,
                Key = "ACT-1",
                Name = "activity 1",

            }));
            Activities.Add(new ActivityViewModelDummy(new Activity
            {
                Id = 2,
                Key = "ACT-2",
                Name = "activity 2"
            }));
            Activities.Add(new ActivityViewModelDummy(new Activity
            {
                Id = 3,
                Key = "ACT-3",
                Name = "activity 3"
            }));

            SelectedActivity = Activities[1];
        }
    }
}
