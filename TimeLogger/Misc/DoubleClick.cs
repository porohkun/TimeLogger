using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TimeLogger.Misc
{
    public static class DoubleClick
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(DoubleClick), new PropertyMetadata(null, CommandChanged));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(DoubleClick), new PropertyMetadata(null));

        public static ICommand? GetCommand(DependencyObject obj)
        {
            return obj.GetValue(CommandProperty) as ICommand;
        }

        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Control control)
                return;
            control.MouseDoubleClick -= Control_MouseDoubleClick;
            control.MouseDoubleClick += Control_MouseDoubleClick;
        }

        private static void Control_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Control control)
                return;
            var command = GetCommand(control);
            if (command != null && command.CanExecute(GetCommandParameter(control)))
            {
                command.Execute(GetCommandParameter(control));
            }
        }
    }
}
