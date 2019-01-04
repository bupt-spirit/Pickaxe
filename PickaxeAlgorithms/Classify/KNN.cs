using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Preprocess.Classify
{
    class KNN : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Classify;
        public override string Name => "K-nearest neighbors algorithm(KNN)";
        public override string Description => "KNN is to classify values into k classes.";

        public KNN()
        {
            Options = new System.Collections.ObjectModel.ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes to be classified", typeof(IEnumerable<RelationAttribute>), null),
                //new Option("Label ")
                new Option("K Number", "Class number", typeof(int), 3),
            };
        }

        public override void Run()
        {
            WriteOutputLine($"Classifying...");

            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            var k = (int)Options[1].Value;
            int count = attributes.ElementAt(0).Data.Count();
            double[] dis = new double[count];
            dis[0] = 0;

            //假设第一个tuple为测试集            
            //最后一列为label


            for (int i = 1; i < count; i++)
            {
                double temp = 0;
                for (int m = 0; m < attributes.Count() - 1; m++)
                {
                    RelationAttribute attribute = attributes.ElementAt(m);
                    temp += ((attribute.Data.ElementAt(0) - attribute.Data.ElementAt(i)) * (attribute.Data.ElementAt(0) - attribute.Data.ElementAt(i)));
                }
                dis[i] = System.Math.Sqrt(temp);
            }

            Dictionary<int, double> dis_with_index = new Dictionary<int, double>();
            dis_with_index.Add(0, 0);
            for (int j = 1; j < count; j++)
            {
                dis_with_index.Add(j, dis[j]);
            }

            //按距离从大到小排列，取前k个
            dis_with_index.OrderByDescending(p => p.Key);
            dis_with_index.Take(k + 1);

            //计算前k个中label个数
            List<Value> label = new List<Value>();
            int[] label_count = new int[count + 1];
            //初始化
            for (int i = 0; i < count; i++)
            {
                label_count[1] = 0;
            }

            for (int i = 1; i < dis_with_index.Count() + 1; i++)
            {
                Value temp = attributes.Last().Data.ElementAt(i - 1);
                if (label.Contains(temp))
                {
                    int index = label.IndexOf(temp);
                    label_count[index]++;
                }
                else
                {
                    label.Add(temp);
                    int index = label.IndexOf(temp);
                    label_count[index]++;//1
                }
            }

            Dictionary<int, Value> label_with_index = new Dictionary<int, Value>();
            //label_with_index.Add(0, 0);
            for (int j = 0; j < label.Count(); j++)
            {
                label_with_index.Add(j, label[j]);
            }

            label_with_index.OrderByDescending(p => p.Key);
            label_with_index.ToList();

            WriteOutputLine($"Finished classifying,label is {label_with_index[1]}");
        }


    }
}
