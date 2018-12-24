using Pickaxe.Utility.Converter;
using PickaxeCore.Relation;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

namespace Pickaxe
{
    /// <summary>
    /// Interaction logic for DataSet.xaml
    /// </summary>
    public partial class DataSetControl : UserControl
    {
        public Relation Relation { get; set; }

        public DataSetControl(Relation relation)
        {
            this.Relation = relation;
            InitializeComponent();
        }

        private void InitializeRelationBinding()
        {
            this.dataGrid.Columns.Clear();
            int attributeIndex = 0;
            foreach (var attribute in this.Relation.Attributes)
            {
                InsertColumn(attributeIndex, attribute);
                attributeIndex += 1;
            }
            this.dataGrid.DataContext = this.Relation.TupleViews;
        }

        private void InsertColumn(int attributeIndex, RelationAttribute attribute) {
            DataGridColumn column;
            if (attribute.Type is AttributeType.Binary)
            {
                var c = new IndexedDataGridTemplateColumn
                {
                    CellTemplate = this.ColumnCellDataTemplate(attribute.Type),
                    CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
                    ColumnIndex = attributeIndex
                };
                column = c;
            }
            else if (attribute.Type is AttributeType.Nominal)
            {
                var c = new IndexedDataGridTemplateColumn
                {
                    CellTemplate = this.ColumnCellDataTemplate(attribute.Type),
                    CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
                    ColumnIndex = attributeIndex
                };
                column = c;
            }
            else if (attribute.Type is AttributeType.Numeric)
            {
                var c = new IndexedDataGridTemplateColumn
                {
                    CellTemplate = this.ColumnCellDataTemplate(attribute.Type),
                    CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
                    ColumnIndex = attributeIndex
                };
                column = c;
            }
            else
            {
                throw new Exception(); // imposible state
            }
            column.Header = attribute;
            column.HeaderTemplate = (DataTemplate)this.FindResource("ColumnHeaderTemplate");
            this.dataGrid.Columns.Insert(attributeIndex, column);
            // adjust columns after
            for (var i = attributeIndex + 1; i < this.dataGrid.Columns.Count; ++i)
            {
                var c = (IndexedDataGridTemplateColumn)this.dataGrid.Columns[i];
                // rebind c.ColumnIndex
                c.ColumnIndex += 1;
                var relationAttribute = (RelationAttribute)c.Header;
                c.CellTemplate = this.ColumnCellDataTemplate(relationAttribute.Type);
                c.CellEditingTemplate = this.ColumnCellEditingDataTemplate(relationAttribute.Type, c.ColumnIndex);
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
            textBlockFactory.SetValue(TextBlock.StyleProperty, this.FindResource("TextBlockBackgroundStyle"));
            textBlockFactory.SetValue(TextBlock.ContextMenuProperty, this.FindResource("DataGridCellContextMenu"));
            template.VisualTree = textBlockFactory;
            return template;
        }

        private DataTemplate ComboBoxItemDataTemplate(AttributeType type)
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
            template.VisualTree = textBlockFactory;
            return template;
        }

