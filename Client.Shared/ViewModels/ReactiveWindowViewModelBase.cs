using System;
using System.ComponentModel;
using System.Reactive;
using System.Threading.Tasks;

using Client.Shared.Controls.BusyIndicator;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Shared.ViewModels
{
    public abstract class ReactiveWindowViewModelBase : ActivatableViewModelBase
    {
        protected BusyIndicatorViewModel BusyIndicatorViewModel { get; }

        [Reactive]
        protected bool IsBusy { get; set; }

        public ReactiveCommand<Unit, Unit> ViewCloseCommand { get; }

        protected ReactiveWindowViewModelBase()
        {
            this.BusyIndicatorViewModel = new BusyIndicatorViewModel();
            this.ViewCloseCommand = ReactiveCommand.Create(() => { }, canExecute: null);

            this.WhenAnyValue(x => x.IsBusy)
                .Subscribe(
                    _ =>
                    {
                        if (this.IsBusy)
                        {
                            this.BusyIndicatorViewModel.Show();
                        }
                        else
                        {
                            this.BusyIndicatorViewModel.Hide();
                        }
                    }
                );
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

        protected async Task DoActionAsync(Action action)
        {
            this.IsBusy = true;

            await Task.Run(action);

            this.IsBusy = false;
        }

        protected async Task<T> DoActionAsync<T>(Func<T> action)
        {
            this.IsBusy = true;

            T result = await Task.Run(action);

            this.IsBusy = false;

            return result;
        }
    }
}
