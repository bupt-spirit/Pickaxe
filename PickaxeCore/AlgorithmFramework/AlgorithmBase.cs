using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Pickaxe.Model;

namespace Pickaxe.AlgorithmFramework
{
    public abstract class AlgorithmBase : IAlgorithm
    {
        public abstract AlgorithmType Type { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }

        public ObservableCollection<Option> Options { get; protected set; }
        public Relation Relation { get; set; }
        public IAddChild Output { get; set; }

        public abstract void Run();

        public void WriteOutput(string text)
        {
            Output?.AddText(text);
        }

        public void WriteOutputLine(string line)
        {
            if (Output != null)
            {
                Output.AddText(line);
                Output.AddText("\n");
            }
        }
    }
}
