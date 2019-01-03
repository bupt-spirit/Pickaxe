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
            var attributes = (IEnumerable<RelationAttribute>)Options[0].Value;
            var label = (RelationAttribute)Options[1].Value;
        }
    }
}
