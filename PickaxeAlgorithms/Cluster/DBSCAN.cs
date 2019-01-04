using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using Pickaxe.Utility.ListExtension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Cluster
{
    class DBSCAN : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Cluster;

        public override string Name => "DBSCAN Cluster";

        public override string Description => "DBSCAN Cluster is a cluster stategy based on density.";

        public DBSCAN()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes take part in clustering", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Eps", "Neighbor distance threshold", typeof(float), 5.0f),
                new Option("MinPts","Number of samples in Eps-region",typeof(int),5),
            };
        }

        public override void Run()
        {
            WriteOutputLine($"Start DBSCAN clustering");
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            float eps = (float)Options[1].Value;
            int MinPts = (int)Options[2].Value;
            //点个数
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

            
            int clusterId = 1;
            List<string> clusterResult = new List<string>();
            for (int i = 0; i < tupleCount; i++)
            {
                clusterResult.Add("UNCLASSIFIED");
            }
            for (int pointId = 0; pointId < tupleCount; pointId++)
            {
                List<string> points = new List<string>();
                //string[] points = new string[pointId + 1];
               for(int i = 0; i < pointId; i++)
                {
                    points.Add(clusterResult[i]);
                }
                //points = clusterResult.Take(pointId + 1);
                if (clusterResult[pointId] == "UNCLASSIFIED")
                {
                    if (expandCluster(attributes, clusterResult.ToArray(), pointId, clusterId, eps, MinPts))
                    {
                        clusterId++;
                    }
                }
            }
            clusterId--;
            WriteOutputLine($"Finished DBSCAN clustering,number of class: {clusterId}");

            //生成新属性，保存分类结果
            var data = new ObservableCollection<Value>();
            data.Resize(tupleCount, Value.MISSING);
            var nominalType = new AttributeType.Nominal();
            for (int i = 0; i < clusterId; ++i)
            {
                nominalType.NominalLabels.Add($"Class{i}");
            }
            var result = new RelationAttribute("DBSCAN_result", nominalType, data);

            for (int i=0;i< clusterResult.Count; i++)
            {
                result.Data[i] = int.Parse(clusterResult[i]);
            }
            Relation.Add(result);
        }

        //能否成功分类
        bool expandCluster(List<RelationAttribute> attributes, string[] clusterResult, int pointId, int clusterId, float eps, int minPts)
        {
            List<int> seeds = regionQuery(attributes, pointId, eps);
            //不满足minPts条件的为噪声点
            if (seeds.Count < minPts)
            {
                clusterResult[pointId] = "NOISE";
                return false;
            }
            else
            {
                //划分到该簇
                clusterResult[pointId] = clusterId.ToString();
                for (int i = 0; i < seeds.Count; i++)
                {
                    clusterResult[i] = clusterId.ToString();
                }

                //持续扩张
                while (seeds.Count > 0)
                {
                    int curr_point = seeds[0];
                    List<int> queryResults = regionQuery(attributes, pointId, eps);
                    //是核心点
                    if (queryResults.Count >= minPts)
                    {
                        for (int i = 0; i < queryResults.Count; i++)
                        {
                            int resultPoint = queryResults[i];
                            if (clusterResult[resultPoint] == "UNCLASSIFIED")
                            {
                                seeds.Add(resultPoint);
                                clusterResult[resultPoint] = clusterId.ToString();
                            }
                            else if (clusterResult[resultPoint] == "NOISE")
                                clusterResult[resultPoint] = clusterId.ToString();
                        }
                    }
                    seeds.RemoveAt(0);
                }
                return true;
            }
        }

        //输出在eps范围内的点的id
        List<int> regionQuery(List<RelationAttribute> attributes, int pointId, float eps)
        {
            int pointCount = attributes[0].Data.Count;
            List<int> seeds = new List<int>();
            for (int i = 0; i < pointCount; i++)
            {
                float temp = 0;
                for (int j = 0; j < attributes.Count; j++)
                {
                    RelationAttribute attribute = attributes.ElementAt(j);
                    temp=(attribute.Data.ElementAt(0) - attribute.Data.ElementAt(i)) * (attribute.Data.ElementAt(pointId) - attribute.Data.ElementAt(i));
                }

                float t = float.Parse(System.Math.Sqrt(temp).ToString());
                if (epsNeighbor(t, eps))
                    seeds.Add(i);
            }
            return seeds;
        }

        //是否在eps范围内
        bool epsNeighbor(float a, float eps)
        {
            if (a < eps)
                return true;
            else return false;
        }
    }
}
