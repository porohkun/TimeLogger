using System.Windows;
using System.Windows.Controls;

namespace TimeLogger.Misc
{
    public static class CloseButtonBehavior
    {
        public static bool GetInvoke(DependencyObject obj)
        {
            return (bool)obj.GetValue(InvokeProperty);
        }

        public static void SetInvoke(DependencyObject obj, bool value)
        {
            obj.SetValue(InvokeProperty, value);
        }

        public static readonly DependencyProperty InvokeProperty =
            DependencyProperty.RegisterAttached("Invoke", typeof(bool), typeof(CloseButtonBehavior), new PropertyMetadata(false, OnInvokeChanged));

        private static void OnInvokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button && e.NewValue is bool and true)
            {
                button.Click += Button_Click;
            }
        }

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not DependencyObject dobj)
                return;
            Window.GetWindow(dobj)?.Hide();
        }
    }
}
