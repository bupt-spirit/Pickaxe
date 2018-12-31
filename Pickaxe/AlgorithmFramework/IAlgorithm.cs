using Pickaxe.Model;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Pickaxe.AlgorithmFramework
{
    public enum AlgorithmType
    {
        Preprocess,
        Cluster,
        Classify,
        Associate,
    }

    public interface IAlgorithm
    {
        AlgorithmType Type { get; }
        string Name { get; }
        string Description { get; }
        ObservableCollection<Option> Options { get; }
        Relation Relation { set; }
        IAddChild Output { set; }
        void Run();
    }
}
