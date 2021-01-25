using System;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;

using ReactiveUI;

namespace Client.Shared.ViewModels
{
    public abstract class ReactiveWindowViewModelBase : ActivatableViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ViewCloseCommand { get; }

        protected ReactiveWindowViewModelBase()
        {
            this.ViewCloseCommand = ReactiveCommand.Create(() => { }, canExecute: null);
        }

        public virtual async Task OnClosingAsync(CancelEventArgs eventPattern)
        {
        }

        public virtual void OnClosing(CancelEventArgs eventPattern)
        {
        }

        protected void CloseView()
        {
            this.ViewCloseCommand.Execute().Subscribe();
        }
    }
}
