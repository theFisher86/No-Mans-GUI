using libMBIN.Models.Structs;
using NoMansGUI.Models;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Windows.Media;
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
            Console.WriteLine("color found!" + value.ToString());
            int a = 1, r = 1, g = 1, b = 1;
            foreach(var f in value as List<MBINField>)
            {
                Console.WriteLine(f.Name + ": " + f.Value + "NMSType " + f.NMSType + "TemplateType " + f.TemplateType);
                switch(f.Name)
                {
                    case "A":
                        a = (int)(float.Parse(f.Value.ToString()) * 255);
                        break;
                    case "R":
                        r = (int)(float.Parse(f.Value.ToString()) * 255);
                        break;
                    case "G":
                        g = (int)(float.Parse(f.Value.ToString()) * 255);
                        break;
                    case "B":
                        b = (int)(float.Parse(f.Value.ToString()) * 255);
                        break;
                }
            }
            Console.WriteLine("color Values :" + a.ToString() + "," + r.ToString() + "," + g.ToString() + "," + b.ToString());
            
            Color newc = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            Console.WriteLine("Completed Color: " + newc.ToString());
            //return new SolidBrush(newc);        
            return newc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color nmsColor = (Color)value;

            List<MBINField> colorField = new List<MBINField>();
            colorField.Add(new MBINField { Name = "R", Value = nmsColor.R / 255, NMSType = "System.Single", TemplateType = "System.Single" });
            colorField.Add(new MBINField { Name = "B", Value = nmsColor.B / 255, NMSType = "CoSystem.Singlelour", TemplateType = "System.Single" });
            colorField.Add(new MBINField { Name = "A", Value = nmsColor.A / 255, NMSType = "System.Single", TemplateType = "System.Single" });
            colorField.Add(new MBINField { Name = "G", Value = nmsColor.G / 255, NMSType = "System.Single", TemplateType = "System.Single" });

            return colorField;
        }
    }
}
