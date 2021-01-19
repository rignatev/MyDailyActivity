using System;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MyDailyActivity.ViewModels;
using MyDailyActivity.Views;

using Services;

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

                ServicesConfigurator.ConfigureServices(serviceCollection, configuration.GetConnectionString("Sqlite"));

                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
