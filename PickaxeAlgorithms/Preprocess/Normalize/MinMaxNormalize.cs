using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
                new Option("Generate new Attibute","whether generate new attrbute",typeof(bool),false),
            };
        }

        public override void Run()
        {
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            var flag = (bool)Options[1].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                Normalize(attribute, flag);

                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }

        private void Normalize(RelationAttribute attribute, bool flag)
        {
            if (!(attribute.Type is AttributeType.Numeric))
                return;
            Value max = float.NegativeInfinity, min = float.PositiveInfinity;
            foreach (var v in attribute.Data)
            {
                if (v.IsMissing())
                    continue;
                if (v > max)
                    max = v;
                if (v < min)
                    min = v;
            }
            if (float.IsNegativeInfinity(max) || float.IsPositiveInfinity(min))
                return;
            if (flag)
            {

                var data = new ObservableCollection<Value>();
                data.Resize(attribute.Data.Count, Value.MISSING);
                var newAttr = new RelationAttribute(attribute.Name + "min_max_result", attribute.Type, data);
                for (var i = 0; i < attribute.Data.Count; i++)
                {
                    if (attribute.Data[i].IsMissing())
                    {
                        newAttr.Data[i] = Value.MISSING;
                        continue;
                    }
                    newAttr.Data[i] = (attribute.Data[i] - min) / (max - min);
                }
                Relation.Add(newAttr);
                return;
            }
            else
            {
                for (var i = 0; i < attribute.Data.Count; i++)
                {
                    if (attribute.Data[i].IsMissing())
                        continue;
                    attribute.Data[i] = (attribute.Data[i] - min) / (max - min);
                }
            }
        }
    }
}
