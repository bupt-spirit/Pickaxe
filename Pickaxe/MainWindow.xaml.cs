using PickaxeCore.Relation;
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
        private Relation relation;

        public MainWindow()
        {
            InitializeRelation();
            InitializeComponent();
            this.tabItemDataSet.Content = new DataSetControl(relation);
        }

        private void InitializeRelation()
        {
            this.relation = new Relation();
            this.relation.AddAttribute(this.relation.AllMissingAttribute(
                "Numeric Attribute",
                new PickaxeCore.Relation.AttributeType.Numeric()));
            this.relation.AddAttribute(this.relation.AllMissingAttribute(
                "Nominal Attribute",
                new PickaxeCore.Relation.AttributeType.Nominal(
                    new[] { "type 1", "type 2", "type 3" })
                    ));
            this.relation.AddAttribute(this.relation.AllMissingAttribute(
                "Binray Attribute",
                new PickaxeCore.Relation.AttributeType.Binary()));
            this.relation.AddTuple(new Value[] { 1.1f, 2f, 0f });
            this.relation.AddTuple(new Value[] { 2.1f, 2f, 0f });
            this.relation.AddTuple(new Value[] { 3.1f, 2f, Value.MISSING });
        }
    }
}
