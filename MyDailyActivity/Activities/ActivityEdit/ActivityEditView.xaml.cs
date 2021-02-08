using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace MyDailyActivity.Activities.ActivityEdit
{
    public class ActivityEditView : ReactiveWindowViewBase<ActivityEditViewModel>
    {
        public ActivityEditView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
