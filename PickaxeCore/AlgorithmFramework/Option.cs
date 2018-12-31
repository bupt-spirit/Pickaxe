using Pickaxe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.AlgorithmFramework
{
    public class Option : NotifyPropertyChangedBase
    {
        #region Fields

        private string _name;
        private string _description;
        private object _value;

        #endregion

        public string Name {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public object Value
        {
            get => _value;
            set
            {
                if (value != null)
                {
                    if (!Type.IsAssignableFrom(value.GetType()))
                        throw new ArgumentException($"{Type} is not assignable from value type {value.GetType()}");
                }
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        public Type Type { get; private set; }


        #region Constructor

        public Option(string name, string description, Type type, object defaultValue)
        {
            Name = name;
            Description = description;
            Type = type;
            Value = defaultValue;
        }

        #endregion
    }
}
