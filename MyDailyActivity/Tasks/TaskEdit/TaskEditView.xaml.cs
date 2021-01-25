using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace MyDailyActivity.Tasks.TaskEdit
{
    public class TaskEditView : ReactiveWindowViewBase<TaskEditViewModel>
    {
        public TaskEditView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
