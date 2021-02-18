using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;

using Client.Shared.ViewModels;

using Contracts.Shared.Models;

using Infrastructure.Shared.OperationResult;
using Infrastructure.Shared.Utils;

using Microsoft.Extensions.DependencyInjection;

using MyDailyActivity.Activities;
using MyDailyActivity.Projects;
using MyDailyActivity.Tasks;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Services.Contracts.Activities;

namespace MyDailyActivity.MainWindow
{
    public class MainWindowViewModel : ReactiveWindowViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly IActivityService _activityService;

        [Reactive]
        private DateTime StartDateTimeUtc { get; set; }

        [Reactive]
        private string Description { get; set; }

        private ReactiveCommand<Unit, Unit> DoneCommand { get; }

        private IReadOnlyList<MenuItemViewModel> MenuItems { get; }

        private ReactiveCommand<Unit, Unit> OpenActivitiesWindowCommand { get; set; }

        private ReactiveCommand<Unit, Unit> OpenProjectsWindowCommand { get; set; }

        private ReactiveCommand<Unit, Unit> OpenTasksWindowCommand { get; set; }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _serviceScope = serviceProvider.CreateScope();
            _activityService = _serviceScope.ServiceProvider.GetRequiredService<IActivityService>();

            this.StartDateTimeUtc = DateTime.UtcNow;
            this.Description = "Some description...";

            IObservable<bool> canExecuteDoneCommand = this.WhenAnyValue(x => x.Description, description => description.IsNotNullOrEmpty());
            this.DoneCommand = ReactiveCommand.Create(DoneCommandAction, canExecuteDoneCommand);

            this.MenuItems = CreateMenu();
        }

        /// <inheritdoc />
        protected override void HandleDeactivation(CompositeDisposable disposables)
        {
            _serviceScope.DisposeWith(disposables);
        }

        private void DoneCommandAction()
        {
            DateTime utcNow = DateTime.UtcNow;
            var activity = new ActivityModel
            {
                CreatedDateTimeUtc = utcNow,
                IsHidden = false,
                Description = this.Description,
                StartDateTimeUtc = this.StartDateTimeUtc,
                EndDateTimeUtc = utcNow,
                Duration = utcNow - this.StartDateTimeUtc
            };

            OperationResult<int> createResult = _activityService.Create(activity);
            if (createResult.Success)
            {
                this.StartDateTimeUtc = utcNow;
            }
            else
            {
                ShowErrorDialog($"Failed to create activity: {createResult.Error.Message}");
            }
        }

        private void OpenActivitiesWindowAction()
        {
            var activitiesWindow = new ActivitiesWindowView { DataContext = new ActivitiesWindowViewModel(_serviceProvider) };

            activitiesWindow.ViewModel.ActivitiesChanged.Subscribe(activityChangeSet => Console.WriteLine(activityChangeSet.Count));
            // activitiesWindow.Closing += ActivitiesWindowOnClosing;

            activitiesWindow.Show();
        }

        private void OpenProjectsWindowAction()
        {
            var projectsWindow = new ProjectsWindowView { DataContext = new ProjectsWindowViewModel(_serviceProvider) };

            projectsWindow.ViewModel.ProjectsChanged.Subscribe(projectChangeSet => Console.WriteLine(projectChangeSet.Count));
            // projectsWindow.Closing += ProjectsWindowOnClosing;

            projectsWindow.Show();
        }

        private void OpenTasksWindowAction()
        {
            var tasksWindow = new TasksWindowView { DataContext = new TasksWindowViewModel(_serviceProvider) };

            tasksWindow.ViewModel.TasksChanged.Subscribe(taskChangeSet => Console.WriteLine(taskChangeSet.Count));
            // tasksWindow.Closing += TasksWindowOnClosing;

            tasksWindow.Show();
        }

        private IReadOnlyList<MenuItemViewModel> CreateMenu()
        {
            this.OpenActivitiesWindowCommand = ReactiveCommand.Create(OpenActivitiesWindowAction);
            this.OpenProjectsWindowCommand = ReactiveCommand.Create(OpenProjectsWindowAction);
            this.OpenTasksWindowCommand = ReactiveCommand.Create(OpenTasksWindowAction);

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
                            Command = this.OpenActivitiesWindowCommand
                        },
                        new MenuItemViewModel
                        {
                            Header = "_Projects",
                            Command = this.OpenProjectsWindowCommand
                        },
                        new MenuItemViewModel
                        {
                            Header = "_Tasks",
                            Command = this.OpenTasksWindowCommand
                        }
                    }
                }
            };
        }
    }
}
