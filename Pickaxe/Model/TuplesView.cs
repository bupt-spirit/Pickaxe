using System;
using System.Collections.ObjectModel;

namespace Pickaxe.Model
{
    public class TuplesView : ObservableCollection<TupleView>
    {
        #region Fields

        private Relation relation;

        #endregion

        #region Constructors

        public TuplesView(Relation relation)
        {
            this.relation = relation;
            if (relation.Count != 0)
            {
                for (var i = 0; i < this.relation[0].Data.Count; ++i)
                    // call base InsertItem to bypass override
                    base.InsertItem(i, new TupleView(relation, i));
            }
        }

        #endregion

        #region Overrides

        protected override void ClearItems()
        {
            base.ClearItems();
            foreach (var attribute in relation)
                attribute.Data.Clear();
        }

        protected override void InsertItem(int index, TupleView item)
        {
            if (!item.IsDetached()) {
                throw new ArgumentException("not detached tuple view");
            }
            foreach (var attribute in relation)
                attribute.Data.Insert(index, Value.MISSING);
            base.InsertItem(index, new TupleView(relation, index));
            for (var i = index + 1; i < Count; ++i)
                SetItem(i, TupleView.Detached);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            for (var i = index; i < Count; ++i)
                SetItem(i, TupleView.Detached);
            foreach (var attribute in relation)
                attribute.Data.RemoveAt(index);
        }

        protected override void SetItem(int index, TupleView item)
        {
            if (!item.IsDetached()) throw new ArgumentException("not detached tuple view");
            base.SetItem(index, new TupleView(relation, index));
        }

        #endregion
    }
}
