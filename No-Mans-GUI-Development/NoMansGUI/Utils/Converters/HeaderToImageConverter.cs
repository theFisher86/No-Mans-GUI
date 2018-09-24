using NoMansGUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace NoMansGUI.Utils.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DirectoryItem)
            {
                Uri uri = new Uri("pack://application:,,,/Resources/Images/folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/Resources/Images/nms.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
