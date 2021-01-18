using System;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Data.EF.Core;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDailyActivity.ViewModels;
using MyDailyActivity.Views;

namespace MyDailyActivity
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                var serviceCollection = new ServiceCollection();

                ConfigureServices(serviceCollection, configuration);

                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
            }

            base.OnFrameworkInitializationCompleted();
        }

        static private void ConfigureServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            DataServicesConfigurator.ConfigureServices(serviceCollection, configuration.GetConnectionString("Sqlite"));
        }
    }
}
