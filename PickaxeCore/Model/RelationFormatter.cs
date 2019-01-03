using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Pickaxe.Model
{
    public interface IRelationFormatter
    {
        Relation Deserialize(Stream serializationStream);
        void Serialize(Stream serializationStream, Relation relation);
    }

    public class BinaryRelationFormatter : IRelationFormatter
    {
        private readonly BinaryFormatter _formatter;

        public BinaryRelationFormatter()
        {
            _formatter = new BinaryFormatter();
        }

        public Relation Deserialize(Stream serializationStream)
        {
            var relation = (Relation)_formatter.Deserialize(serializationStream);
            relation.RebindInternalEvents();
            return relation;
        }

        public void Serialize(Stream serializationStream, Relation relation)
        {
            _formatter.Serialize(serializationStream, relation);
        }
    }

    public class CSVRelationFormatter : IRelationFormatter
    {
        public Relation Deserialize(Stream serializationStream)
        {
            var relation = new Relation();
            using (var parser = new TextFieldParser(serializationStream, Encoding.UTF8, true))
            {
                parser.SetDelimiters(new string[] { "," });
                parser.HasFieldsEnclosedInQuotes = true;
                string[] names = parser.ReadFields();
                foreach (var name in names)
                    relation.Add(new RelationAttribute(name, new AttributeType.Numeric(), new ObservableCollection<Value>()));
                var tuplesView = relation.TuplesView;
                var tupleIndex = 0;
                while (!parser.EndOfData)
                {
                    tuplesView.Add(TupleView.Detached);
                    string[] fields = parser.ReadFields();
                    for (int i = 0; i < relation.Count; ++i)
                    {
                        if (float.TryParse(fields[i], out var inFloat))
                        {
                            tuplesView[tupleIndex][i] = inFloat;
                        }
                        else
                        {
                            tuplesView[tupleIndex][i] = Value.MISSING;
                        }
                    }
                    ++tupleIndex;
                }
            }
            return relation;
        }

        public void Serialize(Stream serializationStream, Relation relation)
        {
            throw new NotSupportedException();
        }
    }
}
