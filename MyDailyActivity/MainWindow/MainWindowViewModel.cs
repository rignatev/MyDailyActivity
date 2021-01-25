using System;
using System.Collections.Generic;
using System.Reactive;

using Client.Shared.ViewModels;

using MyDailyActivity.Tasks;

using ReactiveUI;

namespace MyDailyActivity.MainWindow
{
    public class MainWindowViewModel : ReactiveWindowViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;

        public IReadOnlyList<MenuItemViewModel> MenuItems { get; }

        public ReactiveCommand<Unit, Unit> OpenTasksWindowViewCommand { get; }

        public string Greeting => "Hello World!";

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            this.OpenTasksWindowViewCommand = ReactiveCommand.Create(OpenTasksWindowView);

            this.MenuItems = CreateMenu();
        }

        private void OpenTasksWindowView()
        {
            var tasksWindow = new TasksWindowView { DataContext = new TasksWindowViewModel(_serviceProvider) };

            // tasksWindow.Closing += TasksWindowOnClosing;

            tasksWindow.Show();
        }

        private IReadOnlyList<MenuItemViewModel> CreateMenu()
        {
            return new[]
            {
                new MenuItemViewModel
                {
                    Header = "_Windows",
                    Items = new[]
                    {
                        new MenuItemViewModel
                        {
                            Header = "_Tasks",
                            Command = this.OpenTasksWindowViewCommand
                        }
                    }
                }
            };
        }
    }
}
