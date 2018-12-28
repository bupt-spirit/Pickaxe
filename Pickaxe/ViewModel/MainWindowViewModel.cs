using Microsoft.Win32;
using Pickaxe.Model;
using Pickaxe.Utility;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;

namespace Pickaxe.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {//能够通知property的更改，relation：表
        private Relation _relation;
        private string _fileName;

        private ICommand _newRelation;
        private ICommand _openRelation;
        private ICommand _saveRelation;
        private ICommand _saveAsRelation;

        public Relation Relation
        {
            get => _relation;//语法糖
            set
            {
                _relation = value;//value关键字，是set时的右值
                OnPropertyChanged("Relation");//告知change了
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
            get => _newRelation ?? (//??:不是null返回前面的东西，是null返回后面
                _newRelation = new RelayCommand(
                    parameter => Relation.Count != 0,
                    parameter =>
                    {
                        Relation = new Relation();
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

        #region Static members

        private static readonly string FILE_FILTER = "Pickaxe files (*.pickaxe)|*.pickaxe|All files (*.*)|*.*";
        private static readonly IFormatter FORMATTER = new BinaryFormatter();

        #endregion
    }
}
