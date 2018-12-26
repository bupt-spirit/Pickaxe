using Pickaxe.Model;
using Pickaxe.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        //private void InitializeRelation()
        //{
        //    this.relation = new Relation();
        //    this.relation.AddAttribute(this.relation.AllMissingAttribute(
        //        "Numeric Attribute",
        //        new Pickaxe.Model.AttributeType.Numeric()));
        //    this.relation.AddAttribute(this.relation.AllMissingAttribute(
        //        "Nominal Attribute",
        //        new Pickaxe.Model.AttributeType.Nominal(
        //            new[] { "type 1", "type 2", "type 3" })
        //            ));
        //    this.relation.AddAttribute(this.relation.AllMissingAttribute(
        //        "Binray Attribute",
        //        new Pickaxe.Model.AttributeType.Binary()));
        //    this.relation.AddTuple(new Value[] { 1.1f, 2f, 0f });
        //    this.relation.AddTuple(new Value[] { 2.1f, 2f, 0f });
        //    this.relation.AddTuple(new Value[] { 3.1f, 2f, Value.MISSING });
        //}

    //    private void NewButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        this.dataGrid.Columns.Clear();
    //        this.Relation.Clear();
    //    }

    //    private void OpenButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        var openFileDialog = new Microsoft.Win32.OpenFileDialog();
    //        if (openFileDialog.ShowDialog() == true)
    //        {
    //            this.FileName = openFileDialog.FileName;
    //            using (var fileStream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read))
    //            {
    //                this.Relation.ReadFromStream(fileStream);
    //                this.InitializeRelationBinding();
    //            }
    //        }
    //    }

    //    private void SaveButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        if (this.FileName == null)
    //        {
    //            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
    //            if (saveFileDialog.ShowDialog() == true)
    //            {
    //                this.FileName = saveFileDialog.FileName;
    //            }
    //            else
    //            {
    //                return;
    //            }
    //        }
    //        using (var fileStream = new FileStream(this.FileName, FileMode.Create, FileAccess.Write))
    //        {
    //            this.Relation.SaveToStream(fileStream);
    //        }
    //    }

    //    private void SaveAsButton_Click(object sender, RoutedEventArgs e)
    //    {
    //        var saveFileDialog = new Microsoft.Win32.SaveFileDialog();
    //        if (saveFileDialog.ShowDialog() == true)
    //        {
    //            this.FileName = saveFileDialog.FileName;
    //            using (var fileStream = new FileStream(this.FileName, FileMode.Create, FileAccess.Write))
    //            {
    //                this.Relation.SaveToStream(fileStream);
    //            }
    //        }
    //    }
    }
}
