using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.ReactiveUI;

using Client.Shared.ViewModels;

using ReactiveUI;

namespace Client.Shared.Views
{
    public abstract class ReactiveWindowViewBase<TViewModel> : ReactiveWindow<TViewModel>
        where TViewModel : ReactiveWindowViewModelBase
    {
        protected ReactiveWindowViewBase()
        {
            this.WhenActivated(HandleActivation);
        }

        protected virtual void HandleActivationCore(CompositeDisposable disposables)
        {
        }

        private void HandleActivation(CompositeDisposable disposables)
        {
            if (this.ViewModel == null)
            {
                throw new Exception($"{nameof(this.ViewModel)} is null.");
            }

            this.ViewModel.OwnerWindow = (Window)this.VisualRoot;

            this.WhenAnyObservable(x => x.ViewModel.ViewCloseCommand)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => Close())
                .DisposeWith(disposables);

            InitializeEvents(this.ViewModel, disposables);

            HandleActivationCore(disposables);
        }

        private void InitializeEvents(TViewModel viewModel, CompositeDisposable disposables)
        {
            this.Events().Closing.Subscribe(async pattern => await viewModel.OnClosingAsync(pattern)).DisposeWith(disposables);
            this.Events().Closing.Subscribe(viewModel.OnClosing).DisposeWith(disposables);
        }
    }
}
