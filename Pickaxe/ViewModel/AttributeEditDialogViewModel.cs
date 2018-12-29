using Pickaxe.Model;
using Pickaxe.Utility;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Pickaxe.ViewModel
{
    public class AttributeEditDialogViewModel : NotifyPropertyChangedBase
    {
        #region Fields

        public string _name;
        public AttributeType _attributeType;
        public AttributeType.Numeric _numericType;
        public AttributeType.Binary _binaryType;
        public AttributeType.Nominal _nominalType;

        public string _newNominalValue;
        public int _selectedNominalValueIndex;

        public ICommand _addNominalValue;
        public ICommand _removeNominalValue;
        public ICommand _insertNominalValue;
        public ICommand _replaceNominalValue;

        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public AttributeType AttributeType
        {
            get => _attributeType;
            set
            {
                _attributeType = value;
                OnPropertyChanged("AttributeType");
            }
        }

        public AttributeType.Numeric NumericType
        {
            get => _numericType;
            set
            {
                _numericType = value;
                OnPropertyChanged("NumericType");
            }
        }

        public AttributeType.Binary BinaryType
        {
            get => _binaryType;
            set
            {
                _binaryType = value;
                OnPropertyChanged("BinaryType");
            }
        }

        public AttributeType.Nominal NominalType
        {
            get => _nominalType;
            set
            {
                _nominalType = value;
                OnPropertyChanged("NominalType");
            }
        }

        public string NewNominalValue
        {
            get => _newNominalValue;
            set
            {
                _newNominalValue = value;
                OnPropertyChanged("NewNominalValue");
            }
        }
        public int SelectedNominalValueIndex
        {
            get => _selectedNominalValueIndex;
            set
            {
                _selectedNominalValueIndex = value;
                OnPropertyChanged("SelectedNominalValueIndex");
            }
        }

        public ICommand AddNominalValue
        {
            get => _addNominalValue ?? (
                _addNominalValue = new RelayCommand(
                    parameter => AttributeType is AttributeType.Nominal && NewNominalValue != String.Empty,
                    parameter =>
                    {
                        if (AttributeType is AttributeType.Nominal nominal)
                        {
                            nominal.NominalLabels.Add(NewNominalValue);
                            NewNominalValue = String.Empty;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }));
        }

        public ICommand RemoveNominalValue
        {
            get => _removeNominalValue ?? (
                _removeNominalValue = new RelayCommand(
                    parameter => AttributeType is AttributeType.Nominal && SelectedNominalValueIndex != -1,
                    parameter =>
                    {
                        if (AttributeType is AttributeType.Nominal nominal)
                        {
                            nominal.NominalLabels.RemoveAt(SelectedNominalValueIndex);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }));
        }

        public ICommand InsertNominalValue
        {
            get => _insertNominalValue ?? (
                _insertNominalValue = new RelayCommand(
                    parameter => AttributeType is AttributeType.Nominal && SelectedNominalValueIndex != -1 && NewNominalValue != String.Empty,
                    parameter =>
                    {
                        if (AttributeType is AttributeType.Nominal nominal)
                        {
                            nominal.NominalLabels.Insert(SelectedNominalValueIndex, NewNominalValue);
                            NewNominalValue = String.Empty;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }));
        }

        public ICommand ReplaceNominalValue
        {
            get => _replaceNominalValue ?? (
                _replaceNominalValue = new RelayCommand(
                    parameter => AttributeType is AttributeType.Nominal && SelectedNominalValueIndex != -1 && NewNominalValue != String.Empty,
                    parameter =>
                    {
                        if (AttributeType is AttributeType.Nominal nominal)
                        {
                            nominal.NominalLabels[SelectedNominalValueIndex] = NewNominalValue;
                            NewNominalValue = String.Empty;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }));
        }

        #endregion

        #region Constructor

        public AttributeEditDialogViewModel()
        {
            Name = "New Attribute";
            NumericType = new AttributeType.Numeric();
            BinaryType = new AttributeType.Binary();
            NominalType = new AttributeType.Nominal();
            NewNominalValue = String.Empty;
            SelectedNominalValueIndex = -1;
        }

        #endregion
    }
}
