using System;
using System.Reactive;

using Client.Shared.ViewModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Shared.Controls.ButtonsBars
{
    public class BottomButtonsBarViewModel : ViewModelBase
    {
        public enum BarType
        {
            None = 0,
            Close = 1,
            OkCancel = 2,
            ApplyOkCancel = 3
        }

        public enum ButtonType
        {
            None = 0,
            Ok = 1,
            Apply = 2,
            Close = 3,
            Cancel = 4
        }

        public ReactiveCommand<Unit, Unit> ApplyCommand { get; }

        [Reactive]
        public bool CanExecuteApplyCommand { get; set; }

        [Reactive]
        public bool ApplyIsVisible { get; set; }

        [Reactive]
        public string ApplyContent { get; set; } = "Apply";

        public ReactiveCommand<Unit, Unit> OkCommand { get; }

        [Reactive]
        public bool CanExecuteOkCommand { get; set; }

        [Reactive]
        public bool OkIsVisible { get; set; }

        [Reactive]
        public string OkContent { get; set; } = "Ok";

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        [Reactive]
        public bool CanExecuteCancelCommand { get; set; }

        [Reactive]
        public bool CancelIsVisible { get; set; }

        [Reactive]
        public string CancelContent { get; set; } = "Cancel";

        public ReactiveCommand<Unit, Unit> CloseCommand { get; }

        [Reactive]
        public bool CanExecuteCloseCommand { get; set; }

        [Reactive]
        public bool CloseIsVisible { get; set; }

        [Reactive]
        public string CloseContent { get; set; } = "Close";

        public ButtonType LastButtonPressed { get; private set; }

        public BottomButtonsBarViewModel(BarType barType)
        {
            IObservable<bool> canExecuteApplyCommand = this.WhenAnyValue(x => x.CanExecuteApplyCommand);
            IObservable<bool> canExecuteOkCommand = this.WhenAnyValue(x => x.CanExecuteOkCommand);
            IObservable<bool> canExecuteCancelCommand = this.WhenAnyValue(x => x.CanExecuteCancelCommand);
            IObservable<bool> canExecuteCloseCommand = this.WhenAnyValue(x => x.CanExecuteCloseCommand);

            this.ApplyCommand = ReactiveCommand.Create(() => { }, canExecuteApplyCommand);
            this.OkCommand = ReactiveCommand.Create(() => { }, canExecuteOkCommand);
            this.CancelCommand = ReactiveCommand.Create(() => { }, canExecuteCancelCommand);
            this.CloseCommand = ReactiveCommand.Create(() => { }, canExecuteCloseCommand);

            this.ApplyCommand.Subscribe(x => this.LastButtonPressed = ButtonType.Apply);
            this.OkCommand.Subscribe(x => this.LastButtonPressed = ButtonType.Ok);
            this.CancelCommand.Subscribe(x => this.LastButtonPressed = ButtonType.Cancel);
            this.CloseCommand.Subscribe(x => this.LastButtonPressed = ButtonType.Close);

            SetButtonsBarType(barType);
        }

        private void SetButtonsBarType(BarType barType)
        {
            switch (barType)
            {
                case BarType.ApplyOkCancel:
                    this.ApplyIsVisible = true;
                    this.OkIsVisible = true;
                    this.CancelIsVisible = true;
                    this.CanExecuteCancelCommand = true;

                    break;
                case BarType.Close:
                    this.CloseIsVisible = true;
                    this.CanExecuteCloseCommand = true;

                    break;
                case BarType.OkCancel:
                    this.OkIsVisible = true;
                    this.CancelIsVisible = true;
                    this.CanExecuteCancelCommand = true;

                    break;
                default: throw new ArgumentOutOfRangeException(nameof(barType), barType, message: null);
            }
        }
    }
}
