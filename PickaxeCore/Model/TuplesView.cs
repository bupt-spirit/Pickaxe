using System;
using System.Collections.ObjectModel;

namespace Pickaxe.Model
{
    public class TuplesView : ObservableCollection<TupleView>
    {
        #region Fields

        private readonly Relation _relation;

        #endregion

        #region Constructors

        public TuplesView(Relation relation)
        {
            _relation = relation;
            if (relation.Count != 0)
            {
                for (var i = 0; i < this._relation[0].Data.Count; ++i)
                    // call base InsertItem to bypass override
                    base.InsertItem(i, new TupleView(relation, i));
            }
        }

        #endregion

        #region Overrides

        protected override void ClearItems()
        {
            base.ClearItems();
            foreach (var attribute in _relation)
                attribute.Data.Clear();
        }

        protected override void InsertItem(int index, TupleView item)
        {
            if (!item.IsDetached()) {
                throw new ArgumentException("not detached tuple view");
            }
            foreach (var attribute in _relation)
                attribute.Data.Insert(index, Value.MISSING);
            base.InsertItem(index, new TupleView(_relation, index));
            for (var i = index + 1; i < Count; ++i)
                this[i].SetTupleIndexWithoutUpdate(i);
        }

        protected override void RemoveItem(int index)
        {
            foreach (var attribute in _relation)
                attribute.Data.RemoveAt(index);
            base.RemoveItem(index);
            for (var i = index; i < Count; ++i)
                this[i].SetTupleIndexWithoutUpdate(i);
        }

        protected override void SetItem(int index, TupleView item)
        {
            if (!item.IsDetached()) throw new ArgumentException("not detached tuple view");
            base.SetItem(index, new TupleView(_relation, index));
        }

        #endregion
    }
}
