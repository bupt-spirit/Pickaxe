using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.AlgorithmStandalone.Preprocess
{
    class EquifrequentDiscrete
    {
        struct SaveIndex
        {
            public int oldIndex;
            public Value v;

            public SaveIndex(Value v, int oldIndex)
            {
                this.v = v;
                this.oldIndex = oldIndex;
            }
        }
        public static void Run(RelationAttribute attribute, int binNumber)
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
    }
}
