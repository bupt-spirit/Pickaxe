using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Associate
{
    class SkewnessPeakedness : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Associate;

        public override string Name => "Caculate skewness and peakness";

        public override string Description => "Caculate the skewness and peakedness of one specific attribute. Both are" +
            "used to describe if the attribute is in accordance with normal distribution(正态分布).If the attribute strictly in accordance with" +
            "normal distribution, then skewness=0, peakedness=3";

        public SkewnessPeakedness()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Caculate attribute's skewness and peakedness", typeof(IEnumerable<RelationAttribute>), null),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                if (!(attribute.Type is AttributeType.Numeric))
                {
                    WriteOutputLine($"Only numeric type is allowed");
                    return;
                }
                Value sum = 0;
                float miu, sigma;
                var temp = attribute.Data.Where((x) => !x.IsMissing()).ToList();
                foreach (var v in temp)
                    sum += v;
                miu = sum / (temp.Count - 1); // average
                sum = 0;
                foreach (var v in temp)
                    sum += (v - miu) * (v - miu);
                sigma = (float)Math.Sqrt(sum / (temp.Count - 1)); // standard deviation
                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }
    }
}
