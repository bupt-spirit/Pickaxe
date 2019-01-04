using Pickaxe.Model;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Pickaxe.Utility.Converter
{
    class ValueStringConverter : IValueConverter
    {
        public AttributeType AttributeType { get; set; }

        public ValueStringConverter(AttributeType attributeType)
        {
            this.AttributeType = attributeType;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Value inValue)
                return AttributeType.ValueToString(inValue);
            else
                throw new ArgumentException("not a value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class IntWithDefaultStringConverter : IValueConverter
    {
        public int DefaultValue { get; set; }

        public IntWithDefaultStringConverter(int defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int inInt)
                return inInt.ToString();
            else
                throw new ArgumentException("not a int value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string inString)
            {
                if (int.TryParse(inString, out var inInt))
                    return inInt;
                else
                    return DefaultValue;
            }
            else
            {
                throw new ArgumentException("not a string value");
            }
        }
    }

    class FloatWithDefaultStringConverter : IValueConverter
    {
        public float DefaultValue { get; set; }

        public FloatWithDefaultStringConverter(float defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float inFloat)
                return inFloat.ToString();
            else
                throw new ArgumentException("not a int value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string inString)
            {
                if (float.TryParse(inString, out var inFloat))
                    return inFloat;
                else
                    return DefaultValue;
            }
            else
            {
                throw new ArgumentException("not a string value");
            }
        }
    }

    class AttributeTypeStringConverter : IValueConverter
    {
        public static AttributeType.Numeric numericAttributeType = new AttributeType.Numeric();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AttributeType.Binary)
                return "Binary";
            else if (value is AttributeType.Nominal)
                return "Nominal";
            else if (value is AttributeType.Numeric)
                return "Numeric";
            else
                throw new ArgumentException("not a value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class NominalValueStringConverter : IValueConverter
    {
        public static AttributeType.Numeric numericAttributeType = new AttributeType.Numeric();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Value inValue)
                return numericAttributeType.ValueToString(inValue);
            else
                throw new ArgumentException("not a value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return Value.Parse(stringValue);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    class ValueIsMissingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Value inValue)
                return inValue.IsMissing();
            else
                throw new ArgumentException("not a value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return Visibility.Hidden;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ObjectToTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value.GetType();
            return type.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CopyMutliValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    class PureValueStringConverter : IValueConverter
    {
        public PureValueStringConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Value inValue)
                return inValue.ToString();
            else
                throw new ArgumentException("Not a Value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string inString)
                return Value.Parse(inString);
            else
                throw new ArgumentException("Not a string");
        }
    }
}
