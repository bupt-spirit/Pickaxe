using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickaxeCore.Relation
{
    public struct Value : IFormattable, IEquatable<Value>, IComparable<Value>
    {
        public static Value MISSING = (Value) float.NaN;

        private float inner;

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

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (this.IsMissing())
                return "MISSING";
            else
                return this.inner.ToString(format, formatProvider);
        }

        public static implicit operator Value(float f) => new Value { inner = f };

        public static implicit operator float(Value v)
        {
            if (v.IsMissing())
                throw new InvalidCastException("cast missing value");
            return v.inner;
        }

        public bool IsMissing()
        {
            return float.IsNaN(this.inner);
        }

        public static Value Parse(string s)
        {
            float f;
            if (Single.TryParse(s, out f))
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
