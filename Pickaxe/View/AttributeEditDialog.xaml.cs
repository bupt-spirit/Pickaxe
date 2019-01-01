using Pickaxe.Model;
using Pickaxe.ViewModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Pickaxe.View
{
    /// <summary>
    /// Interaction logic for AttributeEditDialog.xaml
    /// </summary>
    public partial class AttributeEditDialog : Window
    {
        public AttributeEditDialogViewModel ViewModel => (AttributeEditDialogViewModel)DataContext;

        public AttributeEditDialog()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (labelSettingGrid == null)
                return;
            UIElement content;
            var selectedAttributeType = (AttributeType)((ComboBoxItem)comboBox.SelectedItem).Content;
            if (selectedAttributeType is AttributeType.Numeric)
            {
                content = (UIElement)FindResource("NumericLabelSetting");
            }
            else if (selectedAttributeType is AttributeType.Binary)
            {
                content = (UIElement)FindResource("BinaryLabelSetting");
            }
            else if (selectedAttributeType is AttributeType.Nominal)
            {
                content = (UIElement)FindResource("NominalLabelSetting");
            }
            else
            {
                throw new NotImplementedException();
            }
            Grid.SetRow(content, 0);
            Grid.SetColumn(content, 0);
            labelSettingGrid.Children.Clear();
            labelSettingGrid.Children.Add(content);
            ViewModel.AttributeType = selectedAttributeType;
        }

        // Validate all dependency objects in a window
        private static bool IsValid(DependencyObject node)
        {
            // Check if dependency object was passed
            if (node != null)
            {
                // Check if dependency object is valid.
                // NOTE: Validation.GetHasError works for controls that have validation rules attached
                bool isValid = !Validation.GetHasError(node);
                if (!isValid)
                {
                    // If the dependency object is invalid, and it can receive the focus,
                    // set the focus
                    if (node is IInputElement element) Keyboard.Focus(element);
                    return false;
                }
            }

            // If this dependency object is valid, check all child dependency objects
            foreach (object subNode in LogicalTreeHelper.GetChildren(node))
            {
                if (subNode is DependencyObject)
                {
                    // If a child dependency object is invalid, return false immediately,
                    // otherwise keep checking
                    if (IsValid((DependencyObject)subNode) == false) return false;
                }
            }

            // All dependency objects are valid
            return true;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsValid(this))
            {
                DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedItem = defaultComboBoxItem;
        }
    }
}
