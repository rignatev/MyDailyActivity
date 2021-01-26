using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace MyDailyActivity.Projects.ProjectEdit
{
    public class ProjectEditView : ReactiveWindowViewBase<ProjectEditViewModel>
    {
        public ProjectEditView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
