using System;
using System.Windows;
using TimeLogger.Abstractions;
using TimeLogger.Attributes;

namespace TimeLogger.Services
{
    /// <inheritdoc/>
    [AsSingletone(typeof(IWindowsService))]
    public class WindowsService : IWindowsService
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Show<TWindow>() where TWindow : Window
        {
            GetWindow<TWindow>().Show();
        }

        public void Hide<TWindow>() where TWindow : Window
        {
            GetWindow<TWindow>().Hide();
        }

        private TWindow GetWindow<TWindow>() where TWindow : Window
        {
            var service = _serviceProvider.GetService(typeof(TWindow));

            if (service is not TWindow window)
                throw new ArgumentException();

            return window;
        }
    }
}
