using NoMansGUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
//using System.Drawing;
using System.Windows.Media;

namespace NoMansGUI.Utils.Converters
{
    public class NMSColourConverter : DependencyObject, IValueConverter
    {

        public static DependencyProperty SourceValueProperty =
         DependencyProperty.Register("SourceValue",
                                     typeof(object),
                                     typeof(NMSColourConverter));
        public object SourceValue
        {
            get { return (object)GetValue(SourceValueProperty); }
            set { SetValue(SourceValueProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("color found!" + value.ToString());
            int a = 1, r = 1, g = 1, b = 1;
            Type t = value.GetType();
            foreach(var f in value as ObservableCollection<MBINField>)
            {
                int num = -1;                                                   // This shit checks to make sure that the value is a valid number
                if (!int.TryParse(f.Value.ToString(), out num))                    // and not null.
                {
                    Console.WriteLine("value not an int, setting 0");           // if it is null it'll be set to 0
                    f.Value = (int)0;
                }
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
            List<MBINField> original = (List<MBINField>)SourceValue;
            // TODO
            // This is breaking shit
            // System.InvalidCastException: 'Unable to cast object of type 'System.Collections.ObjectModel.ObservableCollection`1[NoMansGUI.Models.MBINField]' 
            // to type 'System.Collections.Generic.List`1[NoMansGUI.Models.MBINField]'.'
            // Until it's fixed the color picker drop down is disabled inside MbinTemplates.xaml
            foreach (MBINField field in original)
            {
                switch(field.Name)
                {
                    case "A":
                        field.Value = (float)nmsColor.A / 255;
                        break;
                    case "R":
                        field.Value = (float)nmsColor.R / 255;
                        break;
                    case "G":
                        field.Value = (float)nmsColor.G / 255;
                        break;
                    case "B":
                        field.Value = (float)nmsColor.B / 255;
                        break;
                }
                float num = -1;                                                     // Same thing as above, just checking to make sure it's a number
                if(!float.TryParse(field.Value.ToString(),out num))                    // and setting to 0 if not.
                {
                    Console.WriteLine("field value not a float");
                    field.Value = (float)0;
                }
            }
            return original;
        }
    }
}
