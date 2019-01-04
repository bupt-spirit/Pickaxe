using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pickaxe.Algorithms.Preprocess.Clean
{
    class RemoveNominalValue : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Remove Nominal Value";
        public override string Description => "Remove a nominal value and set all value of the label to MISSING";

        public RemoveNominalValue()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be processed", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Label", "The label to be removed", typeof(string), "?"),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            var label = (string)Options[1].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                RemoveNominalLabel(attribute, label);
                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }

        public void RemoveNominalLabel(RelationAttribute attribute, string label)
        {
            if (!(attribute.Type is AttributeType.Nominal))
            {
                WriteOutputLine($"Attribute {attribute.Name} is not Nominal, skiped.");
                return;
            }
            var nominalType = (AttributeType.Nominal)attribute.Type;
            var index = nominalType.NominalLabels.IndexOf(label);
            var value = Value.ToValue(index);
            if (index == -1)
            {
                WriteOutputLine($"Attribute {attribute.Name} does not contain label {label}, skiped.");
                return;
            }
            for (int i = 0; i < attribute.Data.Count; ++i)
            {
                if (attribute.Data[i].IsMissing())
                    continue;
                if (attribute.Data[i] < value)
                {
                    continue;
                }
                else if (attribute.Data[i] == value)
                {
                    WriteOutputLine($"Set value of tuple {i} and attribute {attribute.Name} to MISSING");
                    attribute.Data[i] = Value.MISSING;
                }
                else
                {
                    attribute.Data[i] -= 1.0f;
                }
            }
            nominalType.NominalLabels.RemoveAt(index);
        }
    }
}
