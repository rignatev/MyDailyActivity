using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace Client.Shared.Controls.ButtonsBars
{
    public class EditButtonsBarView : UserControlViewBase
    {
        public EditButtonsBarView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
