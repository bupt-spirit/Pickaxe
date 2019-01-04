using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Classify
{
    class ID3 : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Classify;

        public override string Name => "ID3";

        public override string Description => "ID3 is a classic classify algorithm based on decision-making tree" +
            "It makes decision by caculationg information degree and information gain, chooses max one for further decision";

        public ID3()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes take part in ID3 classfication", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Label","Attribute that mark different class",typeof(RelationAttribute),null),
            };
        }

        public override void Run()
        {
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            var label = (RelationAttribute)Options[1].Value;
            var tupleCount = attributes[0].Data.Count;
            var attriCount = attributes.Count;
            var attriNames = new string[attriCount + 1];
            var data = new string[tupleCount, attriCount + 1];
            var NominalLabels = ((AttributeType.Nominal)label.Type).NominalLabels;
            for (int j = 0; j < tupleCount; j++)
            {
                if (label.Data[j].IsMissing())
                {
                    WriteOutputLine($"Error:Label value do not allow missing!");
                    return;
                }
                data[j, attriCount] = NominalLabels[(int)label.Data[j]];
            }
            for (int i = 0; i < attriCount; i++)
            {
                if (!(attributes[i].Type is AttributeType.Nominal))
                {
                    WriteOutputLine($"Error:Only Nominal type is allowed");
                    return;
                }
                NominalLabels = ((AttributeType.Nominal)attributes[i].Type).NominalLabels;
                // new ObservableCollection<string>();
                for (int j = 0; j < tupleCount; j++)
                {
                    if (attributes[i].Data[j].IsMissing())
                    {
                        WriteOutputLine($"Error:Do not allow missing!");
                        return;
                    }
                    data[j, i] = NominalLabels[(int)attributes[i].Data[j]];
                }
                attriNames[i] = attributes[i].Name;
            }
            var tree = new DecisionTreeID3<string>(data, attriNames, new string[] { "yes", "no" });
            tree.Learn();
            WriteOutputLine(tree.Output);
        }


        class DecisionTreeNode<T>
        {
            public int Label { get; set; }
            public T Val { get; set; }
            public List<DecisionTreeNode<T>> Children { get; set; }
            public DecisionTreeNode(int label, T val)
            {
                Label = label;
                Val = val;
                Children = new List<DecisionTreeNode<T>>();
            }
        }

        class DecisionTreeID3<T> where T : IEquatable<T>
        {
            T[,] Data;
            string[] Names;
            int Category;
            T[] CategoryLabels;
            DecisionTreeNode<T> Root;
            public string Output { get; set; }
            public DecisionTreeID3(T[,] data, string[] names, T[] categoryLabels)
            {
                Data = data;
                Names = names;
                Category = data.GetLength(1) - 1;//类别变量需要放在最后一列
                CategoryLabels = categoryLabels;
                Output = "ID3 Classify:\n";
            }
            public void Learn()
            {
                int nRows = Data.GetLength(0);
                int nCols = Data.GetLength(1);
                int[] rows = new int[nRows];
                int[] cols = new int[nCols];
                for (int i = 0; i < nRows; i++) rows[i] = i;
                for (int i = 0; i < nCols; i++) cols[i] = i;
                Root = new DecisionTreeNode<T>(-1, default(T));
                Learn(rows, cols, Root);
                DisplayNode(Root);
            }
            public void DisplayNode(DecisionTreeNode<T> Node, int depth = 0)
            {
                if (Node.Label != -1)
                    Output += ($"{ new string('-', depth * 3)} { Names[Node.Label]}:{ Node.Val}\n");
                foreach (var item in Node.Children)
                    DisplayNode(item, depth + 1);
            }
            private void Learn(int[] pnRows, int[] pnCols, DecisionTreeNode<T> Root)
            {
                var categoryValues = GetAttribute(Data, Category, pnRows);
                var categoryCount = categoryValues.Distinct().Count();
                if (categoryCount == 1)
                {
                    var node = new DecisionTreeNode<T>(Category, categoryValues.First());
                    Root.Children.Add(node);
                }
                else
                {
                    if (pnRows.Length == 0) return;
                    else if (pnCols.Length == 1)
                    {
                        //投票～
                        //多数票表决制
                        var Vote = categoryValues.GroupBy(i => i).OrderBy(i => i.Count()).First();
                        var node = new DecisionTreeNode<T>(Category, Vote.First());
                        Root.Children.Add(node);
                    }
                    else
                    {
                        var maxCol = MaxEntropy(pnRows, pnCols);
                        var attributes = GetAttribute(Data, maxCol, pnRows).Distinct();
                        string currentPrefix = Names[maxCol];
                        foreach (var attr in attributes)
                        {
                            int[] rows = pnRows.Where(irow => Data[irow, maxCol].Equals(attr)).ToArray();
                            int[] cols = pnCols.Where(i => i != maxCol).ToArray();
                            var node = new DecisionTreeNode<T>(maxCol, attr);
                            Root.Children.Add(node);
                            Learn(rows, cols, node);//递归生成决策树
                        }
                    }
                }
            }
            public double AttributeInfo(int attrCol, int[] pnRows)
            {
                var tuples = AttributeCount(attrCol, pnRows);
                var sum = (double)pnRows.Length;
                double Entropy = 0.0;
                foreach (var tuple in tuples)
                {
                    int[] count = new int[CategoryLabels.Length];
                    foreach (var irow in pnRows)
                        if (Data[irow, attrCol].Equals(tuple.Item1))
                        {
                            int index = Array.IndexOf(CategoryLabels, Data[irow, Category]);
                            count[index]++;//目前仅支持类别变量在最后一列
                        }
                    double k = 0.0;
                    for (int i = 0; i < count.Length; i++)
                    {
                        double frequency = count[i] / (double)tuple.Item2;
                        double t = -frequency * Log2(frequency);
                        k += t;
                    }
                    double freq = tuple.Item2 / sum;
                    Entropy += freq * k;
                }
                return Entropy;
            }
            public double CategoryInfo(int[] pnRows)
            {
                var tuples = AttributeCount(Category, pnRows);
                var sum = (double)pnRows.Length;
                double Entropy = 0.0;
                foreach (var tuple in tuples)
                {
                    double frequency = tuple.Item2 / sum;
                    double t = -frequency * Log2(frequency);
                    Entropy += t;
                }
                return Entropy;
            }
            private static IEnumerable<T> GetAttribute(T[,] data, int col, int[] pnRows)
            {
                foreach (var irow in pnRows)
                    yield return data[irow, col];
            }
            private static double Log2(double x)
            {
                return x == 0.0 ? 0.0 : Math.Log(x, 2.0);
            }
            public int MaxEntropy(int[] pnRows, int[] pnCols)
            {
                double cateEntropy = CategoryInfo(pnRows);
                int maxAttr = 0;
                double max = double.MinValue;
                foreach (var icol in pnCols)
                    if (icol != Category)
                    {
                        double Gain = cateEntropy - AttributeInfo(icol, pnRows);
                        if (max < Gain)
                        {
                            max = Gain;
                            maxAttr = icol;
                        }
                    }
                return maxAttr;
            }
            public IEnumerable<Tuple<T, int>> AttributeCount(int col, int[] pnRows)
            {
                var tuples = from n in GetAttribute(Data, col, pnRows)
                             group n by n into i
                             select Tuple.Create(i.First(), i.Count());
                return tuples;
            }
        }
    }
}