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

    public class RelationFormatter : IRelationFormatter
    {
        public RelationFormatter()
        {
        }

        public Relation Deserialize(Stream serializationStream)
        {
            var relation = new Relation();
            using (var reader = new StreamReader(serializationStream))
            {
                if (reader.ReadLine() != "@AttributesInfo")
                    throw new FormatException("Invalid pickaxe file");
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == "@Data")
                        break;
                    relation.Add(ParseAttribute(line));
                }
                int tupleIndex = 0;
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null || line == String.Empty)
                        break;
                    relation.TuplesView.Add(TupleView.Detached);
                    ParseDataLine(line, relation.TuplesView[tupleIndex]);
                    tupleIndex += 1;
                }
            }
            return relation;
        }

        private void ParseDataLine(string line, TupleView tupleView)
        {
            var splited = line.Split(new[] { ',' });
            if (splited.Length != tupleView.Count)
                throw new FormatException("data line size mismatch");
            for (var i = 0; i < splited.Length; ++i)
                tupleView[i] = Value.Parse(splited[i]);
        }

        private RelationAttribute ParseAttribute(string line)
        {
            var state = State.Start;
            var name = new StringBuilder();
            var currentLabel = new StringBuilder();
            AttributeType type = null;
            for (int i = 0; i < line.Length; ++i)
            {
                var c = line[i];
                switch (state)
                {
                    case State.Start:
                        if (c == '"')
                            state = State.InName;
                        else
                            throw new FormatException("Expect a name");
                        break;
                    case State.InName:
                        if (c == '"')
                        {
                            ++i; // ignore space
                            state = State.ExpectType;
                        }
                        else
                        {
                            name.Append(c);
                        }
                        break;
                    case State.ExpectType:
                        var typeString = line.Substring(i, 7).Trim();
                        if (typeString == "binary")
                        {
                            i += "binary".Length; // also skip space
                            type = new AttributeType.Binary();
                            state = State.ExpectTrueLabel;
                        }
                        else if (typeString == "numeric")
                        {
                            i += 7;
                            type = new AttributeType.Numeric();
                            state = State.Finished; // non-reachable
                        }
                        else if (typeString == "nominal")
                        {
                            i += "nominal".Length; // also skip space
                            type = new AttributeType.Nominal();
                            state = State.ExpectLabel;
                        }
                        break;
                    case State.ExpectTrueLabel:
                        if (c == '"')
                        {
                            state = State.InTrueLabel;
                            currentLabel.Clear();
                        }
                        else
                            throw new FormatException("Expect a true label");
                        break;
                    case State.InTrueLabel:
                        if (c == '"')
                        {
                            ((AttributeType.Binary)type).TrueLabel = currentLabel.ToString();
                            ++i; // ignore space
                            state = State.ExpectFalseLabel;
                        }
                        else
                        {
                            currentLabel.Append(c);
                        }
                        break;
                    case State.ExpectFalseLabel:
                        if (c == '"')
                        {
                            state = State.InFalseLabel;
                            currentLabel.Clear();
                        }
                        else
                            throw new FormatException("Expect a false label");
                        break;
                    case State.InFalseLabel:
                        if (c == '"')
                        {
                            ((AttributeType.Binary)type).FalseLabel = currentLabel.ToString();
                            state = State.Finished;
                        }
                        else
                        {
                            currentLabel.Append(c);
                        }
                        break;
                    case State.ExpectLabel:
                        if (c == '"')
                        {
                            state = State.InLabel;
                            currentLabel.Clear();
                        }
                        else
                            throw new FormatException("Expect a label");
                        break;
                    case State.InLabel:
                        if (c == '"')
                        {
                            ((AttributeType.Nominal)type).NominalLabels.Add(currentLabel.ToString());
                            ++i; // skip space
                            state = State.ExpectLabel;
                        }
                        else
                        {
                            currentLabel.Append(c);
                        }
                        break;
                    case State.Finished:
                        throw new FormatException("Expect new line");
                }
            }
            return new RelationAttribute(name.ToString(), type, new ObservableCollection<Value>());
        }

        public void Serialize(Stream serializationStream, Relation relation)
        {
            using (var writer = new StreamWriter(serializationStream))
            {
                writer.WriteLine("@AttributesInfo");
                foreach (var attribute in relation)
                {
                    writer.Write('"');
                    writer.Write(attribute.Name);
                    writer.Write('"');
                    writer.Write(' ');
                    if (attribute.Type is AttributeType.Numeric)
                    {
                        writer.WriteLine("numeric");
                    }
                    else if (attribute.Type is AttributeType.Binary binary)
                    {
                        writer.Write("binary ");
                        writer.Write('"');
                        writer.Write(binary.TrueLabel);
                        writer.Write('"');
                        writer.Write(' ');
                        writer.Write('"');
                        writer.Write(binary.FalseLabel);
                        writer.Write('"');
                        writer.WriteLine();
                    }
                    else if (attribute.Type is AttributeType.Nominal nominal)
                    {
                        writer.Write("nominal ");
                        foreach (var label in nominal.NominalLabels)
                        {
                            writer.Write('"');
                            writer.Write(label);
                            writer.Write('"');
                            writer.Write(' ');
                        }
                        writer.WriteLine();
                    }
                }
                writer.WriteLine("@Data");
                foreach (var tupleView in relation.TuplesView)
                {
                    for (int i = 0; i < relation.Count; ++i)
                    {
                        writer.Write(tupleView[i].ToString());
                        if (i != relation.Count - 1)
                            writer.Write(',');
                    }
                    writer.WriteLine();
                }
            }
        }

        private enum State
        {
            Start,
            InName,
            ExpectType,
            ExpectTrueLabel,
            InTrueLabel,
            ExpectFalseLabel,
            InFalseLabel,
            ExpectLabel,
            InLabel,
            Finished,
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
                                    if (!v.IsMissing())
                                    {
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
