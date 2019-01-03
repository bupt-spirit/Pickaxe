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
                new Option("Generate new Attibute","whether generate new attrbute",typeof(bool),false),
            };
        }

        public override void Run()
        {
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            var binNumber = (int)Options[1].Value;
            var flag = (bool)Options[2].Value;
            foreach (var attribute in attributes)
            {
                WriteOutputLine($"Working on attribute {attribute.Name}...");
                Discrete(attribute, binNumber,flag);
                WriteOutputLine($"Finished working on attribute {attribute.Name}");
            }
        }
        
        // TODO: Maybe wrong algorithm
        public void Discrete(RelationAttribute attribute, int binNumber,bool flag)
        {
            if (!(attribute.Type is AttributeType.Numeric))
                return;
            var originCount = attribute.Data.Count;
            var temp = attribute.Data
                .Zip(Enumerable.Range(0, originCount), (v, i) => new SaveIndex(v, i))
                .Where(x => !x.v.IsMissing()).OrderBy((x) => x.v).ToList();
            if (temp.Count == 0)
                return;
            var binSize = (int)Math.Ceiling(temp.Count / (double)binNumber);
            var binCount = new List<int>();
            if (flag)
            {
                var data = new ObservableCollection<Value>();
                data.Resize(attribute.Data.Count, Value.MISSING);
                var newAttr = new RelationAttribute(attribute.Name + "equifrequent_result", attribute.Type, data);
                binCount.Resize(binNumber, 0); // Tracing the count in every bin
                for (var k = 0; k < temp.Count; k++)
                {
                    var j = (int)Math.Floor(k / (double)binNumber);
                    while (j < binNumber)
                    {
                        if (binCount[j] < binSize)
                        {
                            newAttr.Data[temp[k].oldIndex] = j;
                            binCount[j] += 1;
                            break;
                        }
                        j++;
                    }
                }
                Relation.Add(newAttr);
                return;
            }
            binCount.Resize(binNumber, 0); // Tracing the count in every bin
            for (var k = 0; k < temp.Count; k++)
            {
                var j = (int)Math.Floor(k / (double)binNumber);
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
