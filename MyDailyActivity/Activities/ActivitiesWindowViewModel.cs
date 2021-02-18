using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Client.Shared.Controls.ButtonsBars;
using Client.Shared.ViewModels;

using Contracts.Shared.Models;

using DynamicData;
using DynamicData.Binding;

using Infrastructure.Shared.OperationResult;
using Infrastructure.Shared.Utils;

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
                this.IsHidden = activity.IsHidden;
            }
        }

        private readonly IActivityService _activityService;
        private readonly IServiceScope _serviceScope;
        private readonly SourceCache<ActivityModel, int> _activitiesSource = new(x => x.Id);
        private ReadOnlyObservableCollection<ViewListItem> _viewListItems;

        private IEnumerable<ViewListItem> ViewListItems => _viewListItems;

        [Reactive]
        private ViewListItem SelectedActivity { get; set; }

        private EditButtonsBarViewModel EditButtonsBarViewModel { get; set; }

        private BottomButtonsBarViewModel BottomButtonsBarViewModel { get; set; }

        public IObservable<IChangeSet<ActivityModel, int>> ActivitiesChanged { get; }

        internal ReactiveCommand<Unit, Unit> DataGridOnDoubleTapped { get; }

        [Reactive]
        internal List<ViewListItem> SelectedActivities { get; set; } =
            new();

        public ActivitiesWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceScope = serviceProvider.CreateScope();
            _activityService = _serviceScope.ServiceProvider.GetRequiredService<IActivityService>();

            InitializeActivitiesSource();
            InitializeEditButtonsBar();
            InitializeBottomButtonsBar();

            this.ActivitiesChanged = _activitiesSource.Connect().ObserveOn(RxApp.MainThreadScheduler);

            this.DataGridOnDoubleTapped = ReactiveCommand.Create<Unit>(async _ => await EditActionAsync());
        }

        /// <inheritdoc />
        protected override void HandleActivation(CompositeDisposable disposables)
        {
            this.WhenAnyValue(x => x.SelectedActivities).Subscribe(_ => SelectedActivitiesChanged()).DisposeWith(disposables);

            this.SelectedActivity = this.ViewListItems.FirstOrDefault();
        }

        /// <inheritdoc />
        protected override void HandleDeactivation(CompositeDisposable disposables)
        {
            _serviceScope.DisposeWith(disposables);
        }

        private void InitializeActivitiesSource()
        {
            OperationResult<List<ActivityModel>> getEntitiesResult =
                _activityService.GetEntities(new EntityServiceGetEntitiesParameters<ActivityModel, int>());

            if (!getEntitiesResult.Success)
            {
                return;
            }

            _activitiesSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Transform(x => new ViewListItem(x))
                .Sort(SortExpressionComparer<ViewListItem>.Descending(x => x.Id))
                .Bind(out _viewListItems)
                .DisposeMany()
                .Subscribe();

            _activitiesSource.Edit(
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

            this.EditButtonsBarViewModel.CreateCommand.Subscribe(async _ => await CreateActionAsync());
            this.EditButtonsBarViewModel.CopyCommand.Subscribe(async _ => await CopyActionAsync());
            this.EditButtonsBarViewModel.EditCommand.Subscribe(async _ => await EditActionAsync());
            this.EditButtonsBarViewModel.DeleteCommand.Subscribe(async _ => await DeleteActionAsync());

            UpdateEditButtonsState();
        }

        private void InitializeBottomButtonsBar()
        {
            this.BottomButtonsBarViewModel = new BottomButtonsBarViewModel(BottomButtonsBarViewModel.BarType.Close);

            this.BottomButtonsBarViewModel.CloseCommand.Subscribe(_ => CloseAction());
        }

        private void UpdateEditButtonsState()
        {
            bool hasSelectedActivity = this.SelectedActivity != null;
            bool hasSelectedActivities = this.SelectedActivities.Count > 1;

            this.EditButtonsBarViewModel.CanExecuteCopyCommand = hasSelectedActivity && !hasSelectedActivities;
            this.EditButtonsBarViewModel.CanExecuteEditCommand = hasSelectedActivity && !hasSelectedActivities;
            this.EditButtonsBarViewModel.CanExecuteDeleteCommand = hasSelectedActivity || hasSelectedActivities;
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

            if (!activityEditView.ViewModel.IsSuccess)
            {
                return;
            }

            newActivity = activityEditView.ViewModel.Model;
            OperationResult<int> createResult = _activityService.Create(newActivity);

            if (!createResult.Success)
            {
                return;
            }

            newActivity.Id = createResult.Value;

            _activitiesSource.AddOrUpdate(newActivity);

            this.SelectedActivity = this.ViewListItems.First(x => x.Id == newActivity.Id);
        }

        private async Task CopyActionAsync()
        {
            ActivityModel activityCopy = _activitiesSource.Items.First(x => x.Id == this.SelectedActivity.Id).CopyModelForCreate();
            var activityEditView = new ActivityEditView { DataContext = new ActivityEditViewModel(activityCopy) };
            await activityEditView.ShowDialog(this.OwnerWindow);

            if (!activityEditView.ViewModel.IsSuccess)
            {
                return;
            }

            ActivityModel modifiedActivity = activityEditView.ViewModel.Model;
            OperationResult<int> createResult = _activityService.Create(modifiedActivity);

            if (!createResult.Success)
            {
                return;
            }

            modifiedActivity.Id = createResult.Value;

            _activitiesSource.AddOrUpdate(modifiedActivity);

            this.SelectedActivity = this.ViewListItems.First(x => x.Id == modifiedActivity.Id);
        }

        private async Task EditActionAsync()
        {
            ActivityModel activityCopy = _activitiesSource.Items.First(x => x.Id == this.SelectedActivity.Id).CopyModelForEdit();
            var activityEditView = new ActivityEditView { DataContext = new ActivityEditViewModel(activityCopy) };
            await activityEditView.ShowDialog(this.OwnerWindow);

            if (!activityEditView.ViewModel.IsSuccess)
            {
                return;
            }

            ActivityModel modifiedActivity = activityEditView.ViewModel.Model;
            OperationResult updateResult = _activityService.Update(modifiedActivity);

            if (!updateResult.Success)
            {
                return;
            }

            _activitiesSource.AddOrUpdate(modifiedActivity);

            this.SelectedActivity = this.ViewListItems.First(x => x.Id == modifiedActivity.Id);
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

            var deletedIds = new List<int>(this.SelectedActivities.Count);

            foreach (ViewListItem selectedActivity in this.SelectedActivities)
            {
                OperationResult deleteResult = _activityService.Delete(selectedActivity.Id);

                if (deleteResult.Success)
                {
                    deletedIds.Add(selectedActivity.Id);
                }
                else
                {
                    await ShowErrorDialog(
                        $"Fail to delete: {selectedActivity.Id}.\n{deleteResult.Error.Message}",
                        "Delete action"
                    );
                }
            }

            if (!deletedIds.IsNullOrEmpty())
            {
                _activitiesSource.RemoveKeys(deletedIds);
            }
        }

        private void CloseAction()
        {
            CloseView();
        }
    }
}
