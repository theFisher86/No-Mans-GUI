using libMBIN.Models.Structs;
using NoMansGUI.Models;
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
            int a = 1, r = 1, g = 1, b = 1;
            foreach(var f in value as List<MBINField>)
            {
                switch(f.Name)
                {
                    case "A":
                        a = (int)(float.Parse(f.Value) * 255);
                        break;
                    case "R":
                        r = (int)(float.Parse(f.Value) * 255);
                        break;
                    case "G":
                        g = (int)(float.Parse(f.Value) * 255);
                        break;
                    case "B":
                        b = (int)(float.Parse(f.Value) * 255);
                        break;
                }
            }
            Color newc = Color.FromArgb(a, r, g, b);
            return new SolidBrush(newc);        
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color c = (Color)value;

            Colour newc = new Colour
            {
                A = c.A / 255,
                R = c.R / 255,
                G = c.G / 255,
                B = c.B / 255
            };

            return newc;

        }
    }
}
