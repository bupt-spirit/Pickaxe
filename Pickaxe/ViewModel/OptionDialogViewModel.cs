using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.ViewModel
{
    public class OptionDialogViewModel : NotifyPropertyChangedBase
    {
        #region Fields

        private string _name;
        private string _description;
        private ObservableCollection<Option> _options;
        private Relation _relation;

        #endregion

        #region Properties

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public ObservableCollection<Option> Options
        {
            get => _options;
            set
            {
                _options = value;
                OnPropertyChanged("Options");
            }
        }

        public Relation Relation
        {
            get => _relation;
            set
            {
                _relation = value;
                OnPropertyChanged("Relation");
            }
        }

        #endregion
    }
}
