using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Preprocess.Associate
{
    class SpearmanAssociate : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Associate;

        public override string Name => "Spearman Associate";

        public override string Description => "Spearman Associate analyse the association between 2 attributes,must be numeric type, " +
            "don't need to be in accordance with normal distribution(正态分布. The result ranges between -1 and 1. " +
            "If result=0, association is week; With the growth of result, association is stronger. If result=1, then x and y " +
            "strictly monotonically increasse. Else if result=-1, x and y trictly monotonically decrease. ";

        public SpearmanAssociate()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes take part in Spearman Association", typeof(IEnumerable<RelationAttribute>), null),
            };
        }

        public override void Run()
        {
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            if (!(attributes.Count == 2))
            {
                WriteOutputLine($"Error:Spearman Association only analyse the association between 2 attributes!!!");
                return;
            }
            WriteOutputLine($"Working on attribute {attributes[0].Name} and {attributes[1].Name}...");
            foreach (var attribute in attributes)
            {
                if (!(attribute.Type is AttributeType.Numeric))
                {
                    WriteOutputLine($"Error:Only numeric type is allowed!");
                    return;
                }
            }
            //分别取出两个属性，排序好并保存原来的下标，通过where检查有无缺失值，若有，报错；
            var originCount = attributes[0].Data.Count;
            var first = attributes[0].Data
                .Zip(Enumerable.Range(0, originCount), (v, i) => new SaveIndex(v, i))
                .Where(x => !x.v.IsMissing()).OrderBy((x) => x.v).ToList();
            first.Reverse();
            var second = attributes[1].Data
                .Zip(Enumerable.Range(0, originCount), (v, i) => new SaveIndex(v, i))
                .Where(x => !x.v.IsMissing()).OrderBy((x) => x.v).ToList();
            second.Reverse();
            if (!(first.Count == originCount && second.Count == originCount))
            {
                WriteOutputLine($"Error:Missing Value is not allowed!");
                return;
            }
            float result = 0;//1-[(6*sum(di^2)/(N*(N^2-1))]
            float diSum = 0;
            var firstresult = new int[originCount];
            var secondresult = new int[originCount];
            for (int i = 0; i < originCount; i++)
            {
                firstresult[first[i].oldIndex] = i;
                secondresult[second[i].oldIndex] = i;
            }
            for (int i = 0; i < originCount; i++)
            {
                diSum +=(float) Math.Pow(Math.Abs(firstresult[i] - secondresult[i]), 2);
            }
            result = 1 - (6 * diSum) / ((float)Math.Pow(originCount, 3) - originCount);
            WriteOutputLine($"Spearman Association:{result}");
            WriteOutputLine($"Reference:If result=0, association is week.");
            WriteOutputLine($"With the growth of result, association is stronger.");
            WriteOutputLine($"If result=1, x and y trictly monotonically increase.");
            WriteOutputLine($"Else if result=-1, x and y trictly monotonically decrease.");
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
