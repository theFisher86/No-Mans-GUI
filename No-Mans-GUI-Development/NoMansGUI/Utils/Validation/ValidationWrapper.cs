using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NoMansGUI.Utils.Validation
{
    public class ValidationWrapper : DependencyObject
    {
        public static readonly DependencyProperty ExpectedTypeProperty =
            DependencyProperty.Register("ExpectedType", typeof(Type), typeof(ValidationWrapper));

        public Type ExpectedType
        {
            get
            {
                return (Type)GetValue(ExpectedTypeProperty);
            }
            set
            {
                SetValue(ExpectedTypeProperty, value);
            }
        }
    }
}
