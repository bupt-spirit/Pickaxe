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

        public static bool CheckRelationAttributes(IEnumerable<RelationAttribute> collection)
        {
            int tupleCount = -1;
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

        public Relation() : base()
        {
        }

        public Relation(IEnumerable<RelationAttribute> collection) : base(collection)
        {
            if (!CheckRelationAttributes(collection))
            {
                Clear(); // prevent recovery into inconsist state
                throw new ArgumentException("Invalid relation attributes");
            }
        }

        #endregion

        public void RebindInternalEvents()
        {
            foreach (var attribute in this)
                attribute.Data.ListChanged += Data_ListChanged;
        }

        private void Data_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged)
            {
                var index = e.NewIndex;
                TuplesView[index].OnContentChanged();
            }
        }

        #region Overrides

        protected override void ClearItems()
        {
            base.ClearItems();
            foreach (var tupleView in TuplesView)
            {
                tupleView.OnContentChanged();
            }
        }

        protected override void InsertItem(int index, RelationAttribute item)
        {
            if (item.Data.Count != TuplesView.Count)
            {
                throw new ArgumentException("Invalid relation attributes");
            }
            item.Index = index;
            item.Data.ListChanged += Data_ListChanged;
            foreach (var tupleView in TuplesView)
            {
                tupleView.OnContentChanged();
            }
            base.InsertItem(index, item);
            for (var i = index + 1; i < Count; ++i)
            {
                this[i].Index = i;
            }
        }

        protected override void RemoveItem(int index)
        {
            this[index].Data.ListChanged -= Data_ListChanged;
            base.RemoveItem(index);
            foreach (var tupleView in TuplesView)
            {
                tupleView.OnContentChanged();
            }
            for (var i = index; i < Count; ++i)
            {
                this[i].Index = i;
            }
        }

        protected override void SetItem(int index, RelationAttribute item)
        {
            item.Index = index;
            item.Data.ListChanged += Data_ListChanged;
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

    //[Serializable]
    //public class Relation
    //{
    //    public Relation() : this(new List<RelationAttribute>())
    //    {
    //    }

    //    public Relation(List<RelationAttribute> attributes)
    //    {
    //        Attributes = new ObservableCollection<RelationAttribute>(attributes);
    //        if (attributes.Count != 0)
    //        {
    //            foreach (var attribute in attributes)
    //            {
    //                if (attribute.Data.Count != TupleCount)
    //                    throw new ArgumentException("tuple count is not same");
    //            }
    //        }
    //        this.TupleViews = new ObservableCollection<TupleView>();
    //        this.RebuildTupleViews();
    //    }

    //    public ObservableCollection<RelationAttribute> Attributes { get; private set; }
    //    [field: NonSerialized] public ObservableCollection<TupleView> TupleViews { get; private set; }
    //    private int TupleCount
    //    {
    //        get
    //        {
    //            if (this.Attributes.Count == 0)
    //            {
    //                return 0;
    //            }
    //            else
    //            {
    //                return this.Attributes[0].Data.Count;
    //            }
    //        }
    //        set
    //        {
    //            foreach (var attribute in this.Attributes)
    //            {
    //                attribute.Data.Resize(value, Value.MISSING);
    //            }
    //            var originCount = TupleViews.Count;
    //            for (int i = value; i < originCount; ++i)
    //            {
    //                TupleViews.RemoveAt(value);
    //            }
    //        }
    //    }

    //    private void RebuildTupleViews()
    //    {
    //        this.TupleViews.Clear();
    //        for (int i = 0; i < this.TupleCount; ++i)
    //        {
    //            this.TupleViews.Add(new TupleView(this, i));
    //        }
    //    }

    //    public RelationAttribute AllMissingAttribute(string name, AttributeType type)
    //    {
    //        var data = new List<Value>();
    //        data.Resize(this.TupleCount, Value.MISSING);
    //        return new RelationAttribute(name, type, data);
    //    }

    //    public void AddAttribute(RelationAttribute attribute)
    //    {
    //        if (this.TupleCount == 0)
    //        {
    //            this.Attributes.Add(attribute);
    //            this.RebuildTupleViews();
    //            return;
    //        }
    //        Trace.Assert(this.TupleCount == attribute.Data.Count);
    //        var attributeIndex = this.Attributes.Count;
    //        this.Attributes.Add(attribute);
    //        foreach (var tupleView in this.TupleViews)
    //        {
    //            tupleView.FirePropertyChangedEvent(attributeIndex);
    //        }
    //    }

    //    public void InsertAttribute(int attributeIndex, RelationAttribute attribute)
    //    {
    //        if (this.TupleCount == 0)
    //        {
    //            if (attributeIndex != 0)
    //            {
    //                throw new ArgumentException();
    //            }
    //            this.Attributes.Add(attribute);
    //            this.RebuildTupleViews();
    //            return;
    //        }
    //        Trace.Assert(this.TupleCount == attribute.Data.Count);
    //        this.Attributes.Insert(attributeIndex, attribute);
    //        foreach (var tupleView in this.TupleViews)
    //        {
    //            for (var i = attributeIndex; i < this.Attributes.Count; ++i)
    //                tupleView.FirePropertyChangedEvent(i);
    //        }
    //    }

    //    public void RemoveAttribute(int attributeIndex)
    //    {
    //        this.Attributes.RemoveAt(attributeIndex);
    //        foreach (var tupleView in this.TupleViews)
    //        {
    //            tupleView.FirePropertyChangedEvent(attributeIndex);
    //        }
    //    }

    //    public Value this[int attributeIndex, int tupleIndex]
    //    {
    //        get => this.Attributes[attributeIndex].Data[tupleIndex];
    //        set
    //        {
    //            if (!this.Attributes[attributeIndex].Type.ValidateValueWithMissing(value))
    //                throw new ArgumentException(
    //                    $"invalid value {value} for attribute {this.Attributes[attributeIndex].Type}");
    //            this.Attributes[attributeIndex].Data[tupleIndex] = value;
    //            this.TupleViews[tupleIndex].FirePropertyChangedEvent(attributeIndex);
    //        }
    //    }

    //    public void AddTuple(IEnumerable<Value> vs)
    //    {
    //        var tupleIndex = this.TupleCount;
    //        this.TupleCount += 1; // this extends all list
    //        var tupleView = new TupleView(this, tupleIndex);
    //        this.TupleViews.Add(tupleView);

    //        void cleanUp()
    //        {
    //            this.TupleCount -= 1; // reset tupleCount and shrink all list
    //            throw new System.ArgumentException(
    //                $"element count of given tuple is not equal to attribute count {this.Attributes.Count}"
    //                );
    //        }

    //        var e = vs.GetEnumerator();
    //        for (var attributeIndex = 0; attributeIndex < this.Attributes.Count; ++attributeIndex)
    //        {
    //            if (e.MoveNext())
    //            {
    //                var current = e.Current;
    //                if (!this.Attributes[attributeIndex].Type.ValidateValueWithMissing(current))
    //                {
    //                    this.TupleCount -= 1; // reset tupleCount and shrink all list
    //                    throw new ArgumentException(
    //                        $"invalid value in vs"
    //                        );
    //                }
    //                this.Attributes[attributeIndex].Data[tupleIndex] = current;
    //            }
    //            else
    //            {
    //                cleanUp();
    //            }
    //        }
    //        if (e.MoveNext())
    //            cleanUp();
    //    }

    //    public void AddTuple()
    //    {
    //        if (Attributes.Count == 0)
    //            return;
    //        var tupleIndex = this.TupleCount;
    //        this.TupleCount += 1; // this extends all list
    //        var tupleView = new TupleView(this, tupleIndex);
    //        this.TupleViews.Add(tupleView);
    //    }

    //    public void InsertTuple(int index, IEnumerable<Value> vs)
    //    {
    //        if (Attributes.Count == 0)
    //            return;

    //        void throwException()
    //        {
    //            throw new System.ArgumentException(
    //                $"element count of given tuple is not equal to attribute count {this.Attributes.Count}"
    //                );
    //        }

    //        var e = vs.GetEnumerator();
    //        for (var i = 0; i < this.Attributes.Count; ++i)
    //        {
    //            if (e.MoveNext())
    //            {
    //                this.Attributes[i].Data.Insert(index, e.Current);
    //            }
    //            else
    //            {
    //                for (var j = 0; j < i; ++j)
    //                    this.Attributes[j].Data.RemoveAt(index);
    //                throwException();
    //            }
    //        }
    //        if (e.MoveNext())
    //        {
    //            for (var i = 0; i < this.Attributes.Count; ++i)
    //            {
    //                this.Attributes[i].Data.RemoveAt(index);
    //            }
    //            throwException();
    //        }
    //        this.TupleViews.Insert(index, new TupleView(this, index));
    //        for (var i = index + 1; i < this.TupleCount; ++i)
    //        {
    //            this.TupleViews[i].SetTupleIndexWithoutEvent(i);
    //        }
    //    }

    //    public void InsertTuple(int index)
    //    {
    //        if (Attributes.Count == 0)
    //            return;

    //        for (var i = 0; i < this.Attributes.Count; ++i)
    //        {
    //            this.Attributes[i].Data.Insert(index, Value.MISSING);
    //        }
    //        this.TupleViews.Insert(index, new TupleView(this, index));
    //        for (var i = index + 1; i < this.TupleCount; ++i)
    //        {
    //            this.TupleViews[i].SetTupleIndexWithoutEvent(i);
    //        }
    //    }

    //    public void RemoveTupleAt(int index)
    //    {
    //        for (var i = 0; i < this.Attributes.Count; ++i)
    //        {
    //            this.Attributes[i].Data.RemoveAt(index);
    //        }
    //        this.TupleViews.RemoveAt(index);
    //        for (var i = index; i < this.TupleCount; ++i)
    //        {
    //            this.TupleViews[i].SetTupleIndexWithoutEvent(i);
    //        }
    //    }

    //    public void Clear()
    //    {
    //        this.TupleViews.Clear();
    //        this.Attributes.Clear();
    //    }

    //    public void ReadFromStream(Stream stream)
    //    {
    //        this.Clear();
    //        var formatter = new BinaryFormatter();
    //        var newRelation = (Relation)formatter.Deserialize(stream);
    //        this.Attributes = newRelation.Attributes;
    //        this.RebuildTupleViews();
    //    }

    //    public void SaveToStream(Stream stream)
    //    {
    //        var formatter = new BinaryFormatter();
    //        formatter.Serialize(stream, this);
    //    }
    //}
}
