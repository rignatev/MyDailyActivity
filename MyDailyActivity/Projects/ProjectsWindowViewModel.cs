using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Avalonia.Media;

using Client.Shared.Controls.BusyIndicator;
using Client.Shared.Controls.ButtonsBars;
using Client.Shared.ViewModels;

using Contracts.Shared.Models;

using DynamicData;
using DynamicData.Binding;

using Infrastructure.Shared.OperationResult;

using MessageBox.Avalonia.Enums;

using Microsoft.Extensions.DependencyInjection;

using MyDailyActivity.Projects.ProjectEdit;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Services.Contracts.EntityServices;
using Services.Contracts.Projects;

namespace MyDailyActivity.Projects
{
    public class ProjectsWindowViewModel : ReactiveWindowViewModelBase
    {
        internal class ViewListItem
        {
            public int Id { get; }

            public DateTime CreatedDateTime { get; }

            public DateTime? ModifiedDateTime { get; }

            public string Name { get; }

            public string Description { get; }

            public bool IsHidden { get; }

            public ViewListItem(ProjectModel project)
            {
                this.Id = project.Id;
                this.CreatedDateTime = project.CreatedDateTimeUtc.ToLocalTime();
                this.ModifiedDateTime = project.ModifiedDateTimeUtc?.ToLocalTime();
                this.Name = project.Name;
                this.Description = project.Description;
                this.IsHidden = project.IsHidden;
            }
        }

        private readonly IServiceScope _serviceScope;
        private readonly IProjectService _projectService;
        private readonly SourceCache<ProjectModel, int> _projectsSource = new SourceCache<ProjectModel, int>(x => x.Id);
        private readonly ReadOnlyObservableCollection<ViewListItem> _viewListItems;

        private IEnumerable<ViewListItem> ViewListItems => _viewListItems;

        [Reactive]
        private ViewListItem SelectedProject { get; set; }

        private EditButtonsBarViewModel EditButtonsBarViewModel { get; set; }

        private BottomButtonsBarViewModel BottomButtonsBarViewModel { get; set; }

        public IObservable<IChangeSet<ProjectModel, int>> ProjectsChanged { get; }

        internal ReactiveCommand<Unit, Unit> DataGridOnDoubleTapped { get; }

        [Reactive]
        internal List<ViewListItem> SelectedProjects { get; set; } = new List<ViewListItem>();

        public ProjectsWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            _projectService = _serviceScope.ServiceProvider.GetRequiredService<IProjectService>();

            _projectsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new ViewListItem(x))
                .Sort(SortExpressionComparer<ViewListItem>.Descending(x => x.Id))
                .Bind(out _viewListItems)
                .DisposeMany()
                .Subscribe();

            CreateEditButtonsBar();
            CreateBottomButtonsBar();

            ConfigureBusyIndicator(this.BusyIndicatorViewModel);

            this.ProjectsChanged = _projectsSource.Connect().ObserveOn(RxApp.MainThreadScheduler);

