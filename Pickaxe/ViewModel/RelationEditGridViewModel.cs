﻿using Pickaxe.Model;
using Pickaxe.Utility;
using Pickaxe.Utility.ListExtension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Pickaxe.ViewModel
{
    public class RelationEditGridViewModel : NotifyPropertyChangedBase
    {
        private Relation _relation;

        private ICommand _addAttribute;
        private ICommand _insertAttribute;
        private ICommand _removeAttribute;

        private ICommand _addTuple;
        private ICommand _insertTuple;
        private ICommand _removeTuple;

        public Relation Relation
        {
            get => _relation;
            set
            {
                _relation = value;
                OnPropertyChanged("Relation");
            }
        }

        public ICommand AddAttribute
        {
            get => _addAttribute;
            set {
                _addAttribute = value;
                OnPropertyChanged("AddAttribute");
            }
        }

        public ICommand InsertAttribute
        {
            get => _insertAttribute;
            set
            {
                _insertAttribute = value;
                OnPropertyChanged("InsertAttribute");
            }
        }
        public ICommand RemoveAttribute
        {
            get => _removeAttribute;
            set
            {
                _removeAttribute = value;
                OnPropertyChanged("RemoveAttribute");
            }
        }

        public ICommand AddTuple
        {
            get => _addTuple ?? (
                _addTuple = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        return Relation.Count != 0;
                    },
                    parameter =>
                    {
                        Relation.TuplesView.Add(TupleView.Detached);
                    })
                );
        }

        public ICommand InsertTuple
        {
            get => _insertTuple ?? (
                _insertTuple = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        return (int)parameter <= Relation.TuplesView.Count;
                    },
                    parameter =>
                    {
                        Relation.TuplesView.Insert((int)parameter, TupleView.Detached);
                    })
                );
        }

        public ICommand RemoveTuple
        {
            get => _removeTuple ?? (
                _removeTuple = new RelayCommand(
                    parameter =>
                    {
                        if (Relation == null)
                            return false;
                        if (parameter == null)
                            return false;
                        if (Relation.Count == 0)
                            return false;
                        var objects = ((Collection<object>)parameter);
                        return objects.Count() != 0;
                    },
                    parameter =>
                    {
                        var indices = ((IEnumerable<object>)parameter)
                            .Select((obj) => ((TupleView)obj).TupleIndex)
                            .ToList();
                        indices.Sort((x, y) => -x.CompareTo(y)); // reverse sort
                        foreach (var index in indices)
                            Relation.TuplesView.RemoveAt(index);
                    })
                );
        }
    }
}
