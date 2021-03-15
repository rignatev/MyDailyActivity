using System;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
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

        private void DataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                this.ViewModel.SelectedProjects = dataGrid.SelectedItems.Cast<ProjectsWindowViewModel.ViewListItem>().ToList();
            }
        }

        private void DataGrid_OnDoubleTapped(object sender, RoutedEventArgs e)
        {
            this.ViewModel.DataGridOnDoubleTapped.Execute().Subscribe();
        }
    }
}