        private DataTemplate ColumnCellEditingDataTemplate(AttributeType type, int attributeIndex)
        {
            var template = new DataTemplate
            {
                DataType = typeof(Value)
            };
            if (type is AttributeType.Binary binaryAttribute)
            {
                FrameworkElementFactory comboBox = new FrameworkElementFactory(typeof(ComboBox));
                comboBox.SetValue(ComboBox.ItemsSourceProperty, new List<Value> { Value.ToValue(false), Value.ToValue(true), Value.MISSING });
                comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding($"[{attributeIndex}]"));
                var itemTemplate = ComboBoxItemDataTemplate(type);
                comboBox.SetValue(ComboBox.ItemTemplateProperty, itemTemplate);
                comboBox.SetValue(ComboBox.ItemContainerStyleProperty, this.FindResource("ComboBoxItemBackgroundStyle"));
                comboBox.SetValue(ComboBox.IsDropDownOpenProperty, true);
                template.VisualTree = comboBox;
            }
            else if (type is AttributeType.Nominal nominalAttribute)
            {
                FrameworkElementFactory comboBox = new FrameworkElementFactory(typeof(ComboBox));
                comboBox.SetValue(ComboBox.ItemsSourceProperty, nominalAttribute.Values.Append(Value.MISSING));
                comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding($"[{attributeIndex}]"));
                var itemTemplate = ComboBoxItemDataTemplate(type);
                comboBox.SetValue(ComboBox.ItemTemplateProperty, itemTemplate);
                comboBox.SetValue(ComboBox.ItemContainerStyleProperty, this.FindResource("ComboBoxItemBackgroundStyle"));
                comboBox.SetValue(ComboBox.IsDropDownOpenProperty, true);
                template.VisualTree = comboBox;
            }
            else if (type is AttributeType.Numeric numericAttribute)
            {
                FrameworkElementFactory textBox = new FrameworkElementFactory(typeof(TextBox));
                textBox.SetBinding(TextBox.TextProperty, new Binding($"[{attributeIndex}]")
                {
                    Converter = (NominalValueStringConverter)this.FindResource("NominalValueStringConverter"),
                });
                template.VisualTree = textBox;
            }
            else
            {
                throw new Exception(); // imposible state
            }
            template.VisualTree.AddHandler(FrameworkElement.PreviewMouseRightButtonDownEvent,
                    new MouseButtonEventHandler(DataGridCell_PreviewMouseRightButtonDown));
            return template;
        }

        private RelationAttribute NewRelationAttribute()
        {
            return this.Relation.AllMissingAttribute("New Attribute", new AttributeType.Numeric());
        }

        #region Event Handlers

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeRelationBinding();
        }

        private void NewTuple_Clicked(object sender, RoutedEventArgs e)
        {
            this.Relation.AddTuple();
        }

        private void NewAttribute_Clicked(object sender, RoutedEventArgs e)
        {
            var index = this.Relation.AttributeCount;
            this.Relation.AddAttribute(NewRelationAttribute());
            this.InsertColumn(index, this.Relation.Attributes[index]);
        }

        private void InsertAttribute_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is RelationAttribute relationAttribute)
            {
                int index = this.Relation.Attributes.FindIndex((r) => r.Equals(relationAttribute));
                if (index == -1)
                {
                    throw new Exception($"invalid relation attribute {relationAttribute} passed to insert attribute clicked");
                }
                var attribute = NewRelationAttribute();
                this.Relation.InsertAttribute(index, attribute);
                this.InsertColumn(index, attribute);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void RemoveAttribute_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is RelationAttribute relationAttribute)
            {
                int index = this.Relation.Attributes.FindIndex((r) => r.Equals(relationAttribute));
                if (index == -1)
                {
                    throw new Exception($"invalid relation attribute {relationAttribute} passed to insert attribute clicked");
                }
                this.dataGrid.Columns.RemoveAt(index);
                this.Relation.RemoveAttribute(index);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public class IndexedDataGridTemplateColumn : DataGridTemplateColumn
        {
            public int ColumnIndex
            {
                get;
                set;
            }

            protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
            {
                // The DataGridTemplateColumn uses ContentPresenter with DataTemplate.
                ContentPresenter cp = (ContentPresenter)base.GenerateElement(cell, dataItem);
                // Reset the Binding to the specific column. The default binding is to the DataRowView.
                BindingOperations.SetBinding(cp, ContentPresenter.ContentProperty, new Binding($"[{this.ColumnIndex}]"));
                return cp;
            }
        }

        private void DataGridCell_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = (DependencyObject)sender;
            while (dependencyObject != null && !(dependencyObject is DataGridCell))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            if (dependencyObject is DataGridCell dataGridCell)
            {
                if (dataGridCell.IsEditing)
                {
                    dataGridCell.IsEditing = false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void InsertTuple_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var selectedItems = this.dataGrid.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    return;
                }
                var indecies = new List<int>();
                foreach (var selectedItem in selectedItems)
                {
                    var tupleView = selectedItem as TupleView;
                    indecies.Add(tupleView.TupleIndex);
                }
                indecies.Sort((a, b) => -a.CompareTo(b));
                foreach (var index in indecies)
                {
                    this.Relation.InsertTuple(index);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void RemoveTuple_Clicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                var selectedItems = this.dataGrid.SelectedItems;
                if (selectedItems.Count == 0)
                {
                    return;
                }
                var indecies = new List<int>();
                foreach (var selectedItem in selectedItems)
                {
                    var tupleView = selectedItem as TupleView;
                    indecies.Add(tupleView.TupleIndex);
                }
                indecies.Sort((a, b) => -a.CompareTo(b));
                foreach (var index in indecies)
                {
                    this.Relation.RemoveTupleAt(index);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion // Event Handlers
    }
}
