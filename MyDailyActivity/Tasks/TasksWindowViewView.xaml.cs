using System.Linq;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using Client.Shared.Views;

namespace MyDailyActivity.Tasks
{
    public class TasksWindowView : ReactiveWindowViewBase<TasksWindowViewModel>
    {
        public TasksWindowView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void TasksDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is DataGrid dataGrid)
            {
                this.ViewModel.SelectedTasks = dataGrid.SelectedItems.Cast<TasksWindowViewModel.ViewListItem>().ToList();
            }
        }
    }
}
