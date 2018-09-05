using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NoMansGUI.Utils.Validation
{
    public class TypeValidationRule : ValidationRule
    {
        public string TypeName
        {
            get;
            set;
        }


        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                Type convertType = Type.GetType(string.Format("System.{0}", TypeName));
                var v = Convert.ChangeType(value, convertType);
                return ValidationResult.ValidResult;
            }
            catch(Exception ex)
            {
                string e = ex.Message;
                return new ValidationResult(false, "Illegal Type");
            }
        }
    }
}
