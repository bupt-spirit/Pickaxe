using PickaxeCore.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace PickaxeCore.Relation
{
    public class RelationAttribute
    {
        public AttributeType Type { get; set; }
        public List<Value> Data { get; set; }
        public string Name { get; set; }

        public RelationAttribute(string name, AttributeType type, List<Value> data)
        {
            Name = name;
            Type = type;
            Data = data;
        }
    }

    public class Relation
    {
        public Relation() : this(new List<RelationAttribute>())
        {
        }

        public Relation(List<RelationAttribute> attributes)
        {
            Attributes = attributes;
            if (attributes.Count != 0)
            {
                foreach (var attribute in attributes)
                {
                    if (attribute.Data.Count != TupleCount)
                        throw new ArgumentException("tuple count is not same");
                }
            }
            this.TupleViews = new ObservableCollection<TupleView>();
            this.BuildTupleViews();
        }

        public List<RelationAttribute> Attributes { get; private set; }
        public int AttributeCount
        {
            get => this.Attributes.Count;
        }
        public int TupleCount
        {
            get
            {
                if (this.Attributes.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return this.Attributes[0].Data.Count;
                }
            }
            set
            {
                foreach (var attribute in this.Attributes)
                {
                    attribute.Data.Resize(value, Value.MISSING);
                }
            }
        }
        public ObservableCollection<TupleView> TupleViews { get; private set; }

        private void BuildTupleViews()
        {
            this.TupleViews.Clear();
            for (int i = 0; i < this.TupleCount; ++i)
            {
                this.TupleViews.Add(new TupleView(this, i));
            }
        }

        public RelationAttribute AllMissingAttribute(string name, AttributeType type)
        {
            var data = new List<Value>();
            data.Resize(this.TupleCount, Value.MISSING);
            return new RelationAttribute(name, type, data);
        }

        public void AddAttribute(RelationAttribute attribute)
        {
            if (this.TupleCount == 0)
            {
                this.Attributes.Add(attribute);
                this.BuildTupleViews();
                return;
            }
            Trace.Assert(this.TupleCount == attribute.Data.Count);
            var attributeIndex = this.Attributes.Count;
            this.Attributes.Add(attribute);
            foreach (var tupleView in this.TupleViews)
            {
                tupleView.FirePropertyChangedEvent(attributeIndex);
            }
        }

        public void InsertAttribute(int attributeIndex, RelationAttribute attribute)
        {
            if (this.TupleCount == 0)
            {
                if (attributeIndex != 0)
                {
                    throw new ArgumentException();
                }
                this.Attributes.Add(attribute);
                this.BuildTupleViews();
                return;
            }
            Trace.Assert(this.TupleCount == attribute.Data.Count);
            this.Attributes.Insert(attributeIndex, attribute);
            foreach (var tupleView in this.TupleViews)
            {
                for (var i = attributeIndex; i < this.AttributeCount; ++i)
                    tupleView.FirePropertyChangedEvent(i);
            }
        }

        public void RemoveAttribute(int attributeIndex)
        {
            this.Attributes.RemoveAt(attributeIndex);
            foreach (var tupleView in this.TupleViews)
            {
                tupleView.FirePropertyChangedEvent(attributeIndex);
            }
        }

        public Value this[int attributeIndex, int tupleIndex]
        {
            get => this.Attributes[attributeIndex].Data[tupleIndex];
            set
            {
                if (!this.Attributes[attributeIndex].Type.ValidateValueWithMissing(value))
                    throw new ArgumentException(
                        $"invalid value {value} for attribute {this.Attributes[attributeIndex].Type}");
                this.Attributes[attributeIndex].Data[tupleIndex] = value;
                this.TupleViews[tupleIndex].FirePropertyChangedEvent(attributeIndex);
            }
        }

        public void AddTuple(IEnumerable<Value> vs)
        {
            var tupleIndex = this.TupleCount;
            this.TupleCount += 1; // this extends all list
            var tupleView = new TupleView(this, tupleIndex);
            this.TupleViews.Add(tupleView);

            void cleanUp()
            {
                this.TupleCount -= 1; // reset tupleCount and shrink all list
                throw new System.ArgumentException(
                    $"element count of given tuple is not equal to attribute count {this.AttributeCount}"
                    );
            }

            var e = vs.GetEnumerator();
            for (var attributeIndex = 0; attributeIndex < this.AttributeCount; ++attributeIndex)
            {
                if (e.MoveNext())
                {
                    var current = e.Current;
                    if (!this.Attributes[attributeIndex].Type.ValidateValueWithMissing(current))
                    {
                        this.TupleCount -= 1; // reset tupleCount and shrink all list
                        throw new ArgumentException(
                            $"invalid value in vs"
                            );
                    }
                    this.Attributes[attributeIndex].Data[tupleIndex] = current;
                }
                else
                {
                    cleanUp();
                }
            }
            if (e.MoveNext())
                cleanUp();
        }

        public void AddTuple()
        {
            var tupleIndex = this.TupleCount;
            this.TupleCount += 1; // this extends all list
            var tupleView = new TupleView(this, tupleIndex);
            this.TupleViews.Add(tupleView);
        }

        public void InsertTuple(int index, IEnumerable<Value> vs)
        {
            void throwException()
            {
                throw new System.ArgumentException(
                    $"element count of given tuple is not equal to attribute count {this.AttributeCount}"
                    );
            }

            var e = vs.GetEnumerator();
            for (var i = 0; i < this.AttributeCount; ++i)
            {
                if (e.MoveNext())
                {
                    this.Attributes[i].Data.Insert(index, e.Current);
                }
                else
                {
                    for (var j = 0; j < i; ++j)
                        this.Attributes[j].Data.RemoveAt(index);
                    throwException();
                }
            }
            if (e.MoveNext())
            {
                for (var i = 0; i < this.AttributeCount; ++i)
                {
                    this.Attributes[i].Data.RemoveAt(index);
                }
                throwException();
            }
            this.TupleViews.Insert(index, new TupleView(this, index));
            for (var i = index + 1; i < this.TupleCount; ++i)
            {
                this.TupleViews[i].SetTupleIndexWithoutEvent(i);
            }
        }

        public void InsertTuple(int index)
        {
            for (var i = 0; i < this.AttributeCount; ++i)
            {
                this.Attributes[i].Data.Insert(index, Value.MISSING);
            }
            this.TupleViews.Insert(index, new TupleView(this, index));
            for (var i = index + 1; i < this.TupleCount; ++i)
            {
                this.TupleViews[i].SetTupleIndexWithoutEvent(i);
            }
        }

        public void RemoveTupleAt(int index)
        {
            for (var i = 0; i < this.AttributeCount; ++i)
            {
                this.Attributes[i].Data.RemoveAt(index);
            }
            this.TupleViews.RemoveAt(index);
            for (var i = index; i < this.TupleCount; ++i)
            {
                this.TupleViews[i].SetTupleIndexWithoutEvent(i);
            }
        }
    }

    public class TupleView : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;
        public void FirePropertyChangedEvent(int index)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"Values[{index}]"));
        }

        public TupleView(Relation relation, int tupleIndex)
        {
            this.Relation = relation;
            this.TupleIndex = tupleIndex;
        }

        private int tupleIndex;

        public int TupleIndex
        {
            get
            {
                return this.tupleIndex;
            }
            set
            {
                if (this.tupleIndex != value)
                {
                    this.tupleIndex = value;
                    for (var i = 0; i < this.Relation.AttributeCount; ++i)
                    {
                        this.FirePropertyChangedEvent(i);
                    }
                }
            }
        }
        public Relation Relation { get; private set; }

        public Value this[int attributeIndex]
        {
            get
            {
                if (attributeIndex < this.Relation.AttributeCount && this.TupleIndex < this.Relation.TupleCount)
                    return this.Relation[attributeIndex, this.TupleIndex];
                else
                    logger.Debug("accessing missing value at ({0}, {1})", attributeIndex, this.TupleIndex);
                return Value.MISSING;
            }
            set
            {
                this.Relation[attributeIndex, this.TupleIndex] = value;
                FirePropertyChangedEvent(attributeIndex);
            }
        }

        public void SetTupleIndexWithoutEvent(int index)
        {
            this.tupleIndex = index;
        }
    }
}
