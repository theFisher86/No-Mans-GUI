using Caliburn.Micro;
using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Utils.AdminTools;
using NoMansGUI.Utils.Parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels
{
    public class MBinViewModel : Screen
    {
        private NMSTemplate _template;
        private ObservableCollection<MBINField> _fields;

        public ObservableCollection<MBINField> Fields
        {
            get { return _fields; }
            set
            {
                _fields = value;
                NotifyOfPropertyChange(() => Fields);
            }
        }

        public MBinViewModel(NMSTemplate template)
        {
            _template = template;
            //This is where the magic happens

            MbinParser parser = new MbinParser();
            List<MBINField> fields = parser.IterateFields(template, template.GetType());
            Fields = new ObservableCollection<MBINField>(fields);
        }

        public void Save()
        {          
            //Loop all the fields.
            foreach(MBINField field in _fields)
            {
                //Get the field name
                string fieldname = field.Name;
                //Find the field in the template.
                FieldInfo info = _template.GetType().GetField(fieldname);
                //Get the correct type
                Type fieldType = info.FieldType;
                //Convert the value to the right type.
                Console.WriteLine(string.Format("Type: {0} - NMSType {1} - TemplateType {2}", fieldType.Name, field.NMSType, field.TemplateType));

                switch (fieldType.Name)
                {
                    case "String":
                        info.SetValue(_template, field.Value);
                        break;
                    case "Single":
                        info.SetValue(_template, float.Parse((string)field.Value));
                        break;
                    case "Boolean":
                        info.SetValue(_template, bool.Parse((string)field.Value));
                        break;
                    case "Int16":
                        info.SetValue(_template, short.Parse((string)field.Value));
                        break;
                    case "UInt16":
                        info.SetValue(_template, ushort.Parse((string)field.Value));
                        break;
                    case "Int32":
                        info.SetValue(_template, int.Parse((string)field.Value));
                        break;    
                    case "UInt32":
                        info.SetValue(_template, uint.Parse((string)field.Value));
                        break;
                    case "Int64":
                        info.SetValue(_template, long.Parse((string)field.Value));
                        break;
                    case "UInt64":
                        info.SetValue(_template, ulong.Parse((string)field.Value));
                        break;
                    case "Byte[]":
                        if (field.Value != null)
                        {
                            info.SetValue(_template, Convert.FromBase64String((string)field.Value));
                        }
                        else
                        {
                            field.Value = null;
                        }
                        break;
                    case "List`1":

                        //Type elementType = fieldType.GetGenericArguments()[0];
                        //Type listType = typeof(List<>).MakeGenericType(elementType);
                        //IList list = (IList)Activator.CreateInstance(listType);
                        //foreach (MBINField innerObject in (List<MBINField>)field.Value) // child templates
                        //{
                        //    //list.Add();
                        //}
                        //info.SetValue( list;
                        break;
                    default:
                        if(field.TemplateType == "nmsstruct")
                        {
                            Console.WriteLine("Found nmsstruct - Value is Type {0}", field.Value.GetType().ToString());
                            if(field.Value.GetType().Name.Contains("list"))
                            {
                                var Struct = info.GetValue(_template);

                                //foreach(var value in field.Value)
                                //{

                                //}
                                //FieldInfo structInfo = Struct.GetType().getva
                            }
                        }
                        break;
                        //if (field.FieldType.IsArray && field.FieldType.GetElementType().BaseType.Name == "NMSTemplate")
                        //{
                        //    Array array = Array.CreateInstance(field.FieldType.GetElementType(), settings.Size);
                        //    //var data = xmlProperty.Elements.OfType<EXmlProperty>().ToList();
                        //    List<EXmlBase> data = xmlProperty.Elements.ToList();
                        //    int numMeta = 0;
                        //    foreach (EXmlBase entry in data)
                        //    {
                        //        if (entry.GetType() == typeof(EXmlMeta))
                        //        {
                        //            numMeta += 1;
                        //        }
                        //    }
                        //    if (data.Count - numMeta != settings.Size)
                        //    {
                        //        // todo: add a comment in the XML to indicate arrays (+ their size), also need to do the same for showing valid enum values
                        //        var error = $"{field.Name}: XML array size {data.Count - numMeta} doesn't match expected array size {settings.Size}";
                        //        DebugLogComment($"Error: {error}!");
                        //        DebugLogComment("You might have added/removed an item from an array field");
                        //        DebugLogComment("(arrays can't be shortened or extended as they're a fixed size set by the game)");
                        //        throw new Exception(error);
                        //    }
                        //    for (int i = 0; i < data.Count; ++i)
                        //    {
                        //        if (data[i].GetType() == typeof(EXmlProperty))
                        //        {
                        //            NMSTemplate element = DeserializeEXml(data[i]);
                        //            array.SetValue(element, i - numMeta);
                        //        }
                        //        else if (data[i].GetType() == typeof(EXmlMeta))
                        //        {
                        //            DebugLogComment(((EXmlMeta)data[i]).Comment);     // don't need to worry about nummeta here since it is already counted above...
                        //        }
                        //    }

                        //    return array;
                        //}
                        //else if (field.FieldType.IsArray)
                        //{
                        //    Array array = Array.CreateInstance(field.FieldType.GetElementType(), settings.Size);
                        //    //List<EXmlProperty> data = xmlProperty.Elements.OfType<EXmlProperty>().ToList();
                        //    List<EXmlBase> data = xmlProperty.Elements.ToList();
                        //    int numMeta = 0;
                        //    for (int i = 0; i < data.Count; ++i)
                        //    {
                        //        if (data[i].GetType() == typeof(EXmlProperty))
                        //        {
                        //            object element = DeserializeEXmlValue(template, field.FieldType.GetElementType(), field, (EXmlProperty)data[i], templateType, settings);
                        //            array.SetValue(element, i - numMeta);
                        //        }
                        //        else if (data[i].GetType() == typeof(EXmlMeta))
                        //        {
                        //            DebugLogComment(((EXmlMeta)data[i]).Comment);
                        //            numMeta += 1;           // increment so that the actual data is still placed at the right spot
                        //        }
                        //    }

                        //    return array;
                        //}
                        //else if (field.FieldType.IsEnum)
                        //{
                        //    return Array.IndexOf(Enum.GetNames(field.FieldType), xmlProperty.Value);
                        //}
                        //else
                        //{
                        //    return fieldType.IsValueType ? Activator.CreateInstance(fieldType) : null;
                        //}
                }
            }

            _template.WriteToExml(@"C:\Users\xfxma\Desktop\Output Test\test.exml");
        }
    }
}
