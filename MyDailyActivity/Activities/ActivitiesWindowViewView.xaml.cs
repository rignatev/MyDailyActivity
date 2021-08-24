using System;
using System.Linq;
using System.Reactive.Linq;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

using Client.Shared.Views;

using ReactiveUI;

namespace MyDailyActivity.Activities
{
    public class ActivitiesWindowView : ReactiveWindowViewBase<ActivitiesWindowViewModel>
    {
        public ActivitiesWindowView()
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
                this.ViewModel!.SelectedActivities = dataGrid.SelectedItems.Cast<ActivitiesWindowViewModel.ViewListItem>().ToList();
            }
        }

        private void DataGrid_OnDoubleTapped(object sender, RoutedEventArgs e)
        {
            this.ViewModel!.DataGridOnDoubleTapped.Execute().ObserveOn(RxApp.MainThreadScheduler).Subscribe();
        }
    }
}
