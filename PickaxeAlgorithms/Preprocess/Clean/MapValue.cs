using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System.Collections.ObjectModel;

namespace Pickaxe.Algorithms.Preprocess.Clean
{
    class MapValue : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Map Value";
        public override string Description => "Map a value to another value";

        public MapValue()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be processed", typeof(RelationAttribute), null),
                new Option("From", "The value to be mapped from", typeof(Value), Value.MISSING),
                new Option("To", "The value to be mapped to", typeof(Value), Value.MISSING),
            };
        }

        public override void Run()
        {
            var attribute = (RelationAttribute)Options[0].Value;
            var from = (Value)Options[1].Value;
            var to = (Value)Options[2].Value;
            for (int i = 0; i < attribute.Data.Count; ++i)
            {
                if (from.IsMissing())
                {
                    if (attribute.Data[i].IsMissing())
                    {
                        WriteOutputLine($"Map value {i} to {to.ToString()}");
                        attribute.Data[i] = to;
                    }
                }
                else if (attribute.Data[i] == from)
                {
                    WriteOutputLine($"Map value {i} to {to.ToString()}");
                    attribute.Data[i] = to;
                }
            }
        }
    }
}
