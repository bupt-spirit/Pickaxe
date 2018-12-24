using PickaxeCore.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using static System.Linq.Enumerable;

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
                    return this.Attributes.First().Data.Count;
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

        public void AddAttribute(string name, AttributeType attributeType, List<Value> data)
        {
            Trace.Assert(this.TupleCount == data.Count);
            this.Attributes.Add(new RelationAttribute(name, attributeType, data));
        }

        public void AddAttribute(string name, AttributeType attributeType)
        {
            this.Attributes.Add(new RelationAttribute(name, attributeType, new List<Value>()));
            this.Attributes.Last().Data.Resize(this.TupleCount, Value.MISSING);
        }

        public void RemoveAttribute(int attributeIndex)
        {
            this.Attributes.RemoveAt(attributeIndex);
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
            }
        }

        public List<Value> Tuple(int tupleIndex)
        {
            return new List<Value>(
                Range(0, this.AttributeCount).Select((attributeIndex) => this[attributeIndex, tupleIndex])
                );
        }

        public IEnumerable<List<Value>> Tuples()
        {
            return Range(0, this.TupleCount).Select((tupleIndex) => this.Tuple(tupleIndex));
        }

        public void AddTuple(IEnumerable<Value> vs)
        {
            var tupleIndex = this.TupleCount;
            this.TupleCount += 1; // this extends all list

            void cleanUp()
            {
                this.TupleCount -= 1; // reset tupleCount and shrink all list
                throw new System.ArgumentException(
                    $"element count of given tuple is not equal to attribute count {this.AttributeCount}"
                    );
            }

            var e = vs.GetEnumerator();
            foreach (var attributeIndex in Range(0, this.AttributeCount))
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
            this.TupleCount += 1;
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
            foreach (var i in Range(0, this.AttributeCount))
            {
                if (e.MoveNext())
                {
                    this.Attributes[i].Data.Insert(index, e.Current);
                }
                else
                {
                    foreach (var j in Range(0, i))
                        this.Attributes[j].Data.RemoveAt(index);
                    throwException();
                }
            }
            if (e.MoveNext())
            {
                foreach (var i in Range(0, this.AttributeCount))
                {
                    this.Attributes[i].Data.RemoveAt(index);
                }
                throwException();
            }
        }

        public void InsertTuple(int index)
        {
            foreach (var i in Range(0, this.AttributeCount))
            {
                this.Attributes[i].Data.Insert(index, Value.MISSING);
            }
        }
    }

    //public class Tuple : ObservableCollection<Value>
    //{
    //    public Relation Relation { get; private set; }
    //    public int Index { get; set; }

    //    public Tuple(Relation relation, int index)
    //    {
    //        this.Relation = relation;
    //        this.Index = index;
    //        this.CollectionChanged += OnCollectionChanged;
    //    }

    //    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //    {
    //        e.Action
    //    }
    //}
}
