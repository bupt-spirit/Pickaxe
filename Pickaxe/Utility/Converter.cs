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
}
