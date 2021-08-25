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

using MyDailyActivity.Activities.ActivityEdit;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

using Services.Contracts.Activities;
using Services.Contracts.EntityServices;

namespace MyDailyActivity.Activities
{
    public class ActivitiesWindowViewModel : ReactiveWindowViewModelBase
    {
        internal class ViewListItem
        {
            public int Id { get; }

            public DateTime CreatedDateTime { get; }

            public DateTime? ModifiedDateTime { get; }

            public string Description { get; }

            public DateTime StartDateTime { get; }

            public DateTime EndDateTime { get; }

            public TimeSpan Duration { get; }

            public string ProjectName { get; set; }

            public string TaskName { get; set; }

            public bool IsHidden { get; }

            public ViewListItem(ActivityModel activity)
            {
                this.Id = activity.Id;
                this.CreatedDateTime = activity.CreatedDateTimeUtc.ToLocalTime();
                this.ModifiedDateTime = activity.ModifiedDateTimeUtc?.ToLocalTime();
                this.Description = activity.Description;
                this.StartDateTime = activity.StartDateTimeUtc.ToLocalTime();
                this.EndDateTime = activity.EndDateTimeUtc.ToLocalTime();
                this.Duration = activity.Duration;
                this.ProjectName = activity.Project?.Name;
                this.TaskName = activity.Task?.Name;
                this.IsHidden = activity.IsHidden;
            }
        }

        private readonly IActivityService _activityService;
        private readonly IServiceScope _serviceScope;
        private readonly SourceCache<ActivityModel, int> _activitiesSource = new SourceCache<ActivityModel, int>(x => x.Id);
        private readonly ReadOnlyObservableCollection<ViewListItem> _viewListItems;

        private IEnumerable<ViewListItem> ViewListItems => _viewListItems;

        [Reactive]
        private ViewListItem SelectedActivity { get; set; }

        private EditButtonsBarViewModel EditButtonsBarViewModel { get; set; }

        private BottomButtonsBarViewModel BottomButtonsBarViewModel { get; set; }

        public IObservable<IChangeSet<ActivityModel, int>> ActivitiesChanged { get; }

        internal ReactiveCommand<Unit, Unit> DataGridOnDoubleTapped { get; }

        [Reactive]
        internal List<ViewListItem> SelectedActivities { get; set; } = new List<ViewListItem>();

        public ActivitiesWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            _activityService = _serviceScope.ServiceProvider.GetRequiredService<IActivityService>();

            _activitiesSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new ViewListItem(x))
                .Sort(SortExpressionComparer<ViewListItem>.Descending(x => x.Id))
                .Bind(out _viewListItems)
                .DisposeMany()
                .Subscribe();

            CreateEditButtonsBar();
            CreateBottomButtonsBar();

            ConfigureBusyIndicator(this.BusyIndicatorViewModel);

            this.ActivitiesChanged = _activitiesSource.Connect().ObserveOn(RxApp.MainThreadScheduler);

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
            Observable.StartAsync(InitializeActivitiesSource);

            this.WhenAnyValue(x => x.SelectedActivities).Subscribe(_ => SelectedActivitiesChanged()).DisposeWith(disposables);
            this.WhenAnyValue(x => x.IsBusy).Subscribe(_ => UpdateEditButtonsState()).DisposeWith(disposables);
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

        private async Task InitializeActivitiesSource()
        {
            OperationResult<List<ActivityModel>> getEntitiesResult = await DoActionAsync(
                () =>
                {
                    var getEntitiesParameters = new EntityServiceGetEntitiesParameters<ActivityModel, int> { IncludeRelated = true };

                    return _activityService.GetEntities(getEntitiesParameters);
                }
            );

            if (!getEntitiesResult.Success)
            {
                return;
            }

            _activitiesSource.Edit(
                innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(getEntitiesResult.Value);
                }
            );

            this.SelectedActivity = this.ViewListItems.FirstOrDefault();
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

