using System;

using Avalonia.Media;

using Client.Shared.ViewModels;

using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Shared.Controls.BusyIndicator
{
    public class BusyIndicatorViewModel : ViewModelBase
    {
        [Reactive]
        public bool IsVisible { get; private set; }

        [Reactive]
        public string Text { get; set; } = "Please wait...";

        [Reactive]
        public IBrush Foreground { get; set; } = new SolidColorBrush(Colors.White);

        [Reactive]
        public bool TextIsVisible { get; set; } = true;

        [Reactive]
        public bool ProgressBarIsVisible { get; set; } = true;
        
        public IObservable<bool> IsVisibleChanged { get; }

        public BusyIndicatorViewModel()
        {
            this.IsVisibleChanged = this.WhenAnyValue(x => x.IsVisible);
        }

        public void Show()
        {
            this.IsVisible = true;
        }

        public void Hide()
        {
            this.IsVisible = false;
        }
    }
}
