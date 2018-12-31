using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pickaxe.Algorithms.Preprocess.Normalize
{
    class MinMaxNormalize : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Min Max Normalize";
        public override string Description => "Min Max Normalization is a normalization strategy which linearly transforms x to y = (x - min) / (max - min), where min and max are the minimum and maximum values in X, where X is the set of observed values of x.";

        public MinMaxNormalize()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be Min Max normalized", typeof(IEnumerable<RelationAttribute>), null),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                Normalize(attribute);
                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }

        private static void Normalize(RelationAttribute attribute)
        {
            if (!(attribute.Type is AttributeType.Numeric))
                return;
            Value max = Single.NegativeInfinity, min = Single.PositiveInfinity;
            foreach (var v in attribute.Data)
            {
                if (v.IsMissing())
                    continue;
                if (v > max)
                    max = v;
                if (v < min)
                    min = v;
            }
            if (Single.IsNegativeInfinity(max) || Single.IsPositiveInfinity(min))
                return;
            for (int i = 0; i < attribute.Data.Count; i++)
            {
                if (attribute.Data[i].IsMissing())
                    continue;
                attribute.Data[i] = (attribute.Data[i] - min) / (max - min);
            }
        }
    }
}
