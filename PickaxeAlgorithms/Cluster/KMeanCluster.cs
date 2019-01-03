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
            int tupleCount = attributes[0].Data.Count;
            int attriCount = attributes.Count;

            for (int i = 0; i < attributes.Count; i++)
            {//检查属性
                if (!(attributes[i].Type is AttributeType.Numeric))
                {
                    WriteOutputLine($"Error:Only numeric type is allowed!");
                    return;
                }
                var temp = attributes[i].Data.Where(x => (!x.IsMissing())).ToList();
                if (!(temp.Count == tupleCount))
                {
                    WriteOutputLine($"Error:Missing Value is not allowed!");
                    return;
                }
            }
            //生成新属性，保存分类结果
            var data = new ObservableCollection<Value>();
            data.Resize(tupleCount, Value.MISSING);
            var nominalType = new AttributeType.Nominal();
            for (int i = 0; i < K; ++i)
            {
                nominalType.NominalLabels.Add($"Class{i}");
            }
            var result = new RelationAttribute("K_Mean_result", nominalType, data);

            //生成min,max之间的随机数，左闭右开，不能重复
            var centers = new Value[K, attriCount + 1];
            Random rm = new Random();
            var centersIndex = GetRandom(0, true, attributes[0].Data.Count, false, K, rm, false);
            for (int i = 0; i < K; i++)
            {//记录下聚类中心的序号和下标
                for (int j = 0; j < attriCount; j++)
                {
                    centers[i, j] = attributes[j].Data[centersIndex[i]];
                    result.Data[centersIndex[i]] = i;
                }
                centers[i, attriCount] = i;
            }
            bool flag = true;
            int round = 1;
            for (int i = 0; i < tupleCount; i++)
            {//第一轮分类
                var min = Single.PositiveInfinity;
                int tag = 0;
                for (int x = 0; x < K; x++)
                {
                    Value temp = 0;
                    for (int j = 0; j < attriCount; j++)
                    {
                        temp += (float)Math.Pow((attributes[j].Data[i] - centers[x, j]), 2);
                    }
                    if (temp < min)
                    {
                        min = temp;
                        tag = x;
                    }
                }
                result.Data[i] = tag;
            }
            PrintRound(centers, round, attriCount, K);
            while (flag && (round <= N))
            {
                flag = false;
                round++;
                var avg = new float[K, attriCount + 1];
                for (int i = 0; i < tupleCount; i++)
                {
                    for (int j = 0; j < attriCount; j++)
                    {
                        avg[(int)result.Data[i], j] += attributes[j].Data[i];
                    }
                    avg[(int)result.Data[i], attriCount] += 1;
                }
                for (int x = 0; x < K; x++)
                {//计算新聚类中心，并决定是否要更新中心的值，并循环
                    for (int j = 0; j < attriCount; j++)
                    {
                        avg[x, j] /= avg[x, attriCount];
                        if (!(avg[x, j] == centers[x, j]))
                        {
                            flag = true;
                            centers[x, j] = avg[x, j];
                        }
                    }
                }
                for (int i = 0; i < tupleCount; i++)
                {//再次分类
                    var min = Single.PositiveInfinity;
                    int tag = 0;
                    for (int x = 0; x < K; x++)
                    {
                        Value temp = 0;
                        for (int j = 0; j < attriCount; j++)
                        {
                            temp += (float)Math.Pow((attributes[j].Data[i] - centers[x, j]), 2);
                        }
                        if (temp < min)
                        {
                            min = temp;
                            tag = x;
                        }
                    }
                    result.Data[i] = tag;
                }
                PrintRound(centers, round, attriCount, K);
            }
            Relation.Add(result);
        }



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

        private void PrintRound(Value[,] centers, int round, int attriCount, int K)
        {
            WriteOutputLine($"Round {round}: ");
            WriteOutput($"\t  ");
            for (int i = 0; i < attriCount; i++)
                WriteOutput($"Attribute{i}  ");
            WriteOutputLine("");
            for (int i = 0; i < K; i++)
            {
                WriteOutput($"Center {i}:");
                for (int j = 0; j < attriCount; j++)
                    WriteOutput($"{centers[i, j]}\t    ");
                WriteOutputLine(" ");
            }
        }
    }
}



