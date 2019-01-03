using OxyPlot.Series;
using Pickaxe.Model;
using Pickaxe.Utility;
using Pickaxe.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Pickaxe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VisualizeSeries _visualizeSeries;

        protected MainWindowViewModel ViewModel
        {
            get => (MainWindowViewModel)DataContext;
        }

        public VisualizeSeries VisualizeSeries
        {
            get => _visualizeSeries ?? (_visualizeSeries = new VisualizeSeries());
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

        private void SelectorClearButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var selector = (Selector)button.Tag;
            selector.SelectedItem = null;
        }
    }

    public class VisualizeSeries : NotifyPropertyChangedBase
    {
        private float _jitter;
        private RelationAttribute _xAttribute;
        private RelationAttribute _yAttribute;
        private RelationAttribute _colorAttribute;

        public float Jitter
        {
            get => _jitter;
            set
            {
                _jitter = value;
                OnPropertyChanged("Jitter");
                OnPropertyChanged("Points");
            }
        }
        public RelationAttribute XAttribute
        {
            get => _xAttribute;
            set
            {
                _xAttribute = value;
                OnPropertyChanged("XAttribute");
                OnPropertyChanged("Points");
            }
        }
        public RelationAttribute YAttribute
        {
            get => _yAttribute;
            set
            {
                _yAttribute = value;
                OnPropertyChanged("YAttribute");
                OnPropertyChanged("Points");
            }
        }
        public RelationAttribute ColorAttribute
        {
            get => _colorAttribute;
            set
            {
                _colorAttribute = value;
                OnPropertyChanged("ColorAttribute");
                OnPropertyChanged("Points");
            }
        }

        public IEnumerable<ScatterPoint> Points
        {
            get
            {
                var xCount = XAttribute == null ? 0 : XAttribute.Data.Count;
                var yCount = YAttribute == null ? 0 : YAttribute.Data.Count;
                if (xCount == 0 && yCount == 0)
                {
                    return Enumerable.Empty<ScatterPoint>();
                }
                else
                {
                    var count = xCount > yCount ? xCount : yCount;

                    var r = new Random(0);

                    if (XAttribute != null)
                        XAttribute.StatisticView.Refresh();
                    if (YAttribute != null)
                        YAttribute.StatisticView.Refresh();
                    if (ColorAttribute != null)
                        ColorAttribute.StatisticView.Refresh();

                    var xs = JitteredAttributeData(count, XAttribute, r);
                    var ys = JitteredAttributeData(count, YAttribute, r);
                    var colors = NormalizedAttributeData(count, ColorAttribute);
                    return Enumerable
                        .Zip(xs, ys, (x, y) => (x, y))
                        .Zip(colors, (xy, color) => (xy, color))
                        .Select(tuple => new ScatterPoint(
                            tuple.xy.x,
                            tuple.xy.y,
                            double.NaN,
                            tuple.color
                        ));
                }
            }
        }

        public IEnumerable<double> JitteredAttributeData(int count, RelationAttribute attribute, Random r)
        {
            if (attribute == null) return Enumerable.Repeat(0.0, count);
            return attribute.Data.Select((data) =>
            {
                if (data.IsMissing()) return 0;
                var randomFactor = r.NextDouble() - 0.5;
                var result = data * (randomFactor * Jitter + 1);
                return result;
            });
        }

        public IEnumerable<double> NormalizedAttributeData(int count, RelationAttribute attribute)
        {
            if (attribute == null) return Enumerable.Repeat(0.0, count);
            var min = attribute.StatisticView.Min;
            var max = attribute.StatisticView.Max;
            if (max.IsMissing() || min.IsMissing()) return Enumerable.Repeat(0.0, count);
            var range = max - min;
            return attribute.Data.Select((data) =>
            {
                if (data.IsMissing()) return 0.0;
                return (data - min) / range;
            });
        }

        public ScatterPoint GetScatterPoint(int n)
        {
            return new ScatterPoint(
                XAttribute != null ? XAttribute.Data[n] : double.NaN,
                YAttribute != null ? YAttribute.Data[n] : double.NaN,
                double.NaN,
                ColorAttribute != null ? ColorAttribute.Data[n] : double.NaN
            );
        }
    }
}
