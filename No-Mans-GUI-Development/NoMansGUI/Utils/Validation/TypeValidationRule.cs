using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NoMansGUI.Utils.Validation
{
    public class TypeValidationRule : ValidationRule
    {
        public Type Type
        {
            get;
            set;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!(value is Type))
            {
                return new ValidationResult(false, "Illegal Type");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
