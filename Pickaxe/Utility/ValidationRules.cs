using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pickaxe.Utility.ValidationRules
{
    class StringNotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var s = (string)value;
            if (s == string.Empty)
            {
                return new ValidationResult(false, "content is empty");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
