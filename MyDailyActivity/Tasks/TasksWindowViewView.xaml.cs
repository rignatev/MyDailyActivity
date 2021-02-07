using System.Linq;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Client.Shared.Views;

using ReactiveUI;

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
            if (sender is DataGrid dataGrid)
            {
                this.ViewModel.SelectedTasks = dataGrid.SelectedItems.Cast<TasksWindowViewModel.ViewListItem>().ToList();
            }
        }

        private void InputElement_OnDoubleTapped(object sender, RoutedEventArgs e)
        {
            Observable.Start(() => { }).ObserveOn(RxApp.MainThreadScheduler).InvokeCommand(this.ViewModel.DataGridOnDoubleTapped);
        }
    }
}
