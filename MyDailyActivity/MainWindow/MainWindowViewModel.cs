using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;

using Client.Shared.ViewModels;

using Contracts.Shared.Models;

using DynamicData;

using Infrastructure.Shared.OperationResult;
using Infrastructure.Shared.Utils;

using Microsoft.Extensions.DependencyInjection;

using MyDailyActivity.Activities;
using MyDailyActivity.Projects;
using MyDailyActivity.Tasks;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Services.Contracts.Activities;
using Services.Contracts.EntityServices;
using Services.Contracts.Projects;
using Services.Contracts.Tasks;

namespace MyDailyActivity.MainWindow
{
    public class MainWindowViewModel : ReactiveWindowViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;
        private readonly IActivityService _activityService;
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly SourceCache<ProjectModel, int> _projectsSource = new(x => x.Id);
        private readonly SourceCache<TaskModel, int> _tasksSource = new(x => x.Id);
        private readonly ReadOnlyObservableCollection<ProjectModel> _projectItems;
        private readonly ReadOnlyObservableCollection<TaskModel> _tasksItems;

        private WindowBase _activitiesWindow;
        private WindowBase _projectsWindow;
        private WindowBase _tasksWindow;

        private IEnumerable<ProjectModel> ProjectItems => _projectItems;

        private IEnumerable<TaskModel> TaskItems => _tasksItems;

        [Reactive]
        private ProjectModel SelectedProject { get; set; }

        [Reactive]
        private string ProjectText { get; set; }

        [Reactive]
        private TaskModel SelectedTask { get; set; }

        [Reactive]
        private string TaskText { get; set; }

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
            _projectService = _serviceScope.ServiceProvider.GetRequiredService<IProjectService>();
            _taskService = _serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

