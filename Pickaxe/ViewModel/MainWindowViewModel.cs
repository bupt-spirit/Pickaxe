using Pickaxe.Model;
using Pickaxe.Utility;
using System.Windows.Input;

namespace Pickaxe.ViewModel
{
    public class MainWindowViewModel : NotifyPropertyChangedBase
    {
        private Relation _relation;
        private string _fileName;

        private ICommand _newRelation;
        private ICommand _openRelation;
        private ICommand _saveRelation;
        private ICommand _saveAsRelation;

        public Relation Relation {
            get => _relation;
            set {
                _relation = value;
                OnPropertyChanged("Relation");
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public MainWindowViewModel()
        {
            Relation = new Relation();
            FileName = null;
        }

        public ICommand NewRelation
        {
            get => _newRelation ?? (
                _newRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        Relation = new Relation();
                    })
                );
        }

        public ICommand OpenRelation
        {
            get => _openRelation ?? (
                _openRelation = new RelayCommand(
                    parameter => true,
                    parameter =>
                    {
                        // TODO open relation content
                    })
                );
        }
    }
}
