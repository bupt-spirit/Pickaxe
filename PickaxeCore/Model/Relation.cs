using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Pickaxe.Model
{
    [Serializable]
    public class Relation : ObservableCollection<RelationAttribute>
    {
        #region Fields

        [NonSerialized]
        private TuplesView _tuplesView;

        #endregion

        #region Attributes

        public TuplesView TuplesView
        {
            get
            {
                if (_tuplesView == null)
                {
                    _tuplesView = new TuplesView(this);
                }
                return _tuplesView;
            }
        }

        #endregion

        #region Static functions

        private static bool CheckRelationAttributes(IEnumerable<RelationAttribute> collection)
        {
            var tupleCount = -1;
            foreach (var attribute in collection)
            {
                if (tupleCount == -1)
                {
                    tupleCount = attribute.Data.Count;
                }
                else if (tupleCount != attribute.Data.Count)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Constructors

        public Relation()
        {
        }

        public Relation(IEnumerable<RelationAttribute> collection) : base(collection)
        {
            if (!CheckRelationAttributes(collection))
            {
                Clear(); // prevent recovery into inconsistent state
                throw new ArgumentException("Invalid relation attributes");
            }
        }

        #endregion

        public void RebindInternalEvents()
        {
            foreach (var attribute in this)
            {
                attribute.DataCollectionChanged += Attribute_DataCollectionChanged;
                attribute.RebindInternalEvents();
            }
        }

        private void Attribute_DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                TuplesView[e.NewStartingIndex].OnContentChanged();
            }
        }

        #region Overrides

        protected override void ClearItems()
        {
            base.ClearItems();
        }

        protected override void InsertItem(int index, RelationAttribute item)
        {
            if (item.Data.Count != TuplesView.Count)
            {
                throw new ArgumentException("Invalid relation attributes");
            }
            item.Index = index;
            item.DataCollectionChanged += Attribute_DataCollectionChanged;
            base.InsertItem(index, item);
            for (var i = index + 1; i < Count; ++i)
            {
                this[i].Index = i;
            }
            foreach (var tupleView in TuplesView)
            {
                tupleView.OnContentChanged();
            }
        }

        protected override void RemoveItem(int index)
        {
            this[index].DataCollectionChanged -= Attribute_DataCollectionChanged;
            base.RemoveItem(index);
            for (var i = index; i < Count; ++i)
            {
                this[i].Index = i;
            }
            foreach (var tupleView in TuplesView)
            {
                tupleView.OnContentChanged();
            }
        }

        protected override void SetItem(int index, RelationAttribute item)
        {
            item.Index = index;
            item.DataCollectionChanged += Attribute_DataCollectionChanged;
            if (item.Data.Count != TuplesView.Count)
            {
                throw new ArgumentException("Invalid relation attributes");
            }

            base.SetItem(index, item);
            foreach (var tupleView in TuplesView)
            {
                tupleView.OnContentChanged();
            }
        }

        #endregion
    }
}
