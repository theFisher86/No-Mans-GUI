using libMBIN.NMS;
using NoMansGUI.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                    case "nmsstring0x10":
                        Console.WriteLine("Found TemplateType nmsstruct:");
                        if (field.NMSType == typeof(Colour))
                        {
                            return Application.Current.FindResource("ColourPickerDataTemplate") as DataTemplate;
                        }
                        else if(field.NMSType == typeof(Vector2f) || field.NMSType == typeof(Vector4f))
                        {
                            return Application.Current.FindResource("VectorDataTemplate") as DataTemplate;
                        }
                        Console.WriteLine("nmsstruct didn't match any special rules, using default ListDataTemplate.");
                        Console.WriteLine(string.Format("nmsstruct data is : NMSType {0} - Value {1}", field.NMSType, field.Value));

                        return Application.Current.FindResource("ListDataTemplate") as DataTemplate;
                    case "string":
                        return Application.Current.FindResource("StringDataTemplate") as DataTemplate;
                    case "vector2f":
                    case "vector4f":
                    case "vector6f":
                        return Application.Current.FindResource("VectorDataTemplate") as DataTemplate;
                    case "single":
                        Console.WriteLine("Found TemplateType Single");
                        Console.WriteLine(string.Format("Data is : NMSType {0} - Value {1} - Value Type", field.NMSType, field.Value, field.Value.GetType().Name));
                        return Application.Current.FindResource("SingleDataTemplate") as DataTemplate;
                    case "int":
                    case "int16":
                    case "int32":
                    case "int64":
                    case "uint":
                    case "uint16":
                    case "uint32":
                    case "uint64":
                        return Application.Current.FindResource("IntDataTemplate") as DataTemplate;
                    case "boolean":
                        return Application.Current.FindResource("BoolDataTemplate") as DataTemplate;
                    case "enum":
                        return Application.Current.FindResource("EnumDataTemplate") as DataTemplate;
                    default:
                        //We don't yet have a match for these, for now we return the standard string template and log it as missing.
                        Console.WriteLine("No Template Found for " + switchCase);
                        m_log.Error("No Template found for item of type " + switchCase);
                        TemplateLogHelper.AddMissingTemplate(switchCase);
                        return Application.Current.FindResource("StringDataTemplate") as DataTemplate;
                }
            }

            return Application.Current.FindResource("StringDataTemplate") as DataTemplate;
        }

    }
}
