using Pickaxe.Utility;
using PickaxeCore.Relation;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pickaxe.Tabs
{
    /// <summary>
    /// Interaction logic for DataSet.xaml
    /// </summary>
    public partial class DataSetControl : UserControl
    {
        private Relation relation;

        public DataSetControl(Relation relation)
        {
            this.relation = relation;
            InitializeComponent();
            BindRelation();
            ReloadSource();
        }

        private void ReloadSource() {
            this.dataGrid.ItemsSource = this.relation.Tuples();
        }

        private void BindRelation()
        {
            this.dataGrid.Columns.Clear();
            int attributeIndex = -1;
            foreach (var attribute in this.relation.Attributes)
            {
                attributeIndex += 1;
                DataGridColumn column;
                if (attribute.Type is AttributeType.Binary)
                {
                    var c = new IndexedDataGridTemplateColumn
                    {
                        CellTemplate = this.ColumnCellDataTemplate(attribute.Type),
                        CellEditingTemplate = this.ColumnCellDataTemplate(attribute.Type),
                        ColumnIndex = attributeIndex
                    };
                    column = c;
                }
                else if (attribute.Type is AttributeType.Nominal)
                {
                    var c = new IndexedDataGridTemplateColumn
                    {
                        CellTemplate = this.ColumnCellDataTemplate(attribute.Type),
                        CellEditingTemplate = this.ColumnCellDataTemplate(attribute.Type),
                        ColumnIndex = attributeIndex
                    };
                    column = c;
                }
                else if (attribute.Type is AttributeType.Numeric)
                {
                    var c = new IndexedDataGridTemplateColumn
                    {
                        CellTemplate = this.ColumnCellDataTemplate(attribute.Type),
                        CellEditingTemplate = this.ColumnCellDataTemplate(attribute.Type),
                        ColumnIndex = attributeIndex
                    };
                    column = c;
                }
                else
                {
                    throw new Exception(); // imposible state
                }
                column.Header = attribute.Name;
                this.dataGrid.Columns.Add(column);
            }
            
        }

        private DataTemplate ColumnCellDataTemplate(AttributeType type)
        {
            var template = new DataTemplate
            {
                DataType = typeof(Value)
            };
            FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            var binding = new Binding
            {
                Converter = new ValueStringConverter(type)
            };
            textBlockFactory.SetBinding(TextBlock.TextProperty, binding);
            Style style = new Style();
            style.Triggers.Add(CellBackgroundTrigger());
            textBlockFactory.SetValue(TextBlock.StyleProperty, style);
            template.VisualTree = textBlockFactory;
            return template;
        }

        private DataTrigger CellBackgroundTrigger()
        {
            DataTrigger trigger = new DataTrigger
            {
                Binding = new Binding()
                {
                    Converter = new ValueIsMissingConverter()
                },
                Value = true,
            };
            Setter setter = new Setter() {
                Property = TextBlock.BackgroundProperty,
                Value = FindResource("MissingValueBackgroundBrush"),
            };
            trigger.Setters.Add(setter);
            return trigger;
        }

        private void NewTuple_Clicked(object sender, RoutedEventArgs e)
        {
            this.relation.AddTuple();
            ReloadSource();
        }

        private void NewAttribute_Clicked(object sender, RoutedEventArgs e)
        {
            this.relation.AddAttribute("New Attribute", new AttributeType.Numeric());
            BindRelation();
            ReloadSource();
        }
    }

    public class IndexedDataGridTemplateColumn : DataGridTemplateColumn
    {
        public int ColumnIndex
        {
            get;
            set;
        }

        protected override System.Windows.FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            // The DataGridTemplateColumn uses ContentPresenter with DataTemplate.
            ContentPresenter cp = (ContentPresenter)base.GenerateElement(cell, dataItem);
            // Reset the Binding to the specific column. The default binding is to the DataRowView.
            BindingOperations.SetBinding(cp, ContentPresenter.ContentProperty, new Binding($"[{this.ColumnIndex}]"));
            return cp;
        }
    }
}
