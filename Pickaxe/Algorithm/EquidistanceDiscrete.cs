using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.Algorithm
{
    class EquidistanceDiscrete
    {
        public static void run(RelationAttribute attribute, int binNumber)
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
