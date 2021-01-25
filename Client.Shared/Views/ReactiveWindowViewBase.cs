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
            this.ViewModel.OwnerWindow = (Window)this.VisualRoot;

            this.WhenAnyObservable(x => x.ViewModel.ViewCloseCommand)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => Close())
                .DisposeWith(disposables);

            InitializeEvents(disposables);

            HandleActivationCore(disposables);
        }

        private void InitializeEvents(CompositeDisposable disposables)
        {
            this.Events().Closing.Subscribe(async pattern => await this.ViewModel.OnClosingAsync(pattern)).DisposeWith(disposables);
            this.Events().Closing.Subscribe(pattern => this.ViewModel.OnClosing(pattern)).DisposeWith(disposables);
        }
    }
}
