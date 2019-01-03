using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PickaxeAlgorithms.Preprocess.Convert
{
    class BinaryToNumeric : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Convert Binary to Numeric";
        public override string Description => "Convert binary attributes to numeric.";

        public BinaryToNumeric()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be converted", typeof(IEnumerable<RelationAttribute>), null),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                Convert(attribute);
                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }

        public void Convert(RelationAttribute attribute)
        {
            if (!(attribute.Type is AttributeType.Binary))
            {
                WriteOutputLine($"Attribute {attribute.Name} is not Binary, skiped");
                return;
            }
            attribute.Type = new AttributeType.Numeric();
        }
    }
}
