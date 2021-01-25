using System.Reactive.Disposables;
using System.Threading.Tasks;

using Avalonia.Controls;

using MessageBox.Avalonia;
using MessageBox.Avalonia.BaseWindows.Base;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

using ReactiveUI;

namespace Client.Shared.ViewModels
{
    public abstract class ActivatableViewModelBase : ViewModelBase, IActivatableViewModel
    {
        /// <inheritdoc />
        public ViewModelActivator Activator { get; }

        public Window OwnerWindow { get; set; }

        protected ActivatableViewModelBase()
        {
            this.Activator = new ViewModelActivator();

            this.WhenActivated(
                disposables =>
                {
                    HandleActivation(disposables);

                    Disposable
                        .Create(() => HandleDeactivation(disposables))
                        .DisposeWith(disposables);
                }
            );
        }

        protected virtual void HandleActivation(CompositeDisposable disposables)
        {
        }

        protected virtual void HandleDeactivation(CompositeDisposable disposables)
        {
        }

        protected async Task<ButtonResult> ShowInfoDialog(string contentMessage, string contentTitle = "Information") =>
            await ShowMessageDialog(contentTitle, contentMessage, ButtonEnum.Ok, Icon.Info);

        protected async Task<ButtonResult> ShowWarningDialog(string contentMessage, string contentTitle = "Warning") =>
            await ShowMessageDialog(contentTitle, contentMessage, ButtonEnum.Ok, Icon.Warning);

        protected async Task<ButtonResult> ShowErrorDialog(string contentMessage, string contentTitle = "Error") =>
            await ShowMessageDialog(contentTitle, contentMessage, ButtonEnum.Ok, Icon.Error);

        protected async Task<ButtonResult> ShowConfirmationDialog(string contentMessage, string contentTitle = "Confirmation") =>
            await ShowMessageDialog(contentTitle, contentMessage, ButtonEnum.YesNo, Icon.Warning);

        protected async Task<ButtonResult> ShowMessageDialog(
            string contentTitle,
            string contentMessage,
            ButtonEnum buttonDefinitions,
            Icon icon)
        {
            IMsBoxWindow<ButtonResult> msBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow(
                new MessageBoxStandardParams
                {
                    ContentTitle = contentTitle,
                    ContentMessage = contentMessage,
                    Icon = icon,
                    ButtonDefinitions = buttonDefinitions,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false
                }
            );

            return await msBoxStandardWindow.ShowDialog(this.OwnerWindow);
        }

        static protected bool ShowConfirmationDialogResultConfirmed(ButtonResult buttonResult) =>
            buttonResult == ButtonResult.Yes;
    }
}
