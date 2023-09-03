using System;
using System.ComponentModel;
using System.Windows;

namespace TimeLogger.MVVM
{
    public static class ViewModelLocator
    {
        public static DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached("ViewModel", typeof(Type), typeof(ViewModelLocator), new PropertyMetadata(typeof(object), ViewModelChanged));

        public static Type GetViewModel(DependencyObject obj)
        {
            return (Type)obj.GetValue(ViewModelProperty);
        }

        public static void SetViewModel(DependencyObject obj, Type value)
        {
            obj.SetValue(ViewModelProperty, value);
        }

        private static void ViewModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(obj) && e.NewValue is Type viewModelType)
            {
                var viewModel = App.Current.Services.GetService(viewModelType);
                Bind(obj, viewModel);
            }
        }

        private static void Bind(object view, object? viewModel)
        {
            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.DataContext = viewModel;
            }
        }
    }
}
