using TimeLogger.Abstractions;
using TimeLogger.Domain.Data;
using TimeLogger.MVVM;

namespace TimeLogger.ViewModels
{
    /// <inheritdoc/>
    public class NewActivityViewModelDesignDummy : BindableBase, INewActivityViewModel
    {
        public long Id { get; } = 0;
        public string Key { get; set; } = "TSK-1";
        public string Name { get; set; } = "My new task";

        public void Clear() { }
        public void Show(Activity activity) { }
    }
}
