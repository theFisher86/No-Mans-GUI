using libMBIN.Models.Structs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NoMansGUI.Utils.Converters
{
    public class NMSColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Colour c = value as Colour;
            Color newc = Color.FromArgb((int)c.A, (int)c.R, (int)c.G, (int)c.B);
            return newc;         
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c = (Color)value;

            Colour newc = new Colour
            {
                A = c.A,
                R = c.R,
                G = c.G,
                B = c.B
            };

            return newc;

        }
    }
}
