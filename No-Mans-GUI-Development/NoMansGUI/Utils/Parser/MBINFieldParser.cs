using libMBIN.Models;
using NoMansGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils.Parser
{
    public class MBINFieldParser
    {

        public NMSTemplate ParseTemplateFromMBINFields(NMSTemplate template, List<MBINField> fields)
        {
            //Loop all the fields.
            foreach (MBINField field in fields)
            {
                //Get the field name
                string fieldname = field.Name;
                //Find the field in the template.
                FieldInfo info = template.GetType().GetField(fieldname);
                //Get the correct type
                Type fieldType = info.FieldType;
                //Convert the value to the right type.
                Console.WriteLine(string.Format("Type: {0} - NMSType {1} - TemplateType {2}", fieldType.Name, field.NMSType, field.TemplateType));

                switch (fieldType.Name)
                {
                    case "String":
                        info.SetValue(template, field.Value);
                        break;
                    case "Single":
                        info.SetValue(template, float.Parse((string)field.Value));
                        break;
                    case "Boolean":
                        info.SetValue(template, bool.Parse((string)field.Value));
                        break;
                    case "Int16":
                        info.SetValue(template, short.Parse((string)field.Value));
                        break;
                    case "UInt16":
                        info.SetValue(template, ushort.Parse((string)field.Value));
                        break;
                    case "Int32":
                        info.SetValue(template, int.Parse((string)field.Value));
                        break;
                    case "UInt32":
                        info.SetValue(template, uint.Parse((string)field.Value));
                        break;
                    case "Int64":
                        info.SetValue(template, long.Parse((string)field.Value));
                        break;
                    case "UInt64":
                        info.SetValue(template, ulong.Parse((string)field.Value));
                        break;
                    case "Byte[]":
                        if (field.Value != null)
                        {
                            info.SetValue(template, Convert.FromBase64String((string)field.Value));
                        }
                        else
                        {
                            field.Value = null;
                        }
                        break;
                    case "List`1":
                        Console.WriteLine("Found List`1 - Value is Type {0}", field.Value.GetType().ToString());
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
                        if (field.NMSType.BaseType == typeof(NMSTemplate))
                        {
                            Console.WriteLine("Found nmsstruct - Value is Type {0} - FieldType.Name is {1}", field.Value.GetType().ToString(), fieldType.Name);
                           
                                //This will be a struct, i guess
                                var Struct = info.GetValue(template);

                                var stuctTemplate = this.ParseTemplateFromMBINFields((NMSTemplate)Struct, (List<MBINField>)field.Value);

                                Type t = field.NMSType;

                                foreach (MBINField value in (List<MBINField>)field.Value)
                                {
                                    info.SetValue(template, Convert.ChangeType(stuctTemplate, t));
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

            return template;
        }
    }
}
