using Pickaxe.Utility;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace Pickaxe.Model
{
    public class TupleView : NotifyPropertyChangedBase
    {
        #region Fields

        private Relation _relation;
        private int _tupleIndex;

        #endregion

        #region Properties

        public Relation Relation
        {
            get => _relation;
            set
            {
                _relation = value;
                OnPropertyChanged("Relation");
            }
        }

        public int TupleIndex
        {
            get => _tupleIndex;
            set
            {
                _tupleIndex = value;
                OnPropertyChanged("TupleIndex");
            }
        }

        [IndexerName("Item")]
        public Value this[int index]
        {
            get => Relation[index].Data[TupleIndex];
            set
            {
                Relation[index].Data[TupleIndex] = value;
                OnPropertyChanged("Item[]");
            }
        }

        #endregion

        #region Constructors

        public TupleView(Relation relation, int tupleIndex)
        {
            Relation = relation;
            TupleIndex = tupleIndex;
        }

        #endregion

        #region Static functions

        public static readonly TupleView Detached = new TupleView(null, -1);

        #endregion

        #region Methods

        public bool IsDetached() => this.Equals(Detached);

        #endregion
    }

    //public class TupleView : INotifyPropertyChanged
    //{
    //    private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    public void FirePropertyChangedEvent(int index)
    //    {
    //        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"Values[{index}]"));
    //    }

    //    public TupleView(Relation relation, int tupleIndex)
    //    {
    //        this.Relation = relation;
    //        this.TupleIndex = tupleIndex;
    //    }

    //    private int tupleIndex;

    //    public int TupleIndex
    //    {
    //        get
    //        {
    //            return this.tupleIndex;
    //        }
    //        set
    //        {
    //            if (this.tupleIndex != value)
    //            {
    //                this.tupleIndex = value;
    //                for (var i = 0; i < this.Relation.Attributes.Count; ++i)
    //                {
    //                    this.FirePropertyChangedEvent(i);
    //                }
    //            }
    //        }
    //    }
    //    public Relation Relation { get; private set; }

    //    public Value this[int attributeIndex]
    //    {
    //        get
    //        {
    //            if (attributeIndex < this.Relation.Attributes.Count && this.TupleIndex < this.Relation.TupleViews.Count)
    //                return this.Relation[attributeIndex, this.TupleIndex];
    //            else
    //                logger.Debug("accessing missing value at ({0}, {1})", attributeIndex, this.TupleIndex);
    //            return Value.MISSING;
    //        }
    //        set
    //        {
    //            this.Relation[attributeIndex, this.TupleIndex] = value;
    //            FirePropertyChangedEvent(attributeIndex);
    //        }
    //    }

    //    public void SetTupleIndexWithoutEvent(int index)
    //    {
    //        this.tupleIndex = index;
    //    }
    //}
}
