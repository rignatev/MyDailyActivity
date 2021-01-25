using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
        public class ViewListItem
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

        private readonly ITaskService _taskService;
        private readonly SourceCache<TaskModel, int> _tasksSource = new(x => x.Id);
        private ReadOnlyObservableCollection<ViewListItem> _viewListItems;

        public IEnumerable<ViewListItem> ViewListItems => _viewListItems;

        public bool TasksChanged { get; set; }

        [Reactive]
        public ViewListItem SelectedTask { get; set; }

        [Reactive]
        public List<ViewListItem> SelectedTasks { get; set; } =
            new();

        public EditButtonsBarViewModel EditButtonsBarViewModel { get; private set; }

        public BottomButtonsBarViewModel BottomButtonsBarViewModel { get; private set; }

        public TasksWindowViewModel(IServiceProvider serviceProvider)
        {
            _taskService = serviceProvider.GetRequiredService<ITaskService>();

            InitializeTasksSource();
            InitializeEditButtonsBar();
            InitializeBottomButtonsBar();
        }

        /// <inheritdoc />
        protected override void HandleActivation(CompositeDisposable disposables)
        {
            this.WhenAnyValue(x => x.SelectedTasks).Subscribe(_ => SelectedTasksChanged());

            this.SelectedTask = this.ViewListItems.FirstOrDefault();
        }

        private void InitializeTasksSource()
        {
            OperationResult<List<TaskModel>> getEntitiesResult =
                _taskService.GetEntities(new EntityServiceGetEntitiesParameters<TaskModel, int>());

            if (!getEntitiesResult.Success)
            {
                return;
            }

            _tasksSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new ViewListItem(x))
                .Sort(SortExpressionComparer<ViewListItem>.Descending(x => x.Id))
                .Bind(out _viewListItems)
                .DisposeMany()
                .Subscribe();

            _tasksSource.Edit(
                innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(getEntitiesResult.Value);
                }
            );
        }

        private void InitializeEditButtonsBar()
        {
            this.EditButtonsBarViewModel = new EditButtonsBarViewModel();

            this.EditButtonsBarViewModel.CreateCommand.Subscribe(async unit => await CreateActionAsync());
            this.EditButtonsBarViewModel.CopyCommand.Subscribe(async unit => await CopyActionAsync());
            this.EditButtonsBarViewModel.EditCommand.Subscribe(async unit => await EditActionAsync());
            this.EditButtonsBarViewModel.DeleteCommand.Subscribe(async unit => await DeleteActionAsync());

            UpdateEditButtonsState();
        }

        private void InitializeBottomButtonsBar()
        {
            this.BottomButtonsBarViewModel = new BottomButtonsBarViewModel(BottomButtonsBarViewModel.BarType.Close);

            this.BottomButtonsBarViewModel.CloseCommand.Subscribe(_ => CloseAction());
        }

        private void UpdateEditButtonsState()
        {
            bool hasSelectedTask = this.SelectedTask != null;
            bool hasSelectedTasks = this.SelectedTasks.Count > 1;

            this.EditButtonsBarViewModel.CanExecuteCopyCommand = hasSelectedTask && !hasSelectedTasks;
            this.EditButtonsBarViewModel.CanExecuteEditCommand = hasSelectedTask && !hasSelectedTasks;
            this.EditButtonsBarViewModel.CanExecuteDeleteCommand = hasSelectedTask || hasSelectedTasks;
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

            if (!taskEditView.ViewModel.IsSuccess)
            {
                return;
            }

            newTask = taskEditView.ViewModel.Model;
            OperationResult<int> createResult = _taskService.Create(newTask);

            if (!createResult.Success)
            {
                return;
            }

            newTask.Id = createResult.Value;

            _tasksSource.AddOrUpdate(newTask);

            this.TasksChanged = true;
            this.SelectedTask = this.ViewListItems.First(x => x.Id == newTask.Id);
        }

        private async Task CopyActionAsync()
        {
            TaskModel taskCopy = _tasksSource.Items.First(x => x.Id == this.SelectedTask.Id).CopyModelForCreate();
            var taskEditView = new TaskEditView { DataContext = new TaskEditViewModel(taskCopy) };
            await taskEditView.ShowDialog(this.OwnerWindow);

            if (!taskEditView.ViewModel.IsSuccess)
            {
                return;
            }

            TaskModel modifiedTask = taskEditView.ViewModel.Model;
            OperationResult<int> createResult = _taskService.Create(modifiedTask);

            if (!createResult.Success)
            {
                return;
            }
            modifiedTask.Id = createResult.Value;

            _tasksSource.AddOrUpdate(modifiedTask);

            this.TasksChanged = true;
            this.SelectedTask = this.ViewListItems.First(x => x.Id == modifiedTask.Id);
        }

        private async Task EditActionAsync()
        {
            TaskModel taskCopy = _tasksSource.Items.First(x => x.Id == this.SelectedTask.Id).CopyModelForEdit();
            var taskEditView = new TaskEditView { DataContext = new TaskEditViewModel(taskCopy) };
            await taskEditView.ShowDialog(this.OwnerWindow);

            if (!taskEditView.ViewModel.IsSuccess)
            {
                return;
            }

            TaskModel modifiedTask = taskEditView.ViewModel.Model;
            OperationResult updateResult = _taskService.Update(modifiedTask);

            if (!updateResult.Success)
            {
                return;
            }

            _tasksSource.AddOrUpdate(modifiedTask);

            this.TasksChanged = true;
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

            foreach (ViewListItem selectedTask in this.SelectedTasks)
            {
                OperationResult deleteResult = _taskService.Delete(selectedTask.Id);

                if (deleteResult.Success)
                {
                    _tasksSource.RemoveKey(selectedTask.Id);

                    this.TasksChanged = true;
                }
                else
                {
                    await ShowErrorDialog(
                        $"Fail to delete task: {selectedTask.Name}.\n{deleteResult.Error.Message}",
                        "Delete action"
                    );
                }
            }
        }

        private void CloseAction()
        {
            CloseView();
        }
    }
}
