using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using TimeLogger.MVVM;
using TimeLogger.Views;

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddViewModelLocator();

            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
