using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.Converter;
using Pickaxe.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;

namespace Pickaxe
{
    /// <summary>
    /// Interaction logic for OptionDialog.xaml
    /// </summary>
    public partial class OptionDialog : Window
    {
        public OptionDialogViewModel ViewModel
        {
            get => (OptionDialogViewModel)DataContext;
        }

        public OptionDialog()
        {
            InitializeComponent();
        }

        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (e.OldValue != null)
            {
                var old = (OptionDialogViewModel)e.OldValue;
                old.PropertyChanged -= ViewModel_PropertyChanged;
            }
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;
                if (optionGrid != null) // prevent call OnOptionChanged when in InitializeComponent
                    OnOptionsChanged();
            }
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Options")
            {
                OnOptionsChanged();
            }
        }

        private void OnOptionsChanged()
        {
            ClearOptionGrid();
            var row = 0;
            foreach (var option in ViewModel.Options)
            {
                if (option.Type == typeof(IEnumerable<RelationAttribute>))
                {
                    option.Value = Enumerable.Empty<RelationAttribute>();
                }

                optionGrid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto,
                });

                var label = new Label
                {
                    Content = option.Name,
                    ToolTip = option.Description,
                };
                Grid.SetRow(label, row);
                Grid.SetColumn(label, 0);
                optionGrid.Children.Add(label);

                var input = CreateInputComponent(option);
                input.ToolTip = option.Description;
                Grid.SetRow(input, row);
                Grid.SetColumn(input, 1);
                optionGrid.Children.Add(input);

                row += 1;
            }
        }

        private FrameworkElement CreateInputComponent(Option option)
        {
            FrameworkElement element = null;
            if (option.Type == typeof(string))
            {
                var textBox = new TextBox
                {
                    DataContext = option,
                };
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, new Binding("Value"));
                element = textBox;
            }
            else if (option.Type == typeof(int))
            {
                var textBox = new TextBox
                {
                    DataContext = option,
                };
                var defaultValue = (int?)option.Value ?? 0;
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, new Binding("Value")
                {
                    Converter = new IntWithDefaultStringConverter(defaultValue),
                });
                element = textBox;
            }
            else if (option.Type == typeof(bool))
            {
                var checkBox = new CheckBox
                {
                    DataContext = option,
                };
                var defaultValue = (bool?)option.Value ?? false;
                BindingOperations.SetBinding(checkBox, CheckBox.IsCheckedProperty, new Binding("Value"));
                element = checkBox;
            }
            else if (option.Type == typeof(float))
            {
                var textBox = new TextBox
                {
                    DataContext = option,
                };
                var defaultValue = (float?)option.Value ?? 0.0f;
                option.Value = defaultValue;
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, new Binding("Value")
                {
                    Converter = new FloatWithDefaultStringConverter(defaultValue),
                });
                element = textBox;
            }
            else if (option.Type == typeof(RelationAttribute))
            {
                var listView = (ListView)FindResource("SingleSelectionAttributeListView");
                listView.Tag = option;
                BindingOperations.SetBinding(listView, ListView.SelectedItemProperty, new Binding("Tag.Value")
                {
                    RelativeSource = RelativeSource.Self,
                });
                element = listView;
            }
            else if (option.Type == typeof(IEnumerable<RelationAttribute>))
            {
                var listView = (ListView)FindResource("MultipleSelectionAttributeListView");
                listView.Tag = option;
                listView.SelectionChanged += AttributesCollectionListView_SelectionChanged;
                element = listView;
            }
            else
            {
                throw new NotImplementedException();
            }
            return element; // element is never null
        }

        private void AttributesCollectionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (ListView)sender;
            var option = (Option)listView.Tag;
            if (option.Type != typeof(IEnumerable<RelationAttribute>))
                throw new NotSupportedException();
            var relations = listView.SelectedItems.Cast<RelationAttribute>();
            option.Value = relations;
        }

        private void ClearOptionGrid()
        {
            optionGrid.Children.Clear();
            optionGrid.RowDefinitions.Clear();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
