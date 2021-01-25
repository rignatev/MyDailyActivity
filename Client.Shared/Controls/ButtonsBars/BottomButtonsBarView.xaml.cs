using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace Client.Shared.Controls.ButtonsBars
{
    public class BottomButtonsBarView : UserControlViewBase
    {
        public BottomButtonsBarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
