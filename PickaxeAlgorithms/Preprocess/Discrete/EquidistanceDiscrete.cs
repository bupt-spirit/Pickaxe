﻿using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pickaxe.Algorithms.Preprocess.Discrete
{
    class EquidistantDiscrete : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Preprocess;
        public override string Name => "Equidistant Discrete";
        public override string Description => "Equidistant Discrete is to discrete values by equidistant binning.";

        public EquidistantDiscrete()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be equidistant discreted", typeof(IEnumerable<RelationAttribute>), null),
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

        public void Discrete(RelationAttribute attribute, int binNumber,bool flag)
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
            Value binSize = (max - min) / binNumber;
            if (flag)
            {
                var data = new ObservableCollection<Value>();
                data.Resize(attribute.Data.Count, Value.MISSING);
                var newAttr = new RelationAttribute(attribute.Name + "equidistance_result", attribute.Type, data);
                for (int i = 0; i < attribute.Data.Count; ++i)
                {
                    if (attribute.Data[i].IsMissing())
                    {
                        newAttr.Data[i] = Value.MISSING;
                        continue;
                    }
                    if (attribute.Data[i] == max)
                        newAttr.Data[i] = binNumber - 1; // if data[i] is max, use binNumber - 1
                    else
                        newAttr.Data[i] = (float)Math.Floor((attribute.Data[i] - min) / binSize);
                }
                Relation.Add(newAttr);
                return;
            }
            for (int i = 0; i < attribute.Data.Count; ++i)
            {
                if (attribute.Data[i].IsMissing())
                    continue;
                if (attribute.Data[i] == max)
                    attribute.Data[i] = binNumber - 1; // if data[i] is max, use binNumber - 1
                else
                    attribute.Data[i] = (float)Math.Floor((attribute.Data[i] - min) / binSize);
            }
        }
    }
}
