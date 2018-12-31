using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Pickaxe.AlgorithmFramework;
using Pickaxe.AlgorithmStandalone.Preprocess;
using Pickaxe.Model;
using Pickaxe.Utility;
using Pickaxe.Utility.ListExtension;
using Pickaxe.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Pickaxe.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private Relation _relation;
        private string _fileName;
        private int _binNumber;

        private int _histogramBinNumber;
        private SeriesCollection _histogramSeriesCollection;
        private ObservableCollection<string> _histogramLabels;

        private AlgorithmDiscovery _algorithmDiscovery;

        private ICommand _newRelation;
        private ICommand _openRelation;
        private ICommand _reloadRelation;
        private ICommand _saveRelation;
        private ICommand _saveAsRelation;

        private ICommand _addAttribute;
        private ICommand _insertAttribute;
        private ICommand _removeAttribute;

        private ICommand _refreshStatisticsView;

        private ICommand _runAlgorithm;


        public int BinNumber
        {
            get => _binNumber;
            set
            {
                _binNumber = value;
                OnPropertyChanged("BinNumber");
            }
        }

        public Relation Relation
        {
            get => _relation;
            set
            {
                _relation = value;
                OnPropertyChanged("Relation");
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public int HistogramBinNumber
        {
            get => _histogramBinNumber;
            set
            {
                _histogramBinNumber = value;
                OnPropertyChanged("HistogramBinNumber");
            }
        }

        public SeriesCollection HistogramSeriesCollection
        {
            get => _histogramSeriesCollection;
            set
            {
                _histogramSeriesCollection = value;
                OnPropertyChanged("HistogramSeriesCollection");
            }
        }

        public ObservableCollection<string> HistogramLabels
        {
            get => _histogramLabels;
            set
            {
                _histogramLabels = value;
                OnPropertyChanged("HistogramLabels");
            }
        }

        public AlgorithmDiscovery AlgorithmDiscovery {
            get => _algorithmDiscovery ?? (
                _algorithmDiscovery = new AlgorithmDiscovery());
        }

        public MainWindowViewModel()
        {
            Relation = new Relation();
            FileName = null;
            HistogramBinNumber = 10;
            HistogramSeriesCollection = new SeriesCollection();
            HistogramLabels = new ObservableCollection<string>();
        }

        public ICommand NewRelation
        {
            get => _newRelation ?? (
                _newRelation = new RelayCommand(
                    parameter => Relation.Count != 0,
                    parameter =>
                    {
                        Relation = new Relation();
                        FileName = null;
                    })
                );
        }

        public ICommand OpenRelation
        {
            get => _openRelation ?? (
                _openRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var openFileDialog = new OpenFileDialog
                        {
                            Filter = FILE_FILTER
                        };
                        if (openFileDialog.ShowDialog() == true)
                        {
                            using (var stream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                try
                                {
                                    var obj = FORMATTER.Deserialize(stream);
                                    if (obj is Relation relation)
                                    {
                                        relation.RebindInternalEvents();
                                        Relation = relation;
                                        FileName = openFileDialog.FileName;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Selected file is not a valid Pickaxe file, please select another file",
                                            "Invalid Pickaxe file");
                                    }
                                }
                                catch (SerializationException)
                                {
                                    MessageBox.Show("Selected file is corrupted, please select another file",
                                        "Invalid Pickaxe file");
                                }
                            }
                        }
                        // Do nothing
                    })
                );
        }

        public ICommand ReloadRelation
        {
            get => _reloadRelation ?? (
                _reloadRelation = new RelayCommand(
                    parameter => FileName != null,
                    parameter =>
                    {
                        using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
                        {
                            var obj = FORMATTER.Deserialize(stream);
                            if (obj is Relation relation)
                            {
                                relation.RebindInternalEvents();
                                Relation = relation;
                            }
                            else
                            {
                                throw new Exception($"Failed to reload file {FileName}");
                            }
                        }
                    })
                );
        }

        public ICommand SaveRelation
        {
            get => _saveRelation ?? (
                _saveRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        if (FileName == null)
                        {
                            var saveFileDialog = new SaveFileDialog
                            {
                                Filter = FILE_FILTER
                            };
                            if (saveFileDialog.ShowDialog() == true)
                                FileName = saveFileDialog.FileName;
                            else
                                return; // Do nothing
                        }
                        using (var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                        {
                            FORMATTER.Serialize(stream, Relation);
                        }
                    })
                );
        }

        public ICommand SaveAsRelation
        {
            get => _saveAsRelation ?? (
                _saveAsRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var saveFileDialog = new SaveFileDialog
                        {
                            Filter = FILE_FILTER
                        };
                        if (saveFileDialog.ShowDialog() == true)
                        {
                            FileName = saveFileDialog.FileName;
                            using (var stream = new FileStream(FileName, FileMode.Create, FileAccess.Write))
                            {
                                FORMATTER.Serialize(stream, Relation);
                            }
                        }
                    })
                );
        }

        public ICommand AddAttribute
        {
            get => _addAttribute ?? (
                _addAttribute = new RelayCommand(
                    parameter =>
                    {
                        return Relation != null;
                    },
                    parameter =>
                    {
                        var attribute = ShowDialogForNewAttribute();
                        if (attribute != null)
                        {
                            Relation.Add(attribute);
                        }
                    })
                );
        }

        public ICommand InsertAttribute
        {
            get => _insertAttribute ?? (
                _insertAttribute = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        return parameter is int;
                    },
                    parameter =>
                    {
                        var attribute = ShowDialogForNewAttribute();
                        if (attribute != null)
                        {
                            Relation.Insert((int)parameter, attribute);
                        }
                    })
                );
        }

        public ICommand RemoveAttribute
        {
            get => _removeAttribute ?? (
                _removeAttribute = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        return (int)parameter < Relation.Count;
                    },
                    parameter =>
                    {
                        Relation.RemoveAt((int)parameter);
                    })
                );
        }

        public ICommand RefreshStatisticsView
        {
            get => _refreshStatisticsView ?? (
                _refreshStatisticsView = new RelayCommand(
                    parameter =>
                    {
                        var attribute = (RelationAttribute)parameter;
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        return true;
                    },
                    parameter =>
                    {
                        var attribute = (RelationAttribute)parameter;
                        attribute.StatisticView.Refresh();

                        // Update Histogram
                        var max = attribute.StatisticView.Max;
                        var min = attribute.StatisticView.Min;
                        HistogramLabels.Clear();
                        HistogramSeriesCollection.Clear();
                        if (!max.IsMissing() && !min.IsMissing())
                        {
                            var binSize = (max - min) / HistogramBinNumber;

                            for (var i = 0; i < HistogramBinNumber; ++i)
                                HistogramLabels.Add($"{min + i * binSize} - {min + (i + 1) * binSize}");

                            var bins = new List<int>();
                            bins.Resize(HistogramBinNumber, 0);
                            foreach (var value in attribute.Data)
                            {
                                if (!value.IsMissing())
                                {
                                    int bin;
                                    if (value != max)
                                    {
                                        bin = (int)Math.Floor((value - min) / binSize);
                                    }
                                    else
                                    {
                                        bin = HistogramBinNumber - 1;
                                    }
                                    bins[bin] += 1;
                                }
                            }
                            HistogramSeriesCollection.Add(new ColumnSeries
                            {
                                Title = attribute.Name,
                                Values = new ChartValues<int>(bins),
                            });
                        }
                    })
                );
        }

        public ICommand RunAlgorithm
        {
            get => _runAlgorithm ?? (
                _runAlgorithm = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        var parameters = (object[])parameter;
                        var algorithm = (IAlgorithm)parameters[0];
                        if (algorithm == null)
                            return false;
                        return true;
                    },
                    parameter =>
                    {
                        var parameters = (object[])parameter;
                        var algorithm = (IAlgorithm)parameters[0];
                        var output = (TextBox)parameters[1];
                        algorithm.Output = output;

                        var dialog = new OptionDialog();
                        dialog.ViewModel.Relation = Relation;
                        dialog.ViewModel.Name = algorithm.Name;
                        dialog.ViewModel.Description = algorithm.Description;
                        dialog.ViewModel.Options = algorithm.Options;
                        if (dialog.ShowDialog() == true)
                        {
                            output.Clear();
                            algorithm.Run();
                        }
                    })
                );
        }

        #region Static members

        private static readonly string FILE_FILTER = "Pickaxe files (*.pickaxe)|*.pickaxe|All files (*.*)|*.*";
        private static readonly IFormatter FORMATTER = new BinaryFormatter();

        #endregion

        #region Methods

        private RelationAttribute ShowDialogForNewAttribute()
        {
            var dialog = new AttributeEditDialog();
            if (dialog.ShowDialog() == true)
            {
                var data = new ObservableCollection<Value>();
                data.Resize(Relation.TuplesView.Count, Value.MISSING);
                return new RelationAttribute(dialog.ViewModel.Name, dialog.ViewModel.AttributeType, data);
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
