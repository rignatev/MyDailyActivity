using System.Linq;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace MyDailyActivity.Projects
{
    public class ProjectsWindowView : ReactiveWindowViewBase<ProjectsWindowViewModel>
    {
        public ProjectsWindowView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ProjectsDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is DataGrid dataGrid)
            {
                this.ViewModel.SelectedProjects = dataGrid.SelectedItems.Cast<ProjectsWindowViewModel.ViewListItem>().ToList();
            }
        }
    }
}
