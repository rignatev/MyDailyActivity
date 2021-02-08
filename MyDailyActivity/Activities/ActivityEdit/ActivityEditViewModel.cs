using System;
using System.Reactive.Linq;

using Client.Shared.Controls.ButtonsBars;
using Client.Shared.ViewModels;

using Contracts.Shared.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MyDailyActivity.Activities.ActivityEdit
{
    public class ActivityEditViewModel : ReactiveWindowViewModelBase
    {
        [Reactive]
        private bool IsHidden { get; set; }

        [Reactive]
        private string Description { get; set; }

        private IObservable<bool> ItemChanged { get; }

        private BottomButtonsBarViewModel BottomButtonsBarViewModel { get; set; }

        public ActivityModel Model { get; }

        public bool IsSuccess { get; private set; }

        /// <inheritdoc />
        public ActivityEditViewModel(ActivityModel activity)
        {
            this.Model = activity;

            CopyFromModel();

            this.ItemChanged = this.WhenAnyValue(
                    x => x.Description,
                    x => x.IsHidden,
                    (description, isHidden) =>
                        description != this.Model.Description ||
                        isHidden != this.Model.IsHidden
                )
                .Throttle(TimeSpan.FromSeconds(value: 0.25))
                .DistinctUntilChanged();

            InitializeBottomButtonsBar();
        }

        private void InitializeBottomButtonsBar()
        {
            this.BottomButtonsBarViewModel = new BottomButtonsBarViewModel(BottomButtonsBarViewModel.BarType.OkCancel);

            this.BottomButtonsBarViewModel.OkCommand
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => OkAction());

            this.BottomButtonsBarViewModel.CancelCommand
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => CancelAction());

            this.ItemChanged
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => this.BottomButtonsBarViewModel.CanExecuteOkCommand = x);
        }

        private void OkAction()
        {
            CopyToModel();

            this.IsSuccess = true;

            CloseView();
        }

        private void CopyToModel()
        {
            this.Model.Description = this.Description;
            this.Model.IsHidden = this.IsHidden;
        }

        private void CopyFromModel()
        {
            this.Description = this.Model.Description;
            this.IsHidden = this.Model.IsHidden;
        }

        private void CancelAction()
        {
            CloseView();
        }
    }
}
