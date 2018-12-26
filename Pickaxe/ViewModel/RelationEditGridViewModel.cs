using Pickaxe.Model;
using Pickaxe.Utility;
using Pickaxe.Utility.ListExtension;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            get => _addAttribute ?? (
                _addAttribute = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        var data = new ObservableCollection<Value>();
                        data.Resize(Relation.TuplesView.Count, Value.MISSING);
                        Relation.Add(
                            new RelationAttribute("New Attribute", new AttributeType.Numeric(), data)
                            );
                    })
                );
        }

        public ICommand InsertAttribute
        {
            get => _insertAttribute ?? (
                _insertAttribute = new RelayCommand(
                    parameter => {
                        if (parameter == null)
                            return false;
                        return (int)parameter <= Relation.Count;
                    },
                    parameter =>
                    {
                        var data = new ObservableCollection<Value>();
                        data.Resize(Relation.TuplesView.Count, Value.MISSING);
                        Relation.Insert((int)parameter,
                            new RelationAttribute("New Attribute", new AttributeType.Numeric(), data)
                            );
                    })
                );
        }

        public ICommand RemoveAttribute
        {
            get => _removeAttribute ?? (
                _removeAttribute = new RelayCommand(
                    parameter => {
                        if (parameter == null)
                            return false;
                        return (int)parameter < Relation.Count;
                    },
                    parameter =>
                    {
                        Relation.RemoveAt((int)parameter);
                    })
                );
        }

        public ICommand AddTuple
        {
            get => _addTuple ?? (
                _addTuple = new RelayCommand(
                    parameter => {
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
                        if (parameter == null)
                            return false;
                        if (Relation.Count == 0)
                            return false;
                        return ((IEnumerable<int>)parameter).All((i) => i <= Relation.TuplesView.Count);
                    },
                    parameter =>
                    {
                        var indices = ((IEnumerable<int>)parameter).ToList();
                        indices.Sort((x, y) => -x.CompareTo(y)); // reverse sort
                        foreach (var index in indices)
                            Relation.TuplesView.Insert(index, TupleView.Detached);
                    })
                );
        }

        public ICommand RemoveTuple
        {
            get => _removeTuple ?? (
                _removeTuple = new RelayCommand(
                    parameter =>
                    {
                        if (parameter == null)
                            return false;
                        if (Relation.Count == 0)
                            return false;
                        return ((IEnumerable<int>)parameter).All((i) => i < Relation.TuplesView.Count);
                    },
                    parameter =>
                    {
                        var indices = ((IEnumerable<int>)parameter).ToList();
                        indices.Sort((x, y) => -x.CompareTo(y)); // reverse sort
                        foreach (var index in indices)
                            Relation.TuplesView.RemoveAt(index);
                    })
                );
        }
    }
}
