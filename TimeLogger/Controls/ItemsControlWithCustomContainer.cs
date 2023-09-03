using System.Windows.Controls;

namespace TimeLogger.Controls
{
    public class ItemsControlWithCustomContainer : ItemsControl
    {
        protected override bool IsItemItsOwnContainerOverride(object item) { return false; }
    }
}
