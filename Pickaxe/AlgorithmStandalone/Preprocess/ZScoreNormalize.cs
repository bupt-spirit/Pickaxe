using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pickaxe.AlgorithmStandalone.Preprocess
{
    class ZScoreNormalize
    {
        public static void Run(RelationAttribute attribute)
        {
            //Z-Score Normalize
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
