using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Pickaxe.Algorithms.Preprocess.Discrete
{
    class EquifrequentDiscrete : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Equifrequent Discrete";
        public override string Description => "Equifrequent Discrete is to discrete values by equifrequent binning";

        public EquifrequentDiscrete()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be equifrequent discreted", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Bin Number", "Total bin number", typeof(int), 10),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            var binNumber = (int)Options[1].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                Discrete(attribute, binNumber);
                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }
        
        // TODO: Maybe wrong algorithm
        public static void Discrete(RelationAttribute attribute, int binNumber)
        {
            if (!(attribute.Type is AttributeType.Numeric))
                return;
            var originCount = attribute.Data.Count;
            var temp = attribute.Data
                .Zip(Enumerable.Range(0, originCount), (v, i) => new SaveIndex(v, i))
                .Where(x => !x.v.IsMissing()).OrderBy((x) => x.v).ToList();
            if (temp.Count == 0)
                return;
            int binSize = (int)Math.Ceiling(((temp.Count - 1) / (double)binNumber));
            List<int> binCount = new List<int>();
            binCount.Resize(binNumber, 0); // Tracing the count in every bin
            for (int k = 0; k < temp.Count; k++)
            {
                int j = (int)Math.Floor(k / (double)binNumber);
                while (j < binNumber)
                {
                    if (binCount[j] < binSize)
                    {
                        attribute.Data[temp[k].oldIndex] = j;
                        binCount[j] += 1;
                        break;
                    }
                    j++;
                }
            }
        }

        private struct SaveIndex
        {
            public int oldIndex;
            public Value v;

            public SaveIndex(Value v, int oldIndex)
            {
                this.v = v;
                this.oldIndex = oldIndex;
            }
        }
    }
}
