using NoMansGUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NoMansGUI.Utils.TemplateBuilder;
using libMBIN.Models.Structs;

namespace NoMansGUI.Utils.TemplateSelectors
{
    /// <summary>
    /// Not sure if you can/will make use of this yourself, it simply tells the view which template to use based on the type, this is where all the 
    /// display stuff is done.
    /// </summary>
    public class MBINTemplateSelector : DataTemplateSelector
    {
        private static readonly log4net.ILog m_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //TODO: This shouldn't a string, find some way to compare actual type.
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item is MBINField field)
            {
                //Get just the last part of the string
                string switchCase = field.TemplateType.Split('.').Last();
                switch (switchCase.ToLower())
                {
                    case "list":
                    case "array":
                    case "nmsstruct":
                        Console.WriteLine("Found TemplateType nmsstruct:");
                        if (field.NMSType == typeof(Colour))
                        {
                            return element.FindResource("ColourPickerDataTemplate") as DataTemplate;
                        }
                        else if(field.NMSType == typeof(Vector2f) || field.NMSType == typeof(Vector4f))
                        {
                            return element.FindResource("VectorDataTemplate") as DataTemplate;
                        }
                        Console.WriteLine("nmsstruct didn't match any special rules, using default ListDataTemplate.");
                        Console.WriteLine(string.Format("nmsstruct data is : NMSType {0} - Value {1}", field.NMSType, field.Value));

                        return element.FindResource("ListDataTemplate") as DataTemplate;
                    case "string":
                        return element.FindResource("StringDataTemplate") as DataTemplate;
                    case "nmsstring0x10":
                        return element.FindResource("NMSString0x10DataTemplate") as DataTemplate;
                    case "vector2f":
                    case "vector4f":
                    case "vector6f":
                        return element.FindResource("VectorDataTemplate") as DataTemplate;
                    case "single":
                        Console.WriteLine("Found TemplateType Single");
                        Console.WriteLine(string.Format("Data is : NMSType {0} - Value {1} - Value Type", field.NMSType, field.Value, field.Value.GetType().Name));
                        return element.FindResource("SingleDataTemplate") as DataTemplate;
                    case "int":
                    case "int16":
                    case "int32":
                    case "int64":
                    case "uint":
                    case "uint16":
                    case "uint32":
                    case "uint64":
                        return element.FindResource("IntDataTemplate") as DataTemplate;
                    case "boolean":
                        return element.FindResource("BoolDataTemplate") as DataTemplate;
                    case "enum":
                        return element.FindResource("EnumDataTemplate") as DataTemplate;
                    default:
                        //We don't yet have a match for these, for now we return the standard string template and log it as missing.
                        Console.WriteLine("No Template Found for " + switchCase);
                        m_log.Error("No Template found for item of type " + switchCase);
                        TemplateLogHelper.AddMissingTemplate(switchCase);
                        return element.FindResource("StringDataTemplate") as DataTemplate;
                }
            }

            return element.FindResource("StringDataTemplate") as DataTemplate;
        }

    }
}
