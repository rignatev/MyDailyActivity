using System.Reactive.Disposables;

using Avalonia.Controls;
using Avalonia.ReactiveUI;

using Client.Shared.ViewModels;

using ReactiveUI;

namespace Client.Shared.Views
{
    public abstract class ReactiveUserControlViewBase<TViewModel> : ReactiveUserControl<TViewModel>
        where TViewModel : ReactiveUserControlViewModelBase
    {
        protected ReactiveUserControlViewBase()
        {
            this.WhenActivated(HandleActivation);
        }

        protected virtual void HandleActivationCore(CompositeDisposable disposables)
        {
        }

        private void HandleActivation(CompositeDisposable disposables)
        {
            this.ViewModel.OwnerWindow = (Window)this.VisualRoot;

            HandleActivationCore(disposables);
        }
    }
}
