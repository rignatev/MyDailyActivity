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

using MyDailyActivity.Tasks.TaskEdit;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Services.Contracts.EntityServices;
using Services.Contracts.Tasks;

namespace MyDailyActivity.Tasks
{
    public class TasksWindowViewModel : ReactiveWindowViewModelBase
    {
        internal class ViewListItem
        {
            public int Id { get; }

            public DateTime CreatedDateTime { get; }

            public DateTime? ModifiedDateTime { get; }

            public string Name { get; }

            public string Description { get; }

            public bool IsHidden { get; }

            public ViewListItem(TaskModel task)
            {
                this.Id = task.Id;
                this.CreatedDateTime = task.CreatedDateTimeUtc.ToLocalTime();
                this.ModifiedDateTime = task.ModifiedDateTimeUtc?.ToLocalTime();
                this.Name = task.Name;
                this.Description = task.Description;
                this.IsHidden = task.IsHidden;
            }
        }

        private readonly IServiceScope _serviceScope;
        private readonly ITaskService _taskService;
        private readonly SourceCache<TaskModel, int> _tasksSource = new SourceCache<TaskModel, int>(x => x.Id);
        private readonly ReadOnlyObservableCollection<ViewListItem> _viewListItems;

        private IEnumerable<ViewListItem> ViewListItems => _viewListItems;

        [Reactive]
        private ViewListItem SelectedTask { get; set; }

        private EditButtonsBarViewModel EditButtonsBarViewModel { get; set; }

        private BottomButtonsBarViewModel BottomButtonsBarViewModel { get; set; }

        public IObservable<IChangeSet<TaskModel, int>> TasksChanged { get; }

        internal ReactiveCommand<Unit, Unit> DataGridOnDoubleTapped { get; }

        [Reactive]
        internal List<ViewListItem> SelectedTasks { get; set; } = new List<ViewListItem>();

        public TasksWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            _taskService = _serviceScope.ServiceProvider.GetRequiredService<ITaskService>();

            _tasksSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new ViewListItem(x))
                .Sort(SortExpressionComparer<ViewListItem>.Descending(x => x.Id))
                .Bind(out _viewListItems)
                .DisposeMany()
                .Subscribe();

            CreateEditButtonsBar();
            CreateBottomButtonsBar();

            ConfigureBusyIndicator(this.BusyIndicatorViewModel);

            this.TasksChanged = _tasksSource.Connect().ObserveOn(RxApp.MainThreadScheduler);

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
            Observable.StartAsync(InitializeTasksSource);

            this.WhenAnyValue(x => x.SelectedTasks).Subscribe(_ => SelectedTasksChanged()).DisposeWith(disposables);
        }

        /// <inheritdoc />
        protected override void HandleDeactivation(CompositeDisposable disposables)
        {
            _serviceScope.DisposeWith(disposables);
        }

        /// <inheritdoc />
        protected override void IsBusyChanged(bool isBusy)
        {
            UpdateEditButtonsState();
        }

        static private void ConfigureBusyIndicator(BusyIndicatorViewModel busyIndicatorViewModel)
        {
            busyIndicatorViewModel.Text = "Wait...";
            busyIndicatorViewModel.Foreground = new SolidColorBrush(Colors.White);
        }

        private async Task InitializeTasksSource()
        {
            OperationResult<List<TaskModel>> getEntitiesResult = await DoActionAsync(
                () => _taskService.GetEntities(new EntityServiceGetEntitiesParameters<TaskModel, int>())
            );

            if (!getEntitiesResult.Success)
            {
                return;
            }

            _tasksSource.Edit(
                innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(getEntitiesResult.Value);
                }
            );

            this.SelectedTask = this.ViewListItems.FirstOrDefault();
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
            if (this.EditButtonsBarViewModel == null)
            {
                return;
            }

            bool hasSelectedTask = this.SelectedTask != null;
            bool hasSelectedTasks = this.SelectedTasks.Count > 1;

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
                this.EditButtonsBarViewModel.CanExecuteCopyCommand = hasSelectedTask && !hasSelectedTasks;
                this.EditButtonsBarViewModel.CanExecuteEditCommand = hasSelectedTask && !hasSelectedTasks;
                this.EditButtonsBarViewModel.CanExecuteDeleteCommand = hasSelectedTask || hasSelectedTasks;
            }
        }

        private void SelectedTasksChanged()
        {
            UpdateEditButtonsState();
        }

        private async Task CreateActionAsync()
        {
            var newTask = new TaskModel { CreatedDateTimeUtc = DateTime.UtcNow };
            var taskEditView = new TaskEditView { DataContext = new TaskEditViewModel(newTask) };
            await taskEditView.ShowDialog(this.OwnerWindow);

            if (!taskEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            newTask = taskEditView.ViewModel.Model;
            OperationResult<int> createResult = await DoActionAsync(() => _taskService.Create(newTask));

            if (!createResult.Success)
            {
                return;
            }

            newTask.Id = createResult.Value;

            _tasksSource.AddOrUpdate(newTask);

            this.SelectedTask = this.ViewListItems.First(x => x.Id == newTask.Id);
        }

        private async Task CopyActionAsync()
        {
            TaskModel taskCopy = _tasksSource.Items.First(x => x.Id == this.SelectedTask.Id).CopyModelForCreate();
            var taskEditView = new TaskEditView { DataContext = new TaskEditViewModel(taskCopy) };
            await taskEditView.ShowDialog(this.OwnerWindow);

            if (!taskEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            TaskModel modifiedTask = taskEditView.ViewModel.Model;
            OperationResult<int> createResult = await DoActionAsync(() => _taskService.Create(modifiedTask));

            if (!createResult.Success)
            {
                return;
            }

            modifiedTask.Id = createResult.Value;

            _tasksSource.AddOrUpdate(modifiedTask);

            this.SelectedTask = this.ViewListItems.First(x => x.Id == modifiedTask.Id);
        }

        private async Task EditActionAsync()
        {
            TaskModel taskCopy = _tasksSource.Items.First(x => x.Id == this.SelectedTask.Id).CopyModelForEdit();
            var taskEditView = new TaskEditView { DataContext = new TaskEditViewModel(taskCopy) };
            await taskEditView.ShowDialog(this.OwnerWindow);

            if (!taskEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            TaskModel modifiedTask = taskEditView.ViewModel.Model;
            OperationResult updateResult = await DoActionAsync(() => _taskService.Update(modifiedTask));

            if (!updateResult.Success)
            {
                return;
            }

            _tasksSource.AddOrUpdate(modifiedTask);

            this.SelectedTask = this.ViewListItems.First(x => x.Id == modifiedTask.Id);
        }

        private async Task DeleteActionAsync()
        {
            ButtonResult confirmationDialogResult = await ShowConfirmationDialog(
                $"You're going to delete {this.SelectedTasks.Count} item(s).\nContinue?",
                "Delete action"
            );

            if (!ShowConfirmationDialogResultConfirmed(confirmationDialogResult))
            {
                Console.WriteLine("No");

                return;
            }

            List<int> selectedTaskIds = this.SelectedTasks.ConvertAll(x => x.Id);
            OperationResult deleteRangeResult = _taskService.DeleteRange(selectedTaskIds);

            if (deleteRangeResult.Success)
            {
                _tasksSource.RemoveKeys(selectedTaskIds);
            }
            else
            {
                string failedIds = string.Join(",", selectedTaskIds);
                await ShowErrorDialog($"Fail to delete: {failedIds}.\n{deleteRangeResult.Error.Message}", "Delete action");
            }
        }

        private void CloseAction()
        {
            CloseView();
        }
    }
}
