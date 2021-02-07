using System.Linq;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Client.Shared.Views;

using ReactiveUI;

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

        private void InputElement_OnDoubleTapped(object sender, RoutedEventArgs e)
        {
            Observable.Start(() => { }).ObserveOn(RxApp.MainThreadScheduler).InvokeCommand(this.ViewModel.DataGridOnDoubleTapped);
        }
    }
}
