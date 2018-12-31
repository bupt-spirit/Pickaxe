using System;

namespace Pickaxe.Model
{
    [Serializable]
    public struct Value : IEquatable<Value>, IComparable<Value>
    {
        public static Value MISSING = (Value)float.NaN;

        private float inner;

        public static implicit operator Value(float f) => new Value { inner = f };

        public static implicit operator float(Value v)
        {
            if (v.IsMissing())
                throw new InvalidCastException("cast missing value");
            return v.inner;
        }

        public static Value ToValue<T>(T from) => (Value)Convert.ToSingle(from);

        public int CompareTo(Value other) => this.inner.CompareTo(other.inner);

        public bool Equals(Value other)
        {
            return (this.IsMissing() && other.IsMissing()) || this.inner.Equals(other.inner);
        }

        public override string ToString()
        {
            if (this.IsMissing())
                return "MISSING";
            else
                return this.inner.ToString();
        }

        public bool IsMissing()
        {
            return float.IsNaN(this.inner);
        }

        public static Value Parse(string s)
        {
            if (Single.TryParse(s, out float f))
            {
                return Value.ToValue(f);
            }
            else
            {
                return Value.MISSING;
            }
        }
    }
}
