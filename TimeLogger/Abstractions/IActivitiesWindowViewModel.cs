using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TimeLogger.Abstractions
{
    /// <summary>
    /// Вьюмодель окна выбора/создания активностей
    /// </summary>
    public interface IActivitiesWindowViewModel : IViewModel
    {
        bool ShowArchived { get; set; }
        bool ShowNewTaskPane { get; set; }
        IActivityViewModel? SelectedActivity { get; set; }
        INewActivityViewModel EditingActivity { get; }

        ObservableCollection<IActivityViewModel> Activities { get; }
        ObservableCollection<ITagViewModel> Tags { get; }

        public ICommand CreateNewActivityCommand { get; }
        public ICommand CancelNewActivityCommand { get; }
        public ICommand SelectActivityCommand { get; }
        public ICommand CloseCommand { get; }
    }
}
