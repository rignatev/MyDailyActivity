using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Client.Shared.Controls.BusyIndicator
{
    public class BusyIndicator : UserControl
    {
        public BusyIndicator()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
