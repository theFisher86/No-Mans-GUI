using System;
using System.Globalization;
using System.Windows.Data;
using libMBIN.NMS;

namespace NoMansGUI.Utils.Converters
{
    public class NMSString0x10Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as NMSString0x10).Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
