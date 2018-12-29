using Pickaxe.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Model
{
    public class StatisticView : NotifyPropertyChangedBase
    {
        #region Fields

        public int _missing;
        public Dictionary<Value, int> distinctValues;

        #endregion

        #region Properties

        public int Missing
        {
            get => _missing;
            set
            {
                _missing = value;
                OnPropertyChanged("Missing");
            }
        }

        public int DistinctValue
        {
            get => distinctValues.Count;
        }

        #endregion

        #region Constructors

        public StatisticView(RelationAttribute attribute)
        {
            distinctValues = new Dictionary<Value, int>();
            attribute.Data.CollectionChanged += Data_CollectionChanged;
            foreach (var value in attribute.Data)
            {
                OnAdded(value);
            }
        }

        #endregion

        #region Event handler

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var newItem = (Value)e.NewItems[0];
                        OnAdded(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        var oldItem = (Value)e.OldItems[0];
                        OnRemoved(oldItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        var oldItem = (Value)e.OldItems[0];
                        var newItem = (Value)e.NewItems[0];
                        OnRemoved(oldItem);
                        OnAdded(newItem);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    OnReset();
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        #endregion

        #region Members

        protected void OnAdded(Value item)
        {
            // handle missing value
            if (item.IsMissing())
                Missing += 1;
            // handle distinct value
            if (distinctValues.ContainsKey(item))
            {
                distinctValues[item] += 1;
            }
            else
            {
                distinctValues.Add(item, 1);
                OnPropertyChanged("DistinctValue");
            }
        }

        protected void OnRemoved(Value item)
        {
            // handle missing value
            if (item.IsMissing())
                Missing -= 1;
            // handle distinct value
            if (distinctValues.ContainsKey(item))
            {
                if (distinctValues[item] == 1)
                {
                    distinctValues.Remove(item);
                    OnPropertyChanged("DistinctValue");
                }
                else
                {
                    distinctValues[item] -= 1;
                }
            }
            else
            {
                throw new ArgumentException("invalid key on removed");
            }
        }

        protected void OnReset()
        {
            // handle missing value
            Missing = 0;
            // handle distinct value
            distinctValues.Clear();
            OnPropertyChanged("DistinctValue");
        }
        #endregion

    }
}
