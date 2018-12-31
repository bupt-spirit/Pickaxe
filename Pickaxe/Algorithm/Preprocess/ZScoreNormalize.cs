using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pickaxe.Algorithm.Preprocess
{
    class ZScoreNormalize : IAlgorithm
    {
        public string Name => "Z-Score Normalize";
        public string Description => "Z-Score is the signed fractional number of standard deviations by which the value of an observation or data point is above the mean value of what is being observed or measured.";

        public ObservableCollection<Option> Options { get; private set; }
        public Relation Relation { get; set; }

        public ZScoreNormalize()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be Z-Score normalized", typeof(IEnumerable<RelationAttribute>), null),
            };
        }

        public void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            foreach (var attribute in attributes)
                Normalize(attribute);
        }

        private static void Normalize(RelationAttribute attribute)
        {
            // Z-Score Normalize
            if (!(attribute.Type is AttributeType.Numeric))
                return;
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
            for (int i = 0; i < attribute.Data.Count; i++)
            {
                if (attribute.Data[i].IsMissing())
                    continue;
                attribute.Data[i] = (attribute.Data[i] - miu) / sigma;
            }
        }
    }
}
