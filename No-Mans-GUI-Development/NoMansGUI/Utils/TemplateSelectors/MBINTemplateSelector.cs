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
using System.Drawing;

namespace NoMansGUI.Utils.TemplateSelectors
{
    /// <summary>
    /// Not sure if you can/will make use of this yourself, it simply tells the view which template to use based on the type, this is where all the 
    /// display stuff is done.
    /// </summary>
    public class MBINTemplateSelector : DataTemplateSelector
    {
        private static readonly log4net.ILog m_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            if (item is MBINField field)
            {
                //Get just the last part of the string
                string switchCase = field.TemplateType.Split('.').Last();
                Console.WriteLine("name: " + field.Name.ToString() + " value: " + field.Value.ToString() + " NMStype: " + field.NMSType.ToString() + " TemplateType: " + field.TemplateType.ToString());
                switch (switchCase.ToLower())
                {
                    case "list":
                    case "array":
                        if(field.NMSType == "Colour")
                        {
                            return element.FindResource("ColourPickerDataTemplate") as DataTemplate;
                        }
                        return element.FindResource("ListDataTemplate") as DataTemplate;
                    case "string":
                        return element.FindResource("StringDataTemplate") as DataTemplate;
                    case "nmsstring0x10":
                        return element.FindResource("NMSString0x10DataTemplate") as DataTemplate;
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
                        m_log.Error("No Template found for item of type " + switchCase);
                        TemplateLogHelper.AddMissingTemplate(switchCase);
                        return element.FindResource("StringDataTemplate") as DataTemplate;
                }
            }

            return element.FindResource("StringDataTemplate") as DataTemplate;
        }

    }
}
