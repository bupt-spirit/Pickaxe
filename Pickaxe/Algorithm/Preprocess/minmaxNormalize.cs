using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Algorithm.Preprocess
{
    class MinMaxNormalize
    {
        public static void Run(RelationAttribute attribute)
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
            for (int i = 0; i < attribute.Data.Count; i++) {
                if (attribute.Data[i].IsMissing())
                    continue;
                attribute.Data[i] = (attribute.Data[i] - min) / (max - min);
            }
        }
    }
}
