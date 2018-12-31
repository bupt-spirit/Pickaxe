using LiveCharts;
using LiveCharts.Wpf;
using Pickaxe.Model;
using Pickaxe.ViewModel;
using System.Windows;

namespace Pickaxe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel)DataContext;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AttributeDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ViewModel.RefreshStatisticsView.CanExecute(attributeDataGrid.SelectedItem))
            {
                ViewModel.RefreshStatisticsView.Execute(attributeDataGrid.SelectedItem);
            }
        }
    }
}
