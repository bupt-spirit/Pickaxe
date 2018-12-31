using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.AlgorithmFramework
{
    public interface IAlgorithm
    {
        string Name { get; }
        string Description { get; }
        ObservableCollection<Option> Options { get; }
        Relation Relation { set; }
        void Run();
    }
}
