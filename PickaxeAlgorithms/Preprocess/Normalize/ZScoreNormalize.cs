using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Markup;

namespace Pickaxe.Algorithms.Preprocess.Normalize
{
    class ZScoreNormalize : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Z-Score Normalize";
        public override string Description => "Z-Score is the signed fractional number of standard deviations by which the value of an observation or data point is above the mean value of what is being observed or measured.";

        public ZScoreNormalize()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be Z-Score normalized", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Generate new Attibute","whether generate new attrbute",typeof(bool),false),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
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
            // Z-Score Normalize
            if (!(attribute.Type is AttributeType.Numeric))
            {
                WriteOutputLine($"Error:Only numeric type is allowed!");
                return;
            }
            Value sum = 0;
            float miu, sigma;
            var temp = attribute.Data.Where((x) => !x.IsMissing()).ToList();
            foreach (var v in temp)
                sum += v;
            miu = sum / temp.Count; // average
            sum = 0;
            foreach (var v in temp)
                sum += (v - miu) * (v - miu);
            sigma = (float)Math.Sqrt(sum / temp.Count); // standard deviation
            if (flag)
            {
                var data = new ObservableCollection<Value>();
                data.Resize(attribute.Data.Count, Value.MISSING);
                var newAttr = new RelationAttribute(attribute.Name + "z_score_result", attribute.Type, data);
                for (var i = 0; i < attribute.Data.Count; i++)
                {
                    if (attribute.Data[i].IsMissing())
                    {
                        newAttr.Data[i] = Value.MISSING;
                        continue;
                    }
                    newAttr.Data[i] = (attribute.Data[i] - miu) / sigma;
                }
                Relation.Add(newAttr);
                return;
            }
            for (var i = 0; i < attribute.Data.Count; i++)
            {
                if (attribute.Data[i].IsMissing())
                    continue;
                attribute.Data[i] = (attribute.Data[i] - miu) / sigma;
            }
        }
    }
}