            _projectsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _projectItems)
                .DisposeMany()
                .Subscribe();

            _tasksSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _tasksItems)
                .DisposeMany()
                .Subscribe();

            IObservable<bool> canExecuteDoneCommand = this.WhenAnyValue(
                x => x.Description,
                x => x.ProjectText,
                x => x.TaskText,
                (description, projectText, taskText) =>
                {
                    return
                        description.IsNotNullOrEmpty() &&
                        (projectText.IsNullOrEmpty() || this.ProjectItems.Any(x => x.Name.Equals(projectText, StringComparison.Ordinal))) &&
                        (taskText.IsNullOrEmpty() || this.TaskItems.Any(x => x.Name.Equals(taskText, StringComparison.Ordinal)));
                }
            );

            this.DoneCommand = ReactiveCommand.CreateFromTask(DoneCommandAction, canExecuteDoneCommand);

            this.MenuItems = CreateMenu();
        }

        /// <inheritdoc />
        protected override void HandleActivation(CompositeDisposable disposables)
        {
            Observable.StartAsync(InitializeProjectItems);
            Observable.StartAsync(InitializeTaskItems);
            Observable.StartAsync(InitializeInitialActivity);
        }

        /// <inheritdoc />
        protected override void HandleDeactivation(CompositeDisposable disposables)
        {
            _serviceScope.DisposeWith(disposables);
        }

        private async Task InitializeInitialActivity()
        {
            ActivityModel initialActivity = await DoActionAsync(() => _activityService.CreateInitialActivity().GetValueOrThrow());

            this.StartDateTimeUtc = initialActivity.StartDateTimeUtc;
            this.Description = initialActivity.Description;
            this.SelectedProject = initialActivity.Project;
            this.SelectedTask = initialActivity.Task;
        }

        private async Task InitializeProjectItems()
        {
            var getEntitiesParameters = new EntityServiceGetEntitiesParameters<ProjectModel, int>
            {
                OrderByProperty = x => x.Id,
                OrderByDescending = true
            };

            OperationResult<List<ProjectModel>> getEntitiesResult =
                await DoActionAsync(() => _projectService.GetEntities(getEntitiesParameters));

            if (getEntitiesResult.Success)
            {
                List<ProjectModel> activeProjects = getEntitiesResult.Value.Where(x => !x.IsHidden).ToList();

                _projectsSource.Edit(
                    innerCache =>
                    {
                        innerCache.Clear();
                        innerCache.AddOrUpdate(activeProjects);
                    }
                );
            }
        }

        private async Task InitializeTaskItems()
        {
            var getEntitiesParameters = new EntityServiceGetEntitiesParameters<TaskModel, int>
            {
                OrderByProperty = x => x.Id,
                OrderByDescending = true
            };

            OperationResult<List<TaskModel>> getEntitiesResult = await DoActionAsync(() => _taskService.GetEntities(getEntitiesParameters));

            if (getEntitiesResult.Success)
            {
                List<TaskModel> activeTasks = getEntitiesResult.Value.Where(x => !x.IsHidden).ToList();

                _tasksSource.Edit(
                    innerCache =>
                    {
                        innerCache.Clear();
                        innerCache.AddOrUpdate(activeTasks);
                    }
                );
            }
        }

        private async Task DoneCommandAction()
        {
            DateTime utcNow = DateTime.UtcNow.TrimToSeconds();
            var activity = new ActivityModel
            {
                CreatedDateTimeUtc = utcNow,
                IsHidden = false,
                Description = this.Description,
                StartDateTimeUtc = this.StartDateTimeUtc,
                EndDateTimeUtc = utcNow,
                Duration = utcNow - this.StartDateTimeUtc,
                Project = this.SelectedProject,
                Task = this.SelectedTask
            };

            OperationResult<int> createResult = await DoActionAsync(() => _activityService.Create(activity));
            if (createResult.Success)
            {
                this.StartDateTimeUtc = utcNow;
            }
            else
            {
                await ShowErrorDialog($"Failed to create activity: {createResult.Error.Message}");
            }
        }

        private void OpenActivitiesWindowAction()
        {
            if (_activitiesWindow != null)
            {
                _activitiesWindow.Activate();

                return;
            }

            var activitiesWindow = new ActivitiesWindowView { DataContext = new ActivitiesWindowViewModel(_serviceProvider) };

            activitiesWindow.ViewModel.ActivitiesChanged.Subscribe(activityChangeSet => Console.WriteLine(activityChangeSet.Count));
            activitiesWindow.Closing += ActivitiesWindowOnClosing;
            activitiesWindow.Show();

            _activitiesWindow = activitiesWindow;
        }

        private void ActivitiesWindowOnClosing(object sender, CancelEventArgs e)
        {
            _activitiesWindow = null;
        }

        private void OpenProjectsWindowAction()
        {
            if (_projectsWindow != null)
            {
                _projectsWindow.Activate();

                return;
            }

            var projectsWindow = new ProjectsWindowView { DataContext = new ProjectsWindowViewModel(_serviceProvider) };
            projectsWindow.ViewModel.ProjectsChanged.Subscribe(_ => Observable.StartAsync(InitializeProjectItems));
            projectsWindow.Closing += ProjectsWindowOnClosing;
            projectsWindow.Show();

            _projectsWindow = projectsWindow;
        }

        private void ProjectsWindowOnClosing(object sender, CancelEventArgs e)
        {
            _projectsWindow = null;
        }

        private void OpenTasksWindowAction()
        {
            if (_tasksWindow != null)
            {
                _tasksWindow.Activate();

                return;
            }

            var tasksWindow = new TasksWindowView { DataContext = new TasksWindowViewModel(_serviceProvider) };
            tasksWindow.ViewModel.TasksChanged.Subscribe(_ => Observable.StartAsync(InitializeTaskItems));
            tasksWindow.Closing += TasksWindowOnClosing;
            tasksWindow.Show();

            _tasksWindow = tasksWindow;
        }

        private void TasksWindowOnClosing(object sender, CancelEventArgs e)
        {
            _tasksWindow = null;
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
