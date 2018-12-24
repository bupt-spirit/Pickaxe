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

namespace Pickaxe
{
    /// <summary>
    /// Interaction logic for OptionDialog.xaml
    /// </summary>
    public partial class OptionDialog : Window
    {
        private string Description { get; set; }

        public OptionDialog(string title, string description)
        {
            InitializeComponent();
            this.DataContext = this;
            this.Title = title;
            this.Description = description;
        }
    }
}
