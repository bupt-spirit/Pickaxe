using Pickaxe.Model;
using Pickaxe.Utility.Converter;
using Pickaxe.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Pickaxe.View
{
    /// <summary>
    /// Interaction logic for relationEditGrid.xaml
    /// </summary>
    public partial class RelationEditGrid : UserControl
    {
        protected RelationEditGridViewModel ViewModel
        {
            get => (RelationEditGridViewModel)DataContext;
        }

        public RelationEditGrid()
        {
            InitializeComponent();
        }

        #region Relation dependency proterty

        public static readonly DependencyProperty RelationProperty =
            DependencyProperty.Register(
            "Relation", typeof(Relation),
            typeof(RelationEditGrid),
            new PropertyMetadata(null, StaticRelation_PropertyChanged)
            );
        public Relation Relation
        {
            get { return (Relation)GetValue(RelationProperty); }
            set { SetValue(RelationProperty, value); }
        }

        public static void StaticRelation_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RelationEditGrid)d).Relation_PropertyChanged(e);
        }

        private void Relation_PropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((Relation)e.OldValue).CollectionChanged -= Relation_CollectionChanged;

            ViewModel.Relation = (Relation)e.NewValue;
            ClearColumn();
            var index = 0;
            foreach (var attribute in ViewModel.Relation)
            {
                InsertColumn(index, ViewModel.Relation[index]);
                index += 1;
            }
            ViewModel.Relation.CollectionChanged += Relation_CollectionChanged;
        }

        #endregion

        #region Column management

        private void InsertColumn(int attributeIndex, RelationAttribute attribute)
        {
            DataGridColumn column = new DataGridTemplateColumn
            {
                CellTemplate = this.ColumnCellDataTemplate(attribute.Type, attributeIndex),
                CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
            };
            column.Header = attribute;
            column.HeaderTemplate = (DataTemplate)this.FindResource("ColumnHeaderTemplate");
            this.dataGrid.Columns.Insert(attributeIndex, column);
            // adjust columns after
            for (var i = attributeIndex + 1; i < this.dataGrid.Columns.Count; ++i)
            {
                var c = (DataGridTemplateColumn)this.dataGrid.Columns[i];
                var relationAttribute = (RelationAttribute)c.Header;
                c.CellTemplate = this.ColumnCellDataTemplate(relationAttribute.Type, i);
                c.CellEditingTemplate = this.ColumnCellEditingDataTemplate(relationAttribute.Type, i);
            }
        }

        private void RemoveColumn(int attributeIndex)
        {
            this.dataGrid.Columns.RemoveAt(attributeIndex);
            // adjust columns after
            for (var i = attributeIndex; i < this.dataGrid.Columns.Count; ++i)
            {
                var c = (DataGridTemplateColumn)this.dataGrid.Columns[i];
                var relationAttribute = (RelationAttribute)c.Header;
                c.CellTemplate = this.ColumnCellDataTemplate(relationAttribute.Type, i);
                c.CellEditingTemplate = this.ColumnCellEditingDataTemplate(relationAttribute.Type, i);
            }
        }

        private void ClearColumn()
        {
            this.dataGrid.Columns.Clear();
        }

        #endregion

        #region Data templates

        private DataTrigger ValueMissingForegroundTrigger(DependencyProperty property, int attributeIndex)
        {
            var trigger = new DataTrigger()
            {
                Binding = new Binding($"[{attributeIndex}]")
                {
                    Converter = (IValueConverter)FindResource("ValueIsMissingConverter"),
                },
                Value = true,
            };
            trigger.Setters.Add(new Setter(property, FindResource("MissingValueForgroundBrush")));
            return trigger;
        }

        private DataTemplate ColumnCellDataTemplate(AttributeType type, int attributeIndex)
        {
            var template = new DataTemplate(typeof(Value));
            FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding($"[{attributeIndex}]")
            {
                Converter = new ValueStringConverter(type)
            });
            Style style = new Style();
            style.Triggers.Add(ValueMissingForegroundTrigger(TextBlock.ForegroundProperty, attributeIndex));
            textBlockFactory.SetValue(TextBlock.StyleProperty, style);
            //textBlockFactory.SetValue(TextBlock.ContextMenuProperty, this.FindResource("DataGridCellContextMenu"));
            template.VisualTree = textBlockFactory;
            return template;
        }

        private DataTemplate ComboBoxItemDataTemplate(AttributeType type)
        {
            var template = new DataTemplate(typeof(Value));
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
                comboBox.SetValue(ComboBox.ItemsSourceProperty, nominalAttribute.ValidValues);
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

        #endregion // Data templates

        #region Event handlers

        private void Relation_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var index = e.NewStartingIndex;
                        InsertColumn(index, ViewModel.Relation[index]);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveColumn(e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearColumn();
                    break;
                default:
                    throw new NotImplementedException();
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

        #endregion // Event handlers
    }

    ///// <summary>
    ///// Interaction logic for relationEditGrid.xaml
    ///// </summary>
    //public partial class RelationEditGrid : UserControl
    //{
    //    public Relation Relation { get => DataContext as Relation; }

    //    public RelationEditGrid()
    //    {
    //        InitializeComponent();
    //    }

    //    private void InitializeRelationBinding()
    //    {
    //        dataGrid.Columns.Clear();

    //        int attributeIndex = 0;
    //        foreach (var attribute in Relation.Attributes)
    //        {
    //            InsertColumn(attributeIndex, attribute);
    //            attributeIndex += 1;
    //        }

    //        dataGrid.DataContext = Relation.TupleViews;
    //    }

    //    #region Column Management

    //    private void InsertColumn(int attributeIndex, RelationAttribute attribute)
    //    {
    //        DataGridColumn column;
    //        if (attribute.Type is AttributeType.Binary)
    //        {
    //            var c = new DataGridTemplateColumn
    //            {
    //                CellTemplate = this.ColumnCellDataTemplate(attribute.Type, attributeIndex),
    //                CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
    //            };
    //            column = c;
    //        }
    //        else if (attribute.Type is AttributeType.Nominal)
    //        {
    //            var c = new DataGridTemplateColumn
    //            {
    //                CellTemplate = this.ColumnCellDataTemplate(attribute.Type, attributeIndex),
    //                CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
    //            };
    //            column = c;
    //        }
    //        else if (attribute.Type is AttributeType.Numeric)
    //        {
    //            var c = new DataGridTemplateColumn
    //            {
    //                CellTemplate = this.ColumnCellDataTemplate(attribute.Type, attributeIndex),
    //                CellEditingTemplate = this.ColumnCellEditingDataTemplate(attribute.Type, attributeIndex),
    //            };
    //            column = c;
    //        }
    //        else
    //        {
    //            throw new Exception(); // imposible state
    //        }
    //        column.Header = attribute;
    //        column.HeaderTemplate = (DataTemplate)this.FindResource("ColumnHeaderTemplate");
    //        this.dataGrid.Columns.Insert(attributeIndex, column);
    //        // adjust columns after
    //        for (var i = attributeIndex + 1; i < this.dataGrid.Columns.Count; ++i)
    //        {
    //            var c = (DataGridTemplateColumn)this.dataGrid.Columns[i];
    //            var relationAttribute = (RelationAttribute)c.Header;
    //            c.CellTemplate = this.ColumnCellDataTemplate(relationAttribute.Type, i);
    //            c.CellEditingTemplate = this.ColumnCellEditingDataTemplate(relationAttribute.Type, i);
    //        }
    //    }

    //    private void RemoveColumn(int attributeIndex)
    //    {
    //        this.dataGrid.Columns.RemoveAt(attributeIndex);
    //        // adjust columns after
    //        for (var i = attributeIndex; i < this.dataGrid.Columns.Count; ++i)
    //        {
    //            var c = (DataGridTemplateColumn)this.dataGrid.Columns[i];
    //            var relationAttribute = (RelationAttribute)c.Header;
    //            c.CellTemplate = this.ColumnCellDataTemplate(relationAttribute.Type, i);
    //            c.CellEditingTemplate = this.ColumnCellEditingDataTemplate(relationAttribute.Type, i);
    //        }
    //    }

    //    #endregion // Column Management

    //    #region Data Templates

    //    private DataTrigger ValueMissingForegroundTrigger(DependencyProperty property, int attributeIndex)
    //    {
    //        var trigger = new DataTrigger()
    //        {
    //            Binding = new Binding($"[{attributeIndex}]")
    //            {
    //                Converter = (IValueConverter)FindResource("ValueIsMissingConverter"),
    //            },
    //            Value = true,
    //        };
    //        trigger.Setters.Add(new Setter(property, FindResource("MissingValueForgroundBrush")));
    //        return trigger;
    //    }

    //    private DataTemplate ColumnCellDataTemplate(AttributeType type, int attributeIndex)
    //    {
    //        var template = new DataTemplate
    //        {
    //            DataType = typeof(Value)
    //        };
    //        FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
    //        textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding($"[{attributeIndex}]") {
    //            Converter = new ValueStringConverter(type)
    //        });
    //        Style style = new Style();
    //        style.Triggers.Add(ValueMissingForegroundTrigger(TextBlock.ForegroundProperty, attributeIndex));
    //        textBlockFactory.SetValue(TextBlock.StyleProperty, style);
    //        textBlockFactory.SetValue(TextBlock.ContextMenuProperty, this.FindResource("DataGridCellContextMenu"));
    //        template.VisualTree = textBlockFactory;
    //        return template;
    //    }

    //    private DataTemplate ComboBoxItemDataTemplate(AttributeType type)
    //    {
    //        var template = new DataTemplate
    //        {
    //            DataType = typeof(Value)
    //        };
    //        FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
    //        var binding = new Binding
    //        {
    //            Converter = new ValueStringConverter(type)
    //        };
    //        textBlockFactory.SetBinding(TextBlock.TextProperty, binding);
    //        template.VisualTree = textBlockFactory;
    //        return template;
    //    }

    //    private DataTemplate ColumnCellEditingDataTemplate(AttributeType type, int attributeIndex)
    //    {
    //        var template = new DataTemplate
    //        {
    //            DataType = typeof(Value)
    //        };
    //        if (type is AttributeType.Binary binaryAttribute)
    //        {
    //            FrameworkElementFactory comboBox = new FrameworkElementFactory(typeof(ComboBox));
    //            comboBox.SetValue(ComboBox.ItemsSourceProperty, new List<Value> { Value.ToValue(false), Value.ToValue(true), Value.MISSING });
    //            comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding($"[{attributeIndex}]"));
    //            var itemTemplate = ComboBoxItemDataTemplate(type);
    //            comboBox.SetValue(ComboBox.ItemTemplateProperty, itemTemplate);
    //            comboBox.SetValue(ComboBox.ItemContainerStyleProperty, this.FindResource("ComboBoxItemBackgroundStyle"));
    //            comboBox.SetValue(ComboBox.IsDropDownOpenProperty, true);
    //            template.VisualTree = comboBox;
    //        }
    //        else if (type is AttributeType.Nominal nominalAttribute)
    //        {
    //            FrameworkElementFactory comboBox = new FrameworkElementFactory(typeof(ComboBox));
    //            comboBox.SetValue(ComboBox.ItemsSourceProperty, nominalAttribute.Values.Append(Value.MISSING));
    //            comboBox.SetBinding(ComboBox.SelectedItemProperty, new Binding($"[{attributeIndex}]"));
    //            var itemTemplate = ComboBoxItemDataTemplate(type);
    //            comboBox.SetValue(ComboBox.ItemTemplateProperty, itemTemplate);
    //            comboBox.SetValue(ComboBox.ItemContainerStyleProperty, this.FindResource("ComboBoxItemBackgroundStyle"));
    //            comboBox.SetValue(ComboBox.IsDropDownOpenProperty, true);
    //            template.VisualTree = comboBox;
    //        }
    //        else if (type is AttributeType.Numeric numericAttribute)
    //        {
    //            FrameworkElementFactory textBox = new FrameworkElementFactory(typeof(TextBox));
    //            textBox.SetBinding(TextBox.TextProperty, new Binding($"[{attributeIndex}]")
    //            {
    //                Converter = (NominalValueStringConverter)this.FindResource("NominalValueStringConverter"),
    //            });
    //            template.VisualTree = textBox;
    //        }
    //        else
    //        {
    //            throw new Exception(); // imposible state
    //        }
    //        template.VisualTree.AddHandler(FrameworkElement.PreviewMouseRightButtonDownEvent,
    //                new MouseButtonEventHandler(DataGridCell_PreviewMouseRightButtonDown));
    //        return template;
    //    }

    //    #endregion // Data Templates

    //    #region Helpers

    //    private RelationAttribute NewRelationAttribute()
    //    {
    //        return this.Relation.AllMissingAttribute("New Attribute", new AttributeType.Numeric());
    //    }

    //    private int FindRelationAttributeIndex(RelationAttribute relationAttribute)
    //    {
    //        int index = -1;
    //        for (int i = 0; i < Relation.Attributes.Count; ++i)
    //        {
    //            if (Relation.Attributes[i].Equals(relationAttribute))
    //                index = i;
    //        }
    //        return index;
    //    }

    //    #endregion // Helper

    //    #region Event Handlers

    //    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (Relation == null)
    //        {
    //            dataGrid.Columns.Clear();
    //            dataGrid.DataContext = null;
    //        }
    //        else
    //        {
    //            InitializeRelationBinding();
    //        }
    //    }

    //    private void NewAttribute_Clicked(object sender, RoutedEventArgs e)
    //    {
    //        var index = this.Relation.Attributes.Count;
    //        var attribute = NewRelationAttribute();
    //        Relation.AddAttribute(attribute);
    //        InsertColumn(index, attribute);
    //    }

    //    private void InsertAttribute_Clicked(object sender, RoutedEventArgs e)
    //    {
    //        if (sender is MenuItem menuItem && menuItem.DataContext is RelationAttribute relationAttribute)
    //        {
    //            int index = FindRelationAttributeIndex(relationAttribute);
    //            if (index == -1)
    //            {
    //                throw new Exception($"invalid relation attribute {relationAttribute} passed to insert attribute clicked");
    //            }
    //            var attribute = NewRelationAttribute();
    //            Relation.InsertAttribute(index, attribute);
    //            InsertColumn(index, attribute);
    //        }
    //        else
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    private void RemoveAttribute_Clicked(object sender, RoutedEventArgs e)
    //    {
    //        if (sender is MenuItem menuItem && menuItem.DataContext is RelationAttribute relationAttribute)
    //        {
    //            int index = FindRelationAttributeIndex(relationAttribute);
    //            if (index == -1)
    //            {
    //                throw new Exception($"invalid relation attribute {relationAttribute} passed to insert attribute clicked");
    //            }
    //            Relation.RemoveAttribute(index);
    //            RemoveColumn(index);
    //        }
    //        else
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    private void NewTuple_Clicked(object sender, RoutedEventArgs e)
    //    {
    //        this.Relation.AddTuple();
    //    }

    //    private void InsertTuple_Clicked(object sender, RoutedEventArgs e)
    //    {
    //        if (sender is MenuItem menuItem)
    //        {
    //            var selectedItems = this.dataGrid.SelectedItems;
    //            if (selectedItems.Count == 0)
    //            {
    //                return;
    //            }
    //            var indecies = new List<int>();
    //            foreach (var selectedItem in selectedItems)
    //            {
    //                var tupleView = selectedItem as Model.TupleView;
    //                indecies.Add(tupleView.TupleIndex);
    //            }
    //            indecies.Sort((a, b) => -a.CompareTo(b));
    //            foreach (var index in indecies)
    //            {
    //                this.Relation.InsertTuple(index);
    //            }
    //        }
    //        else
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    private void RemoveTuple_Clicked(object sender, RoutedEventArgs e)
    //    {
    //        if (sender is MenuItem menuItem)
    //        {
    //            var selectedItems = this.dataGrid.SelectedItems;
    //            if (selectedItems.Count == 0)
    //            {
    //                return;
    //            }
    //            var indecies = new List<int>();
    //            foreach (var selectedItem in selectedItems)
    //            {
    //                var tupleView = selectedItem as Model.TupleView;
    //                indecies.Add(tupleView.TupleIndex);
    //            }
    //            indecies.Sort((a, b) => -a.CompareTo(b));
    //            foreach (var index in indecies)
    //            {
    //                this.Relation.RemoveTupleAt(index);
    //            }
    //        }
    //        else
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    private void DataGridCell_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    //    {
    //        var dependencyObject = (DependencyObject)sender;
    //        while (dependencyObject != null && !(dependencyObject is DataGridCell))
    //        {
    //            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
    //        }
    //        if (dependencyObject is DataGridCell dataGridCell)
    //        {
    //            if (dataGridCell.IsEditing)
    //            {
    //                dataGridCell.IsEditing = false;
    //            }
    //        }
    //        else
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    #endregion // Event Handlers
    //}
}
