using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace MyDailyActivity.MainWindow
{
    public class MainWindowView : ReactiveWindowViewBase<MainWindowViewModel>
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