            bool hasSelectedActivity = this.SelectedActivity != null;
            bool hasSelectedActivities = this.SelectedActivities.Count > 1;

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
                this.EditButtonsBarViewModel.CanExecuteCopyCommand = hasSelectedActivity && !hasSelectedActivities;
                this.EditButtonsBarViewModel.CanExecuteEditCommand = hasSelectedActivity && !hasSelectedActivities;
                this.EditButtonsBarViewModel.CanExecuteDeleteCommand = hasSelectedActivity || hasSelectedActivities;
            }
        }

        private void SelectedActivitiesChanged()
        {
            UpdateEditButtonsState();
        }

        private async Task CreateActionAsync()
        {
            var newActivity = new ActivityModel { CreatedDateTimeUtc = DateTime.UtcNow };
            var activityEditView = new ActivityEditView { DataContext = new ActivityEditViewModel(newActivity) };
            await activityEditView.ShowDialog(this.OwnerWindow);

            if (!activityEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            newActivity = activityEditView.ViewModel.Model;
            OperationResult<int> createResult = await DoActionAsync(() => _activityService.Create(newActivity));

            if (!createResult.Success)
            {
                return;
            }

            OperationResult<ActivityModel> getActivityResult = await DoActionAsync(() => _activityService.GetEntity(createResult.Value));
            ActivityModel createdActivity = getActivityResult.GetValueOrThrow();

            _activitiesSource.AddOrUpdate(createdActivity);

            this.SelectedActivity = this.ViewListItems.First(x => x.Id == createdActivity.Id);
        }

        private async Task CopyActionAsync()
        {
            ActivityModel activityCopy = _activitiesSource.Items.First(x => x.Id == this.SelectedActivity.Id).CopyModelForCreate();
            var activityEditView = new ActivityEditView { DataContext = new ActivityEditViewModel(activityCopy) };
            await activityEditView.ShowDialog(this.OwnerWindow);

            if (!activityEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            ActivityModel modifiedActivity = activityEditView.ViewModel.Model;
            OperationResult<int> createResult = await DoActionAsync(() => _activityService.Create(modifiedActivity));

            if (!createResult.Success)
            {
                return;
            }

            OperationResult<ActivityModel> getActivityResult = await DoActionAsync(() => _activityService.GetEntity(createResult.Value));
            ActivityModel createdActivity = getActivityResult.GetValueOrThrow();

            _activitiesSource.AddOrUpdate(createdActivity);

            this.SelectedActivity = this.ViewListItems.First(x => x.Id == createdActivity.Id);
        }

        private async Task EditActionAsync()
        {
            ActivityModel activityCopy = _activitiesSource.Items.First(x => x.Id == this.SelectedActivity.Id).CopyModelForEdit();
            var activityEditView = new ActivityEditView { DataContext = new ActivityEditViewModel(activityCopy) };
            await activityEditView.ShowDialog(this.OwnerWindow);

            if (!activityEditView.ViewModel!.IsSuccess)
            {
                return;
            }

            ActivityModel modifiedActivity = activityEditView.ViewModel.Model;
            OperationResult updateResult = await DoActionAsync(() => _activityService.Update(modifiedActivity));

            if (!updateResult.Success)
            {
                return;
            }

            OperationResult<ActivityModel> getActivityResult =
                await DoActionAsync(() => _activityService.GetEntity(modifiedActivity.Id, includeRelated: true));

            ActivityModel updatedActivity = getActivityResult.GetValueOrThrow();

            _activitiesSource.AddOrUpdate(updatedActivity);

            this.SelectedActivity = this.ViewListItems.First(x => x.Id == updatedActivity.Id);
        }

        private async Task DeleteActionAsync()
        {
            ButtonResult confirmationDialogResult = await ShowConfirmationDialog(
                $"You're going to delete {this.SelectedActivities.Count} item(s).\nContinue?",
                "Delete action"
            );

            if (!ShowConfirmationDialogResultConfirmed(confirmationDialogResult))
            {
                Console.WriteLine("No");

                return;
            }

            List<int> selectedActivityIds = this.SelectedActivities.ConvertAll(x => x.Id);
            OperationResult deleteRangeResult = await DoActionAsync(() => _activityService.DeleteRange(selectedActivityIds));

            if (deleteRangeResult.Success)
            {
                _activitiesSource.RemoveKeys(selectedActivityIds);
            }
            else
            {
                string failedIds = string.Join(",", selectedActivityIds);
                await ShowErrorDialog($"Fail to delete: {failedIds}.\n{deleteRangeResult.Error.Message}", "Delete action");
            }
        }

        private void CloseAction()
        {
            CloseView();
        }
    }
}
