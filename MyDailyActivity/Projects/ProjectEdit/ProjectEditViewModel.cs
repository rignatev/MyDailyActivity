using System;
using System.Reactive.Linq;

using Client.Shared.Controls.ButtonsBars;
using Client.Shared.ViewModels;

using Contracts.Shared.Models;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MyDailyActivity.Projects.ProjectEdit
{
    public class ProjectEditViewModel : ReactiveWindowViewModelBase
    {
        [Reactive]
        private bool IsHidden { get; set; }

        [Reactive]
        private string Name { get; set; }

        [Reactive]
        private string Description { get; set; }

        private IObservable<bool> ItemChanged { get; }

        private BottomButtonsBarViewModel BottomButtonsBarViewModel { get; set; }

        public ProjectModel Model { get; }

        public bool IsSuccess { get; private set; }

        /// <inheritdoc />
        public ProjectEditViewModel(ProjectModel project)
        {
            this.Model = project;

            CopyFromModel();

            this.ItemChanged = this.WhenAnyValue(
                    x => x.Name,
                    x => x.Description,
                    x => x.IsHidden,
                    (name, description, isHidden) =>
                        name != this.Model.Name ||
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
            this.Model.Name = this.Name;
            this.Model.Description = this.Description;
            this.Model.IsHidden = this.IsHidden;
        }

        private void CopyFromModel()
        {
            this.Name = this.Model.Name;
            this.Description = this.Model.Description;
            this.IsHidden = this.Model.IsHidden;
        }

        private void CancelAction()
        {
            CloseView();
        }
    }
}
