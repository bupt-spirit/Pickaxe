using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Preprocess.Cluster
{
    class KMeanCluster : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Cluster;

        public override string Name => "K-Mean Cluster";

        public override string Description => "K-Mean Cluster is a cluster stategy based on distance between tuples which will devide the original relation into K groups. the target cluster number K must be given";

        public KMeanCluster()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes take part in clustering", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Cluster number", "Devide the opretion into K parts, K=cluster number", typeof(int), 5),
                new Option("Max Round","Max round number before stop",typeof(int),5),
            };
        }

        public override void Run()
        {
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            int K = (int)Options[1].Value;
            int N = (int)Options[2].Value;
            int originCount = attributes[0].Data.Count;

            for(int i=0;i<attributes.Count;i++)
            {//检查属性
                if (!(attributes[i].Type is AttributeType.Numeric))
                {
                    WriteOutputLine($"Error:Only numeric type is allowed!");
                    return;
                }
                var temp=attributes[i].Data.Where(x => (!x.IsMissing())).ToList();
                if (!(temp.Count == originCount))
                {
                    WriteOutputLine($"Error:Missing Value is not allowed!");
                    return;
                }
            }           
            //生成新属性，保存分类结果
            var data = new ObservableCollection<Value>();
            data.Resize(originCount, Value.MISSING);
            var result = new RelationAttribute("K_Mean_result", new AttributeType.Nominal(), data);

            //生成min,max之间的随机数，左闭右开，不能重复
            var centers = new Value[K, attributes.Count];
            Random rm = new Random();
            var centersIndex = GetRandom(0, true, attributes[0].Data.Count, false, K, rm, false);
            for (int i = 0; i < K; i++) {
                for (int j = 0; j < attributes[0].Data.Count; j++)
                {
                    centers[i, j] = attributes[centersIndex[i]].Data[j];
                    result.Data[centersIndex[i]] = i;
                }
            }
            bool flag = true;
            int round = 1;
            while (flag && (round <= N)) {

            }
        }

        private void CalculateDistance_ToOneCenter() { }

        private void CalculateDistance_ToAllCenter() { }

        private static List<int> GetRandom(int minNum, bool isIncludeMinNum, int maxNum, bool isIncludeMaxNum, int ResultCount, Random rm, bool isSame)
        {
            List<int> randomList = new List<int>();
            int nValue = 0;

            #region 是否包含最大最小值，默认包含最小值，不包含最大值
            if (!isIncludeMinNum) { minNum = minNum + 1; }
            if (isIncludeMaxNum) { maxNum = maxNum + 1; }
            #endregion

            if (isSame)
            {
                for (int i = 0; randomList.Count < ResultCount; i++)
                {
                    nValue = rm.Next(minNum, maxNum);
                    randomList.Add(nValue);
                }
            }
            else
            {
                for (int i = 0; randomList.Count < ResultCount; i++)
                {
                    nValue = rm.Next(minNum, maxNum);
                    //重复判断
                    if (!randomList.Contains(nValue))
                    {
                        randomList.Add(nValue);
                    }
                }
            }

            return randomList;
        }

    }
}
