using DrWPF.Windows.Data;
using Pickaxe.Utility;
using System.Collections;
using System.Collections.Generic;

namespace Pickaxe.Model
{
    public class StatisticView : NotifyPropertyChangedBase
    {
        #region Fields

        private RelationAttribute _relationAttribute;
        private int _missing;
        private Dictionary<Value, int> _distinctValues;
        private Value _min;
        private Value _max;

        #endregion

        #region Properties

        public RelationAttribute RelationAttribute
        {
            get => _relationAttribute;
            private set
            {
                _relationAttribute = value;
                OnPropertyChanged("RelationAttribute");
            }
        }

        public int Missing
        {
            get => _missing;
            private set
            {
                _missing = value;
                OnPropertyChanged("Missing");
            }
        }

        public Dictionary<Value, int> DistinctValues
        {
            get => _distinctValues;
            private set {
                _distinctValues = value;
                OnPropertyChanged("DistinctValues");
            }
        }

        public Value Min
        {
            get => _min;
            private set
            {
                _min = value;
                OnPropertyChanged("Min");
            }
        }

        public Value Max
        {
            get => _max;
            private set
            {
                _max = value;
                OnPropertyChanged("Max");
            }
        }

        #endregion

        #region Constructors

        public StatisticView(RelationAttribute attribute)
        {
            RelationAttribute = attribute;
            //DistinctValues = new ObservableSortedDictionary<Value, int>(
            //    Comparer<DictionaryEntry>.Create((x, y) =>
            //    {
            //        var xKey = (Value)x.Key;
            //        var yKey = (Value)y.Key;
            //        return xKey.CompareTo(yKey);
            //    }));
            DistinctValues = new Dictionary<Value, int>();
            // Not refresh
            //Refresh();
        }

        #endregion

        #region Members

        protected void OnAdded(Value item)
        {
            // handle missing value
            if (item.IsMissing())
                Missing += 1;
            // handle distinct value
            if (!item.IsMissing())
            {
                if (DistinctValues.ContainsKey(item))
                    DistinctValues[item] += 1;
                else
                    DistinctValues.Add(item, 1);
            }
            // handle min max
            if (!item.IsMissing())
            {
                if (DistinctValues.Count != 0)
                {
                    if (Max.IsMissing() || item > Max)
                        Max = item;
                    if (Min.IsMissing() || item < Min)
                        Min = item;
                }
            }
        }

        public void Reset()
        {
            Min = Value.MISSING;
            Max = Value.MISSING;
            Missing = 0;
            DistinctValues.Clear();
        }

        public void Refresh()
        {
            Reset();
            foreach (var value in RelationAttribute.Data)
            {
                OnAdded(value);
            }
        }
        #endregion

    }
}
