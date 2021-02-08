using System;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                this.ViewModel.SelectedTasks = dataGrid.SelectedItems.Cast<TasksWindowViewModel.ViewListItem>().ToList();
            }
        }

        private void DataGrid_OnDoubleTapped(object sender, RoutedEventArgs e)
        {
            this.ViewModel.DataGridOnDoubleTapped.Execute().Subscribe();
        }
    }
}
