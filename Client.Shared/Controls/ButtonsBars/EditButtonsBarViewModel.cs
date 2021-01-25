using System;
using System.Reactive;

using Client.Shared.ViewModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Shared.Controls.ButtonsBars
{
    public class EditButtonsBarViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> CreateCommand { get; }

        [Reactive]
        public bool CanExecuteCreateCommand { get; set; } = true;

        [Reactive]
        public bool CreateIsVisible { get; set; } = true;

        [Reactive]
        public string CreateContent { get; set; } = "Create";

        public ReactiveCommand<Unit, Unit> CopyCommand { get; }

        [Reactive]
        public bool CanExecuteCopyCommand { get; set; } = true;

        [Reactive]
        public bool CopyIsVisible { get; set; } = true;

        [Reactive]
        public string CopyContent { get; set; } = "Copy";

        public ReactiveCommand<Unit, Unit> EditCommand { get; }

        [Reactive]
        public bool CanExecuteEditCommand { get; set; } = true;

        [Reactive]
        public bool EditIsVisible { get; set; } = true;

        [Reactive]
        public string EditContent { get; set; } = "Edit";

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        [Reactive]
        public bool CanExecuteDeleteCommand { get; set; } = true;

        [Reactive]
        public bool DeleteIsVisible { get; set; } = true;

        [Reactive]
        public string DeleteContent { get; set; } = "Delete";

        public EditButtonsBarViewModel()
        {
            IObservable<bool> canExecuteCreateCommand = this.WhenAnyValue(x => x.CanExecuteCreateCommand);
            IObservable<bool> canExecuteCopyCommand = this.WhenAnyValue(x => x.CanExecuteCopyCommand);
            IObservable<bool> canExecuteEditCommand = this.WhenAnyValue(x => x.CanExecuteEditCommand);
            IObservable<bool> canExecuteDeleteCommand = this.WhenAnyValue(x => x.CanExecuteDeleteCommand);

            this.CreateCommand = ReactiveCommand.Create(() => { }, canExecuteCreateCommand);
            this.CopyCommand = ReactiveCommand.Create(() => { }, canExecuteCopyCommand);
            this.EditCommand = ReactiveCommand.Create(() => { }, canExecuteEditCommand);
            this.DeleteCommand = ReactiveCommand.Create(() => { }, canExecuteDeleteCommand);
        }
    }
}
