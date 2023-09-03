using System.Windows;

namespace TimeLogger.Views
{
    public class WindowBase : Window
    {
        protected void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
