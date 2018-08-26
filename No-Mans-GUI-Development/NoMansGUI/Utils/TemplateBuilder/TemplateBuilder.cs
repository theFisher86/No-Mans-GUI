using libMBIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NoMansGUI.Utils.TemplateBuilder
{
    public static class TemplateBuilder
    {

        public static DataTemplate GenerateTemplate(NMSTemplate data)
        {
            DataTemplate dt = new DataTemplate
            {
                DataType = data.GetType()
            };

            FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
            spFactory.Name = "Test";
            spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            spFactory.SetValue(StackPanel.DataContextProperty, data);


            //It starts pretty much the same as usual, we get a collection of all the fields in the NMSTemplate.
            IOrderedEnumerable<FieldInfo> fields = data.GetType().GetFields().OrderBy(field => field.MetadataToken);
            if (fields != null)
            {
                //We then loop over all those fields.
                foreach (FieldInfo fieldInfo in fields)
                {
                    var attributes = (NMSAttribute[])fieldInfo.GetCustomAttributes(typeof(NMSAttribute), false);                        //
                    libMBIN.Models.NMSAttribute attrib = null;                                                                          //
                    if (attributes.Length > 0) attrib = attributes[0];                                                                  //
                    bool ignore = false;                                                                                                //
                    if (attrib != null) ignore = attrib.Ignore;                                                                         //

                    if (!ignore)                                                                                                        // Add the field to the mbinContents list
                    {
                        FrameworkElementFactory Label = new FrameworkElementFactory(typeof(TextBlock));
                        Label.SetValue(TextBlock.TextProperty, fieldInfo.Name);
                        Label.SetValue(TextBlock.ToolTipProperty, fieldInfo.Name);
                        spFactory.AppendChild(Label);

                        FrameworkElementFactory cardHolder = new FrameworkElementFactory(typeof(TextBlock));
                        cardHolder.SetBinding(TextBlock.TextProperty, new Binding(fieldInfo.Name));
                        cardHolder.SetValue(TextBlock.ToolTipProperty, fieldInfo.Name);
                        spFactory.AppendChild(cardHolder);
                    }
                }
                dt.VisualTree = spFactory;
            }
            return dt;
        }
    }
}
