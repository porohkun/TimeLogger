using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TimeLogger.DataAccess.Data;
using TimeLogger.DataAccess.Repositories;
using TimeLogger.Domain.Common.Contracts;
using TimeLogger.Misc;
using TimeLogger.Shared.Abstractions;
using TimeLogger.Views;

namespace TimeLogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        private readonly DatabaseInitializer _initializer;
        public App()
        {
            Services = ConfigureServices();
            _initializer = Services.GetRequiredService<DatabaseInitializer>();

            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        public IServiceProvider Services { get; }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingletonsFromAttributes();
            services.AddTransientsFromAttributes();
            services.AddDbContext<DataContext>(o =>
                o.UseSqlite($"Data Source={Consts.AppDataPath}/data.db"));
            services.AddTransient<DatabaseInitializer>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ActivitiesWindow>();

            foreach (var type in typeof(BaseEntity).GetInheritors())
                services.AddSingleton(typeof(IRepository<>).MakeGenericType(type),
                    typeof(DataRepository<>).MakeGenericType(type));

            return services.BuildServiceProvider();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            _initializer.Initialize();
            var mainWindow = Services.GetService<MainWindow>();
            mainWindow?.Show();
        }
    }
}
