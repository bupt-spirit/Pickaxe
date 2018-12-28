using Microsoft.Win32;
using Pickaxe.Algorithm;
using Pickaxe.Model;
using Pickaxe.Utility;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;

namespace Pickaxe.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private Relation _relation;
        private string _fileName;
        private int _binNumber;

        private ICommand _newRelation;
        private ICommand _openRelation;
        private ICommand _reloadRelation;
        private ICommand _saveRelation;
        private ICommand _saveAsRelation;
        private ICommand _addAttribute;
        private ICommand _insertAttribute;
        private ICommand _removeAttribute;
        private ICommand _equidistanceDiscreteAttribute;

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

        public MainWindowViewModel()
        {
            Relation = new Relation();
            FileName = null;
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
                        var data = new ObservableCollection<Value>();
                        data.Resize(Relation.TuplesView.Count, Value.MISSING);
                        Relation.Add(
                            new RelationAttribute("New Attribute", new AttributeType.Numeric(), data)
                            );
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
                        var data = new ObservableCollection<Value>();
                        data.Resize(Relation.TuplesView.Count, Value.MISSING);
                        Relation.Insert((int)parameter,
                            new RelationAttribute("New Attribute", new AttributeType.Numeric(), data)
                            );
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

        public ICommand EquidistanceDiscreteAttribute
        {
            get => _equidistanceDiscreteAttribute ?? (
                _equidistanceDiscreteAttribute = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        if (BinNumber == 0)
                            return false;
                        return parameter is RelationAttribute;
                    },
                    parameter =>
                    {
                        var attribute = parameter as RelationAttribute;
                        EquidistanceDiscrete.Run(attribute, BinNumber);
                    })
                );
        }

        #region Static members

        private static readonly string FILE_FILTER = "Pickaxe files (*.pickaxe)|*.pickaxe|All files (*.*)|*.*";
        private static readonly IFormatter FORMATTER = new BinaryFormatter();

        #endregion
    }
}
