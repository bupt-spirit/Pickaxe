using Pickaxe.Utility;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections;

namespace Pickaxe.Model
{
    public class TupleView : NotifyPropertyChangedBase
    {
        #region Fields

        private Relation _relation;
        private int _tupleIndex;

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
                if (value == -1)
                {
                    _tupleIndex = -1;
                    return;
                }
                _tupleIndex = value;
                OnPropertyChanged("TupleIndex");
                OnPropertyChanged("Item[]");
            }
        }

        public int Count => Relation.Count;

        #endregion


        #region Constructors

        public TupleView(Relation relation, int tupleIndex)
        {
            Relation = relation;
            TupleIndex = tupleIndex;
        }

        #endregion

        #region Methods

        public void SetTupleIndexWithoutUpdate(int index)
        {
            this._tupleIndex = index;
            OnPropertyChanged("TupleIndex");
        }

        public bool IsDetached()
        {
            return Relation == null && TupleIndex == -1;
        }

        public void OnContentChanged() {
            OnPropertyChanged("Item[]");
        }

        #endregion

        #region Static menbers

        public static TupleView _detached;
        public static TupleView Detached
        {
            get => _detached ?? (
                _detached = new TupleView(null, -1)
                );
        }

        #endregion
    }
}
