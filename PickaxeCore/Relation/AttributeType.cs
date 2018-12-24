using System;
using System.Collections.Generic;
using static System.Linq.Enumerable;

namespace PickaxeCore.Relation
{
    /// <summary>
    /// class AttributeType is the base of all types of attributes of PickaxeCore.
    /// </summary>
    public abstract class AttributeType
    {
        // prevent other inherit
        private AttributeType() { }

        public abstract bool ValidateValue(Value value);
        public bool ValidateValueWithMissing(Value value) => value.IsMissing() || this.ValidateValue(value);
        public abstract string ValueToString(Value value);

        public class Numeric : AttributeType
        {
            public override bool ValidateValue(Value value) => !value.IsMissing();
            public override string ValueToString(Value value) => value.ToString();
        }

        public class Binary : AttributeType
        {
            public override bool ValidateValue(Value value) {
                return !value.IsMissing() && (value == Value.ToValue(true) || value == Value.ToValue(false));
            }

            public override string ValueToString(Value value)
            {
                if (value.IsMissing())
                    return value.ToString();
                else if (value == Value.ToValue(true))
                    return TrueLabel;
                else if (value == Value.ToValue(false))
                    return FalseLabel;
                else
                    throw new ArgumentException("invalid binary value");
            }

            public string TrueLabel { get; set; }
            public string FalseLabel { get; set; }

            // Numeric is just a tag, contains no other information
            public Binary(string trueLabel, string falseLabel)
            {
                this.TrueLabel = trueLabel;
                this.FalseLabel = falseLabel;
            }

            public Binary() : this("true", "false")
            {
            }
        }

        // Nominal contains name of every nominal
        public class Nominal : AttributeType
        {
            public Nominal()
            {
                this.nominalValueIndex = new Dictionary<string, int>();
                this.nominalLabels = new List<string>();
            }

            public Nominal(IEnumerable<string> nominalValues) : this()
            {
                foreach (var nominalValue in nominalValues)
                {
                    this.Add(nominalValue);
                }
            }

            private List<string> nominalLabels;
            private Dictionary<string, int> nominalValueIndex;
            public int Count { get => nominalLabels.Count; }

            public override bool ValidateValue(Value value)
            {
                if (value.IsMissing())
                    return false;
                try
                {
                    var index = ValueToIndex(value);
                    return index < this.Count;
                } catch (ArgumentException)
                {
                    return false;
                }
            }

            public override string ValueToString(Value value)
            {
                if (value.IsMissing())
                    return value.ToString();
                else
                    return nominalLabels[ValueToIndex(value)];
            }

            public void Add(string label)
            {
                if (nominalValueIndex.ContainsKey(label))
                {
                    throw new System.ArgumentException(
                        $"nominal label {label} already exists"
                        );
                }
                var inInt32 = nominalLabels.Count;
                nominalLabels.Add(label);
                nominalValueIndex[label] = inInt32;
            }


            public void Remove(string label, IList<Value> values)
            {
                var index = nominalValueIndex[label];
                nominalValueIndex.Remove(label);
                nominalLabels.RemoveAt(index);
                this.RemoveMapValue(index, values);
            }

            public void Remove(Value value, IList<Value> values)
            {
                var index = ValueToIndex(value);
                var label = nominalLabels[index];
                nominalValueIndex.Remove(label);
                nominalLabels.RemoveAt(index);
                this.RemoveMapValue(index, values);
            }

            private void RemoveMapValue(int removedIndex, IList<Value> values)
            {
                foreach (var i in Range(0, values.Count))
                {
                    var current = values[i];
                    var removed = Value.ToValue(removedIndex);
                    if (current.IsMissing() || current < removed)
                    {
                        // do nothing
                    }
                    else if (current == removed)
                    {
                        // remove value
                        values[i] = Value.MISSING;
                    }
                    else if (current > removed && current < this.Count + 1)
                    {
                        values[i] = current - 1;
                    }
                    else
                    {
                        throw new System.ArgumentException($"value {current} in list is invalid for attribute");
                    }
                }
            }

            public string this[Value nominalValue]
            {
                get
                {
                    return nominalLabels[ValueToIndex(nominalValue)];
                }
                set
                {
                    nominalLabels[ValueToIndex(nominalValue)] = value;
                }
            }

            public Value this[string label]
            {
                get
                {
                    return Value.ToValue(nominalValueIndex[label]);
                }
            }

            private static int ValueToIndex(Value value)
            {
                var inDecimal = Math.Floor(value);
                if (inDecimal != value)
                {
                    throw new System.ArgumentException(
                        $"nominal value {value} is not an integer"
                        );
                }
                var inInt32 = Convert.ToInt32(inDecimal);
                if (inInt32 != inDecimal)
                {
                    throw new System.ArgumentException(
                        $"nominal value {value} is too large"
                        );
                }
                if (inInt32 < 0)
                {
                    throw new System.ArgumentException(
                        $"nominal value {value} is less than zero"
                        );
                }
                return inInt32;
            }
        }
    }
}