            this.DataGridOnDoubleTapped = ReactiveCommand.Create<Unit>(
                async _ =>
                {
                    if (this.EditButtonsBarViewModel.CanExecuteEditCommand)
                    {
                        await EditActionAsync();
                    }
                }
            );
        }

        /// <inheritdoc />
        protected override void HandleActivation(CompositeDisposable disposables)
        {
            Observable.StartAsync(InitializeProjectsSource);

            this.WhenAnyValue(x => x.SelectedProjects).Subscribe(_ => SelectedProjectsChanged()).DisposeWith(disposables);
        }

        /// <inheritdoc />
        protected override void HandleDeactivation(CompositeDisposable disposables)
        {
            _serviceScope.DisposeWith(disposables);
        }

        static private void ConfigureBusyIndicator(BusyIndicatorViewModel busyIndicatorViewModel)
        {
            busyIndicatorViewModel.Text = "Wait...";
            busyIndicatorViewModel.Foreground = new SolidColorBrush(Colors.White);
        }

        private async Task InitializeProjectsSource()
        {
            OperationResult<List<ProjectModel>> getEntitiesResult = await DoActionAsync(
                () => _projectService.GetEntities(new EntityServiceGetEntitiesParameters<ProjectModel, int>())
            );

            if (!getEntitiesResult.Success)
            {
                return;
            }

            _projectsSource.Edit(
                innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(getEntitiesResult.Value);
                }
            );

            this.SelectedProject = this.ViewListItems.FirstOrDefault();
        }

        private void CreateEditButtonsBar()
        {
            this.EditButtonsBarViewModel = new EditButtonsBarViewModel();

            this.EditButtonsBarViewModel.CreateCommand.Subscribe(async _ => await CreateActionAsync());
            this.EditButtonsBarViewModel.CopyCommand.Subscribe(async _ => await CopyActionAsync());
            this.EditButtonsBarViewModel.EditCommand.Subscribe(async _ => await EditActionAsync());
            this.EditButtonsBarViewModel.DeleteCommand.Subscribe(async _ => await DeleteActionAsync());

            UpdateEditButtonsState();
        }

        private void CreateBottomButtonsBar()
        {
            this.BottomButtonsBarViewModel = new BottomButtonsBarViewModel(BottomButtonsBarViewModel.BarType.Close);

            this.BottomButtonsBarViewModel.CloseCommand.Subscribe(_ => CloseAction());
        }

        private void UpdateEditButtonsState()
        {
            bool hasSelectedProject = this.SelectedProject != null;
            bool hasSelectedProjects = this.SelectedProjects.Count > 1;

            if (this.IsBusy)
            {
                this.EditButtonsBarViewModel.CanExecuteCreateCommand = false;
                this.EditButtonsBarViewModel.CanExecuteCopyCommand = false;
                this.EditButtonsBarViewModel.CanExecuteEditCommand = false;
                this.EditButtonsBarViewModel.CanExecuteDeleteCommand = false;
            }
            else
            {
                this.EditButtonsBarViewModel.CanExecuteCreateCommand = true;
                this.EditButtonsBarViewModel.CanExecuteCopyCommand = hasSelectedProject && !hasSelectedProjects;
                this.EditButtonsBarViewModel.CanExecuteEditCommand = hasSelectedProject && !hasSelectedProjects;
                this.EditButtonsBarViewModel.CanExecuteDeleteCommand = hasSelectedProject || hasSelectedProjects;
            }
        }

        private void SelectedProjectsChanged()
        {
            UpdateEditButtonsState();
        }

        private async Task CreateActionAsync()
        {
            var newProject = new ProjectModel { CreatedDateTimeUtc = DateTime.UtcNow };
            var projectEditView = new ProjectEditView { DataContext = new ProjectEditViewModel(newProject) };
            await projectEditView.ShowDialog(this.OwnerWindow);

            if (!projectEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            newProject = projectEditView.ViewModel.Model;
            OperationResult<int> createResult = await DoActionAsync(() => _projectService.Create(newProject));

            if (!createResult.Success)
            {
                return;
            }

            newProject.Id = createResult.Value;

            _projectsSource.AddOrUpdate(newProject);

            this.SelectedProject = this.ViewListItems.First(x => x.Id == newProject.Id);
        }

        private async Task CopyActionAsync()
        {
            ProjectModel projectCopy = _projectsSource.Items.First(x => x.Id == this.SelectedProject.Id).CopyModelForCreate();
            var projectEditView = new ProjectEditView { DataContext = new ProjectEditViewModel(projectCopy) };
            await projectEditView.ShowDialog(this.OwnerWindow);

            if (!projectEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            ProjectModel modifiedProject = projectEditView.ViewModel.Model;
            OperationResult<int> createResult = await DoActionAsync(() => _projectService.Create(modifiedProject));

            if (!createResult.Success)
            {
                return;
            }

            modifiedProject.Id = createResult.Value;

            _projectsSource.AddOrUpdate(modifiedProject);

            this.SelectedProject = this.ViewListItems.First(x => x.Id == modifiedProject.Id);
        }

        private async Task EditActionAsync()
        {
            ProjectModel projectCopy = _projectsSource.Items.First(x => x.Id == this.SelectedProject.Id).CopyModelForEdit();
            var projectEditView = new ProjectEditView { DataContext = new ProjectEditViewModel(projectCopy) };
            await projectEditView.ShowDialog(this.OwnerWindow);

            if (!projectEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            ProjectModel modifiedProject = projectEditView.ViewModel.Model;
            OperationResult updateResult = await DoActionAsync(() => _projectService.Update(modifiedProject));

            if (!updateResult.Success)
            {
                return;
            }

            _projectsSource.AddOrUpdate(modifiedProject);

            this.SelectedProject = this.ViewListItems.First(x => x.Id == modifiedProject.Id);
        }

        private async Task DeleteActionAsync()
        {
            ButtonResult confirmationDialogResult = await ShowConfirmationDialog(
                $"You're going to delete {this.SelectedProjects.Count} item(s).\nContinue?",
                "Delete action"
            );

            if (!ShowConfirmationDialogResultConfirmed(confirmationDialogResult))
            {
                Console.WriteLine("No");

                return;
            }

            List<int> selectedProjectIds = this.SelectedProjects.ConvertAll(x => x.Id);
            OperationResult deleteRangeResult = _projectService.DeleteRange(selectedProjectIds);

            if (deleteRangeResult.Success)
            {
                _projectsSource.RemoveKeys(selectedProjectIds);
            }
            else
            {
                string failedIds = string.Join(",", selectedProjectIds);
                await ShowErrorDialog($"Fail to delete: {failedIds}.\n{deleteRangeResult.Error.Message}", "Delete action");
            }
        }

        private void CloseAction()
        {
            CloseView();
        }
    }
}
