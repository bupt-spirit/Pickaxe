using PickaxeCore.Relation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Pickaxe.Utility
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
}
