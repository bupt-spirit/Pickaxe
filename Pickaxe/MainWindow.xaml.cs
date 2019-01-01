using Pickaxe.ViewModel;
using System.Windows;
using System.Windows.Controls;

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

        private void AlgorithmHistoryListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            var textBox = (TextBox)listView.Tag;
            var algorithmHistory = (AlgorithmHistoryViewModel)listView.SelectedItem;
            textBox.Text = algorithmHistory.OutputText;
        }
    }
}
