using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
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
                var dictionaries = new HashSet<string>[relation.Count];

                var isNominal = new bool[relation.Count];
                for (int i = 0; i < relation.Count; ++i)
                {
                    dictionaries[i] = new HashSet<string>();
                    isNominal[i] = false;
                }
                while (!parser.EndOfData)
                {
                    tuplesView.Add(TupleView.Detached);
                    string[] fields = parser.ReadFields();
                    for (int i = 0; i < relation.Count; ++i)
                    {
                        var field = fields[i].Trim();
                        if (isNominal[i])
                        {
                            if (dictionaries[i].Contains(field))
                            {
                                tuplesView[tupleIndex][i] = ((AttributeType.Nominal)relation[i].Type).NominalLabels.IndexOf(field);
                            }
                            else
                            {
                                var labels = ((AttributeType.Nominal)relation[i].Type).NominalLabels;
                                var index = labels.Count;
                                labels.Add(field);
                                dictionaries[i].Add(field);
                                tuplesView[tupleIndex][i] = Value.ToValue(index);
                            }
                        }
                        else
                        {
                            if (field == String.Empty)
                            {
                                tuplesView[tupleIndex][i] = Value.MISSING;
                            }
                            if (float.TryParse(fields[i], out var inFloat))
                            {
                                tuplesView[tupleIndex][i] = inFloat;
                            }
                            else
                            {
                                foreach (var v in relation[i].Data)
                                {
                                    if (!v.IsMissing()) {
                                        throw new FormatException("Can not determine attribute type from csv");
                                    }
                                }
                                relation[i].Type = new AttributeType.Nominal();
                                isNominal[i] = true;
                                --i; // reprocess this field
                            }
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
