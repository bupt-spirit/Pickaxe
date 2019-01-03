using Pickaxe.Utility;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Pickaxe.Model
{
    [Serializable]
    public abstract class AttributeType : NotifyPropertyChangedBase
    {
        private AttributeType() { }

        public abstract bool ValidateValue(Value value);

        public bool ValidateValueWithMissing(Value value) {
            return value.IsMissing() || this.ValidateValue(value);
        }

        public abstract string ValueToString(Value value);

        [Serializable]
        public class Numeric : AttributeType
        {
            public override bool ValidateValue(Value value) => !value.IsMissing();
            public override string ValueToString(Value value) => value.ToString();
        }

        [Serializable]
        public class Binary : AttributeType
        {
            private string _trueLabel;
            private string _falseLabel;

            public override bool ValidateValue(Value value)
            {
                return !value.IsMissing() && (value == Value.ToValue(true) || value == Value.ToValue(false));
            }

            public override string ValueToString(Value value)
            {
                if (value.IsMissing())
                    return value.ToString();
                else if (value == Value.ToValue(true))
                    return TrueLabel;
                else if (value == Value.ToValue(false))
                    return FalseLabel;
                else
                    throw new ArgumentException("invalid binary value");
            }

            public string TrueLabel {
                get => _trueLabel;
                set {
                    _trueLabel = value;
                    OnPropertyChanged("TrueLabel");
                }
            }
            public string FalseLabel {
                get => _falseLabel;
                set
                {
                    _falseLabel = value;
                    OnPropertyChanged("FalseLabel");
                }
            }

            public Binary() : this("true", "false")
            {
            }

            public Binary(string trueLabel, string falseLabel)
            {
                this.TrueLabel = trueLabel;
                this.FalseLabel = falseLabel;
            }
        }

        [Serializable]
        public class Nominal : AttributeType
        {
            public override bool ValidateValue(Value value)
            {
                if (value.IsMissing())
                    return false;
                try
                {
                    var index = ValueToIndex(value);
                    return index < NominalLabels.Count;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }

            public override string ValueToString(Value value)
            {
                if (value.IsMissing())
                    return value.ToString();
                else
                    return NominalLabels[ValueToIndex(value)];
            }

            public Nominal()
            {
                NominalLabels = new ObservableCollection<string>();
                _validValues = new ObservableCollection<Value>
                {
                    Value.MISSING
                };
            }

            public Nominal(ObservableCollection<string> nominalValues)
            {
                NominalLabels = nominalValues;
                _validValues = new ObservableCollection<Value>();
                for (int i = 0; i < NominalLabels.Count; ++i)
                    _validValues.Add(Value.ToValue(i));
                _validValues.Add(Value.MISSING);
            }

            private void NominalLabels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (NominalLabels.Count + 1 > _validValues.Count)
                {
                    _validValues.RemoveAt(_validValues.Count - 1); // remove last value
                    for (int i = _validValues.Count; i < NominalLabels.Count; ++i)
                    {
                        _validValues.Add(Value.ToValue(i));
                    }
                    _validValues.Add(Value.MISSING);
                }
                else if (NominalLabels.Count + 1 < _validValues.Count)
                {
                    _validValues.Resize(NominalLabels.Count, Value.MISSING);
                    _validValues.Add(Value.MISSING);
                }
            }

            private ObservableCollection<string> _nominalLabels;
            private ObservableCollection<Value> _validValues;
            private ReadOnlyObservableCollection<Value> _validValuesReadOnly;

            public ReadOnlyObservableCollection<Value> ValidValues
            {
                get
                {
                    if (_validValuesReadOnly == null)
                    {
                        _validValuesReadOnly = new ReadOnlyObservableCollection<Value>(_validValues);
                    }
                    return _validValuesReadOnly;
                }
            }
            public ObservableCollection<string> NominalLabels
            {
                get => _nominalLabels;
                set {
                    _nominalLabels = value;
                    for (int i = 0; i < _nominalLabels.Count; ++i)
                        _validValues.Add(Value.ToValue(i));
                    _validValues.Add(Value.MISSING);
                    NominalLabels.CollectionChanged += NominalLabels_CollectionChanged;
                    OnPropertyChanged("NominalLabels");
                }
            }

            public static int ValueToIndex(Value value)
            {
                var inDecimal = Math.Floor(value);
                if (inDecimal != value)
                {
                    throw new System.ArgumentException(
                        $"nominal value {value} is not an integer"
                        );
                }
                var inInt32 = Convert.ToInt32(inDecimal);
                if (inInt32 != inDecimal)
                {
                    throw new System.ArgumentException(
                        $"nominal value {value} is too large"
                        );
                }
                if (inInt32 < 0)
                {
                    throw new System.ArgumentException(
                        $"nominal value {value} is less than zero"
                        );
                }
                return inInt32;
            }
        }
    }
}
