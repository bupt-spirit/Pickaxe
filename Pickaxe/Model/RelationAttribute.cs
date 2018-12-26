using Pickaxe.Utility;
using System;
using System.Collections.ObjectModel;

namespace Pickaxe.Model
{
    [Serializable]
    public class RelationAttribute : NotifyPropertyChangedBase
    {
        private AttributeType _type;
        private ObservableCollection<Value> _data;
        private string _name;

        public AttributeType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        public ObservableCollection<Value> Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged("Data");
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public RelationAttribute()
        {
        }

        public RelationAttribute(string name, AttributeType type, ObservableCollection<Value> data)
        {
            Name = name;
            Type = type;
            Data = data;
        }
    }
}
