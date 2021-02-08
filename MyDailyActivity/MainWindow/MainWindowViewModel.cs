using System;
using System.Collections.Generic;
using System.Reactive;

using Client.Shared.ViewModels;

using MyDailyActivity.Activities;
using MyDailyActivity.Projects;
using MyDailyActivity.Tasks;

using ReactiveUI;

namespace MyDailyActivity.MainWindow
{
    public class MainWindowViewModel : ReactiveWindowViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;

        private IReadOnlyList<MenuItemViewModel> MenuItems { get; }

        private ReactiveCommand<Unit, Unit> OpenActivitiesWindowViewCommand { get; }

        private ReactiveCommand<Unit, Unit> OpenProjectsWindowViewCommand { get; }

        private ReactiveCommand<Unit, Unit> OpenTasksWindowViewCommand { get; }

        public string Greeting => "Hello World!";

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            this.OpenActivitiesWindowViewCommand = ReactiveCommand.Create(OpenActivitiesWindowView);
            this.OpenProjectsWindowViewCommand = ReactiveCommand.Create(OpenProjectsWindowView);
            this.OpenTasksWindowViewCommand = ReactiveCommand.Create(OpenTasksWindowView);

            this.MenuItems = CreateMenu();
        }

        private void OpenActivitiesWindowView()
        {
            var activitiesWindow = new ActivitiesWindowView { DataContext = new ActivitiesWindowViewModel(_serviceProvider) };

            activitiesWindow.ViewModel.ActivitiesChanged.Subscribe(activityChangeSet => Console.WriteLine(activityChangeSet.Count));
            // activitiesWindow.Closing += ActivitiesWindowOnClosing;

            activitiesWindow.Show();
        }

        private void OpenProjectsWindowView()
        {
            var projectsWindow = new ProjectsWindowView { DataContext = new ProjectsWindowViewModel(_serviceProvider) };

            projectsWindow.ViewModel.ProjectsChanged.Subscribe(projectChangeSet => Console.WriteLine(projectChangeSet.Count));
            // projectsWindow.Closing += ProjectsWindowOnClosing;

            projectsWindow.Show();
        }

        private void OpenTasksWindowView()
        {
            var tasksWindow = new TasksWindowView { DataContext = new TasksWindowViewModel(_serviceProvider) };

            tasksWindow.ViewModel.TasksChanged.Subscribe(taskChangeSet => Console.WriteLine(taskChangeSet.Count));
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
                            Header = "_Activities",
                            Command = this.OpenActivitiesWindowViewCommand
                        },
                        new MenuItemViewModel
                        {
                            Header = "_Projects",
                            Command = this.OpenProjectsWindowViewCommand
                        },
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
