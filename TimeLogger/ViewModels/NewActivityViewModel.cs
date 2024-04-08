using TimeLogger.Abstractions;
using TimeLogger.Attributes;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc cref="INewActivityViewModel" />
    [AsTransient(typeof(INewActivityViewModel))]
    public class NewActivityViewModel : BindableBase, INewActivityViewModel
    {
        private long _id = 0;
        private string _key = string.Empty;
        private string _name = string.Empty;

        public long Id
        {
            get => _id;
            private set => SetProperty(ref _id, value);
        }
        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public void Clear()
        {
            Id = 0;
            Key = string.Empty;
            Name = string.Empty;
        }

        public void Show(Activity activity)
        {
            Id = activity.Id;
            Key = activity.Key ?? string.Empty;
            Name = activity.Name ?? string.Empty;
        }
    }
}
