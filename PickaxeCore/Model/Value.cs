using System;
using System.Globalization;

namespace Pickaxe.Model
{
    [Serializable]
    public struct Value : IEquatable<Value>, IComparable<Value>
    {
        public static Value MISSING = (Value)float.NaN;

        private float _inner;

        public static implicit operator Value(float f) => new Value { _inner = f };

        public static implicit operator float(Value v)
        {
            if (v.IsMissing())
                throw new InvalidCastException("cast missing value");
            return v._inner;
        }

        public static Value ToValue<T>(T from) => (Value)Convert.ToSingle(from);

        public int CompareTo(Value other) => this._inner.CompareTo(other._inner);

        public bool Equals(Value other)
        {
            return (this.IsMissing() && other.IsMissing()) || this._inner.Equals(other._inner);
        }

        public override string ToString()
        {
            if (this.IsMissing())
                return "MISSING";
            else
                return this._inner.ToString(CultureInfo.CurrentCulture);
        }

        public bool IsMissing()
        {
            return float.IsNaN(this._inner);
        }

        public static Value Parse(string s)
        {
            return float.TryParse(s, out var f) ? Value.ToValue(f) : Value.MISSING;
        }
    }
}
