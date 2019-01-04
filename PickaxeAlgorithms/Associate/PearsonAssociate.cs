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
    class PearsonAssociate : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Associate;

        public override string Name => "Pearson Associate";

        public override string Description => "Pearsonn Associate analyse the association between 2 attributes,must be numeric type, " +
            "must be in accordance with normal distribution(正态分布. The result ranges between -1 and 1. " +
            "If result=0, association is week; With the growth of result, association is stronger. If result=1, then x and y " +
            "strictly monotonically increasse. Else if result=-1, x and y trictly monotonically decrease. ";
        public PearsonAssociate()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes take part in Pearson Association", typeof(IEnumerable<RelationAttribute>), null),
            };
        }

        public override void Run()
        {
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            if (!(attributes.Count == 2))
            {
                WriteOutputLine($"Error:Pearson Association only analyse the association between 2 attributes!!!");
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
            var originCount = attributes[0].Data.Count;
            var first = attributes[0].Data.Where(x => !(x.IsMissing())).ToList();
            var second = attributes[1].Data.Where(x => !(x.IsMissing())).ToList();
            if (!(first.Count == originCount && second.Count == originCount))
            {
                WriteOutputLine($"Error:Missing Value is not allowed!");
                return;
            }
            float EXY = 0, EX = 0, EY = 0, EX2 = 0, EY2 = 0;
            for (int i = 0; i < originCount; i++)
            {
                EXY += first[i] * second[i];
                EX += first[i];
                EY += second[i];
                EX2 += (float)Math.Pow(first[i], 2);
                EY2 += (float)Math.Pow(second[i], 2);
            }
            EXY /= originCount;
            EX /= originCount;
            EY /= originCount;
            EX2 /= originCount;
            EY2 /= originCount;
            float pearson = (EXY - EX * EY) / (float)((Math.Sqrt(EX2 - (float)Math.Pow(EX, 2))) * Math.Sqrt(EY2 - (float)Math.Pow(EY, 2)));
            WriteOutputLine($"Pesrson Association:{pearson}");
            WriteOutputLine($"Reference:If result=0, association is week.");
            WriteOutputLine($"With the growth of result, association is stronger.");
            WriteOutputLine($"If result=1, x and y trictly monotonically increase.");
            WriteOutputLine($"Else if result=-1, x and y trictly monotonically decrease.");
        }
    }
}
