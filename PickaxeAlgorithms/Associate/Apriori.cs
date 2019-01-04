using Pickaxe.AlgorithmFramework;
using Pickaxe.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeAlgorithms.Associate
{
    class Apriori : AlgorithmBase
    {
        public override AlgorithmType Type => AlgorithmType.Associate;

        public override string Name => "Apriori";

        public override string Description => "Apriori association try to find every k-1 frequent item set to get k frequent item set";

        public Apriori()
        {
            Options = new ObservableCollection<Option>
            {
                new Option("Attributes", "Attributes take part in Apriori association caculate", typeof(IEnumerable<RelationAttribute>), null),
                new Option("Support","Soppurt threshold value",typeof(float),null),
                new Option("Confidence","Confidence threshold value",typeof(float),null),
            };
        }

        public override void Run()
        {
            var attributes = ((IEnumerable<RelationAttribute>)Options[0].Value).ToList();
            if (attributes == null) {
                WriteOutputLine($"Error:Please choose participating attributes!!!");
                return;
            }
            var support = (float)Options[1].Value;
            var confidence = (float)Options[2].Value;
            var tupleCount = attributes[0].Data.Count;
            var apriori = new Apriori_Run((int)(support * tupleCount), confidence, attributes);
            Dictionary<String, int> frequentCollection = apriori.get();
            WriteOutputLine("----------------Frequent Item Set" + "----------------");
            foreach (KeyValuePair<string, int> kvp in frequentCollection)//KeyValuePair只包含一个键值对用于遍历
            {
                WriteOutputLine(kvp.Key + ":" + kvp.Value);
            }
            Dictionary<String, Double> relationRules = apriori.Rules(frequentCollection);
            WriteOutputLine("----------------Association Rules" + "----------------");
            foreach (KeyValuePair<string, double> kvp in relationRules)
            {
                WriteOutputLine(kvp.Key + ":" + kvp.Value);
            }
        }
        public class Apriori_Run
        {
            private static int support; // 支持度阈值
            private static float confidence; // 置信度阈值
            private static char[] item_Split = { ',' }; // 项之间的分隔符
            private static string itemSplit = ",";//项中元素之间的分隔符
            private static String CON = "->"; // 项之间的分隔符
            private static List<String> transList = new List<String>(); //所有初始数据
            public Apriori_Run(int min_sup, float min_conf, IEnumerable<RelationAttribute> attributes)//构造函数,在里面初始支持度和置信度的阈值
            {
                //初始化数据记录
                var data = attributes.ToList();
                var tupleCount = data[0].Data.Count;
                var attriCount = data.Count;
                var attriNames = new string[attriCount];
                for (int i = 0; i < attriCount; i++)
                    attriNames[i] = data[i].Name;
                for (int i = 0; i < tupleCount; i++)
                {
                    string temp = "";
                    for (int j = 0; j < attriCount; j++)
                        if (data[j].Data[i] == (Value)1)
                            temp += attriNames[j] + ",";
                    transList.Add(temp);
                }
                support = min_sup;
                confidence = min_conf;
            }
            private Dictionary<String, int> getItem() //计算所有频繁1项集
            {
                Dictionary<String, int> mItem = new Dictionary<String, int>();//全部1项集
                Dictionary<String, int> Items = new Dictionary<String, int>(); //频繁1项集
                foreach (String trans in transList)//遍历列表
                {
                    String[] items = trans.Split(item_Split, StringSplitOptions.RemoveEmptyEntries);//将字符串转化为数组，加StringSplitOptions.RemoveEmptyEntries保证返回的数组中无空元素
                    foreach (String item in items)//遍历数组
                    {
                        int count;
                        if (mItem.ContainsKey(item + itemSplit) == true)//判断字典中是否含有指定键名
                        {
                            //如果有就删除这个键值对，并重新添加这个键值对(新的value加一)
                            count = mItem[item + itemSplit];//直接通过key获取value
                            mItem.Remove(item + itemSplit);
                            mItem.Add(item + itemSplit, count + 1);
                        }
                        else
                        {
                            //如果没有就添加该条记录
                            mItem.Add(item + itemSplit, 1);
                        }
                    }
                }
                List<String> keySet = mItem.Keys.ToList();//获取字典sItem1Fc的key的集合
                foreach (String key in keySet)
                {
                    int count = mItem[key];//直接通过key获取value
                    if (count >= support)
                    {
                        Items.Add(key, count);
                    }
                }
                return Items;
            }
            private Dictionary<String, int> getCollection(Dictionary<String, int> item) //生成候选项集
            {
                Dictionary<String, int> dateCollection = new Dictionary<String, int>();//候选项集
                List<String> itemSet1 = item.Keys.ToList();
                List<String> itemSet2 = item.Keys.ToList();//获取字典sItem1Fc的key的集合
                foreach (String item1 in itemSet1)//遍历获取的key的集合
                {
                    foreach (String item2 in itemSet2)
                    {
                        //进行连接
                        String[] tmp1 = item1.Split(item_Split, StringSplitOptions.RemoveEmptyEntries);
                        String[] tmp2 = item2.Split(item_Split, StringSplitOptions.RemoveEmptyEntries);//将字符串转化为数组，加StringSplitOptions.RemoveEmptyEntries保证返回的数组中无空元素
                        String c = "";//定义一个空字符串用来接收中间产生的项集字符串
                        if (tmp1.Length == 1)//如果是1项集
                        {
                            if (tmp1[0].CompareTo(tmp2[0]) < 0)//按字母大小和长度进行连接(这里tmp1[0]首字母小于tmp2[0]的首字母)
                            {
                                c = tmp1[0] + itemSplit + tmp2[0] + itemSplit;
                            }
                        }
                        else
                        {
                            bool flag = true;// 用来判断连接时两个元素是否相同
                            for (int i = 0; i < tmp1.Length - 1; i++)
                            {
                                if (tmp1[i].Equals(tmp2[i]) == false)//因为集合中元素是按首字母大小排序的，需判断两个集合的首个元素是否相同，避免重复连接
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag == true && (tmp1[tmp1.Length - 1].CompareTo(tmp2[tmp2.Length - 1]) < 0))//按字母大小和长度进行连接(这里tmp1[tmp1.Length - 1]首字母小于tmp2[tmp2.Length - 1]的首字母)
                            {
                                c = item1 + tmp2[tmp2.Length - 1] + itemSplit;
                            }
                        }
                        bool Set = false;
                        if (c != "")
                        {
                            String[] tmpC = c.Split(item_Split, StringSplitOptions.RemoveEmptyEntries);//将字符串转化为数组，加StringSplitOptions.RemoveEmptyEntries保证返回的数组中无空元素
                            for (int i = 0; i < tmpC.Length; i++)
                            {
                                String subC = "";//中间字符串
                                for (int j = 0; j < tmpC.Length; j++)
                                {
                                    if (i != j)//不能让同一个元素连接自己，排除重复元素
                                    {
                                        subC = subC + tmpC[j] + itemSplit;
                                    }
                                }
                                if (item.ContainsKey(subC) == false)//判断传过来的字典中是否含有指定键名，没有就添加
                                {
                                    Set = false;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Set = true;
                        }
                        if (Set == false)
                        {
                            dateCollection.Add(c, 0);
                        }
                    }
                }
                return dateCollection;
            }
            public Dictionary<String, int> get()//计算所有频繁项集
            {
                Dictionary<String, int> Collections = new Dictionary<String, int>();//所有的频繁集
                foreach (KeyValuePair<string, int> item in getItem())//遍历频繁1项集
                {
                    Collections.Add(item.Key, item.Value);//添加频繁1项集至所有的频繁集
                }
                Dictionary<String, int> itemkFc = new Dictionary<String, int>();//中间字典
                foreach (KeyValuePair<string, int> item in getItem())
                {
                    itemkFc.Add(item.Key, item.Value);//添加1项集至中间字典
                }
                while (itemkFc != null && itemkFc.Count != 0)
                {
                    Dictionary<String, int> candidateCollection = getCollection(itemkFc);//循环调用生成的候选项集
                    List<String> ccKeySet = candidateCollection.Keys.ToList();//获取候选项集的key的集合
                                                                              //对候选集项进行累加计数
                    foreach (String trans in transList)//遍历初始数据
                    {
                        foreach (String candidate in ccKeySet)//遍历候选项集的key的集合
                        {
                            bool flag = true; // 用来判断初始数据中是否出现该候选项，如果出现，计数加1
                            String[] candidateItems = candidate.Split(item_Split, StringSplitOptions.RemoveEmptyEntries);
                            foreach (String candidateItem in candidateItems)//候选项集中每个元素都和初始数据中的每条记录的每个元素对比
                            {
                                if (trans.IndexOf(candidateItem + itemSplit) == -1)//在初始数据每条记录中查找字段，如果没找到则为-1
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag == true)
                            {
                                int count = candidateCollection[candidate];//直接通过key获取value
                                candidateCollection.Remove(candidate);
                                candidateCollection.Add(candidate, count + 1);
                            }
                        }
                    }
                    //从候选集中找到符合支持度的频繁集项
                    itemkFc.Clear();
                    foreach (String candidate in ccKeySet)
                    {
                        int count = candidateCollection[candidate];
                        if (count >= support)
                        {
                            itemkFc.Add(candidate, count);
                        }
                    }
                    //合并所有频繁集
                    foreach (KeyValuePair<string, int> item in itemkFc)
                    {
                        if (Collections.ContainsKey(item.Key))
                        {
                            Collections.Remove(item.Key);
                        }
                        Collections.Add(item.Key, item.Value);
                    }
                }
                return Collections;
            }
            private void build(List<String> sourceSet, List<List<String>> result) //建立频繁项集的子集
            {
                // 仅有一个元素时，递归终止。此时非空子集仅为其自身，所以直接添加到result中
                if (sourceSet.Count == 1)
                {
                    List<String> set = new List<String>();
                    set.Add(sourceSet[0]);
                    result.Add(set);
                }
                else if (sourceSet.Count > 1)
                {
                    // 当有n个元素时，递归求出前n-1个子集，在于result中
                    build(sourceSet.Take(sourceSet.Count - 1).ToList(), result);
                    int size = result.Count;// 求出此时result的长度，用于后面的追加第n个元素时计数
                                            // 把第n个元素加入到集合中
                    List<String> single = new List<String>();
                    single.Add(sourceSet[sourceSet.Count - 1]);
                    result.Add(single);
                    // 在保留前面的n-1子集的情况下，把第n个元素分别加到前n个子集中，并把新的集加入到result中;
                    // 为保留原有n-1的子集，所以需要先对其进行复制
                    List<String> clone;//中间列表
                    for (int i = 0; i < size; i++)
                    {
                        clone = new List<String>();
                        foreach (String str in result[i])
                        {
                            clone.Add(str);
                        }
                        clone.Add(sourceSet[sourceSet.Count - 1]);
                        result.Add(clone);
                    }
                }
            }
            public Dictionary<String, Double> Rules(Dictionary<String, int> Collection) //计算关联规则
            {
                Dictionary<String, Double> Rule = new Dictionary<String, Double>();
                List<String> keySet = Collection.Keys.ToList();
                foreach (String key in keySet)
                {
                    double countAll = Collection[key];//直接通过key获取value
                    String[] keyItems = key.Split(item_Split, StringSplitOptions.RemoveEmptyEntries);
                    if (keyItems.Length > 1)
                    {
                        List<String> source = keyItems.ToList();

                        //Collections.addAll(source, keyItems);
                        List<List<String>> result = new List<List<String>>();
                        build(source, result); //获得source的所有非空子集
                        foreach (List<String> itemList in result)
                        {
                            if (itemList.Count < source.Count)
                            {   //只处理真子集
                                List<String> otherList = new List<String>();
                                foreach (String sourceItem in source)
                                {
                                    if (!itemList.Contains(sourceItem))
                                    {
                                        otherList.Add(sourceItem);
                                    }
                                }
                                String reasonStr = "";//前置
                                String resultStr = "";//结果
                                foreach (String item in itemList)
                                {
                                    reasonStr = reasonStr + item + itemSplit;
                                }
                                foreach (String item in otherList)
                                {
                                    resultStr = resultStr + item + itemSplit;
                                }
                                double countReason = Collection[reasonStr];//直接通过key获取value
                                double itemConfidence = countAll / countReason;//计算置信度
                                if (itemConfidence >= confidence)
                                {
                                    String rule = reasonStr + CON + resultStr;
                                    //relationRules.Remove(rule);
                                    Rule.Add(rule, itemConfidence);
                                }
                            }
                        }
                    }
                }
                return Rule;
            }
        }
    }
}