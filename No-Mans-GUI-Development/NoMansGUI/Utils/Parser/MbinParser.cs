using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Utils.AdminTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils.Parser
{
    public class MbinParser
    {
        /// <summary>
        /// Loops all the fields on the NMSTemplate, and then recursevly loops and lists it find's to ensure all objects
        /// are converted into MBINField
        /// </summary>
        /// <param name="data">the NMSTemplate</param>
        /// <param name="type">the MNSTemplate type</param>
        /// <returns></returns>
        public List<MBINField> IterateFields(NMSTemplate data, Type type)
        {
            List<MBINField> mbinContents = new List<MBINField>();

            //It starts pretty much the same as usual, we get a collection of all the fields in the NMSTemplate.
            IOrderedEnumerable<FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);
            if (fields == null)
            {
                Console.WriteLine("Error Getting Fields...", "Couldn't get the fields for some reason.\n Data: " + data.ToString() + "\n Will return blank List");
                mbinContents = null;
            } else {
                //We then loop over all those fields.
                foreach (FieldInfo fieldInfo in fields)
                {
                    //Check for NMSAttribute ignore -code by @GaticusHax
                    var attributes = (NMSAttribute[])fieldInfo.GetCustomAttributes(typeof(NMSAttribute), false);                        
                    var attrib = (attributes.Length > 0) ? attributes[0] : new NMSAttribute();
                    if ((attrib?.Ignore ?? false) == false)                                                                         
                    {
                        //This is where things get fun, we need to check if this is a generic type. AFAIK only the collections are.
                        //so we should be ok with this.
                        var isGeneric = fieldInfo.FieldType.IsGenericType;
                        var isArray = fieldInfo.FieldType.IsArray;

                        if (isGeneric)      { mbinContents.Add(ParseList(fieldInfo, data)); }
                        else if (isArray)   { mbinContents.Add(ParseArray(fieldInfo, data, attrib)); }
                        else if (fieldInfo.FieldType.BaseType == typeof(NMSTemplate))
                        {
                            mbinContents.Add(new MBINField
                            {
                                Name = fieldInfo.Name,
                                Value = IterateFields((NMSTemplate)fieldInfo.GetValue(data), fieldInfo.GetValue(data).GetType()),
                                NMSType = fieldInfo.FieldType,
                                TemplateType = "nmsstruct"
                            });
                        }
                        //We sometimes get a field of type NMSTemplate that can be any kind of struct, so we need to handle it right.
                        else if (fieldInfo.FieldType == typeof(NMSTemplate))
                        {
                            mbinContents.Add(ParseStruct(fieldInfo, data));
                        }
                        else if(fieldInfo.FieldType.BaseType == typeof(Enum))
                        {
                            mbinContents.Add(new MBINField
                            {
                                Name = fieldInfo.Name,
                                EnumValues = fieldInfo.FieldType.GetEnumValues(),
                                Value = fieldInfo.GetValue(data),
                                NMSType = fieldInfo.FieldType,
                                TemplateType = "enum"
                            });
                        } else {
                            //We have a nice simple normal object, no fuss
                            mbinContents.Add(new MBINField
                            {
                                Name = fieldInfo.Name,
                                //Notice the ? after GetValue(Data) this ensure we actually have a return before tying to ToString() as some elements are null.
                                Value = fieldInfo.GetValue(data)?.ToString(),
                                NMSType = fieldInfo.FieldType,
                                TemplateType = fieldInfo.FieldType.ToString()
                            });
                        }
                    }
                }
            }
            return mbinContents;
        }

        private MBINField ParseList(FieldInfo fieldInfo, NMSTemplate data)
        {
            //We've found a generic field, we assume it's a collection and get to work.
            //We grab the value from the field
            var value = fieldInfo.GetValue(data);
            //We then create a dynamic variable to hold our list, Dynamic allow us to avoid typing and let .net work out
            //what the object is, as at compile time we have no realistic way of know what the type actually is.
            dynamic list = value;

            // build the basic MBINField to hold the list, we use NMSType because otherwise NMSType becomes something like
            // System.Collections.Generic.List`1[[libMBIN.Models.Structs.GcDiscoveryRewardLookup, libMBIN, Version=1.57.0.0, Culture=neutral, PublicKeyToken=null]]
            // which is just a little annoying to work with.
            MBINField mBINField = new MBINField()
            {
                Name = fieldInfo.Name,
                NMSType = fieldInfo.FieldType,
                TemplateType = "nmsstruct"
            };
            //We build a list of MBINFields, which will hold the elements of this list.
            List<MBINField> v = new List<MBINField>();

            //The type of objects in the array
            Type aType;
            GetListElementType((dynamic)fieldInfo.GetValue(data), out aType);

            //Loop all the elements in the original list.
            foreach (object listentry in list)
            {
                //We create a new MBINField for each element, as these elements are often Classes we need to recusivly call
                //IterateFields so we build up a list of the class fields again.                              

                object innerValue = null;
                string t = "";

                if (aType == typeof(NMSTemplate))
                {
                    aType = listentry.GetType();
                }

                if (aType.IsClass == true)
                {
                    innerValue = IterateFields((NMSTemplate)listentry, listentry.GetType());
                    t = "nmsstruct";
                }
                else
                {
                    innerValue = Convert.ChangeType(listentry, listentry.GetType());
                    t = listentry.GetType().ToString();
                }


                MBINField f = new MBINField()
                {
                    Name = fieldInfo.Name, //aType.ToString(),
                    Value = innerValue,
                    NMSType = listentry.GetType(),
                    TemplateType = t
                };
                //Obviously we need to add it to the list we created
                v.Add(f);
            }
            //And then we set the value of the orignal MBINField to the list.
            mBINField.Value = v;

            //And finally return the parent.
            return mBINField;
        }

        private MBINField ParseArray(FieldInfo fieldInfo, NMSTemplate data, NMSAttribute attrib)
        {
            //We've found a generic field, we assume it's a collection and get to work.
            //We grab the value from the field
            var value = fieldInfo.GetValue(data);
            //We then create a dynamic variable to hold our list, Dynamic allow us to avoid typing and let .net work out
            //what the object is, as at compile time we have no realistic way of know what the type actually is.
            dynamic array = value;

            // build the basic MBINField to hold the list, we use NMSType because otherwise NMSType becomes something like
            // System.Collections.Generic.List`1[[libMBIN.Models.Structs.GcDiscoveryRewardLookup, libMBIN, Version=1.57.0.0, Culture=neutral, PublicKeyToken=null]]
            // which is just a little annoying to work with.
            MBINField mBINField = new MBINField()
            {
                Name = fieldInfo.Name,
                NMSType = fieldInfo.FieldType,
                TemplateType = "array"
            };
            //We build a list of MBINFields, which will hold the elements of this list.
            List<MBINField> v = new List<MBINField>();

            //The type of objects in the array
            Type aType;
            GetArrayElementType(fieldInfo, out aType);

            //Loop all the elements in the original list.
            int i = 0;
            foreach (object arrayentry in array)
            {
                string name = "";
                if (attrib?.EnumValue != null)
                {
                    name = attrib.EnumValue[i];
                    i++;
                }

                object innerValue = null;
                string t = "";
                if (aType.IsClass == true)
                {
                    innerValue = IterateFields((NMSTemplate)arrayentry, arrayentry.GetType());
                    t = "nmsstruct";
                }
                else
                {
                    innerValue = Convert.ChangeType(arrayentry, aType);
                    t = arrayentry.GetType().ToString();
                }


                MBINField f = new MBINField()
                {
                    Name = string.IsNullOrEmpty(name) ? aType.ToString() : name,
                    Value = innerValue,
                    NMSType = aType,
                    TemplateType = t
                };
                //Obviously we need to add it to the list we created
                v.Add(f);
            }
            //And then we set the value of the orignal MBINField to the list.
            mBINField.Value = v;

            //And finally we all the parent.
            return mBINField;
        }

        private MBINField ParseStruct(FieldInfo fieldInfo, NMSTemplate data)
        {
            NMSTemplate template = (NMSTemplate)fieldInfo.GetValue(data);
            if (template != null)
            {
                Type templateType = template.GetType();
                List<MBINField> value = IterateFields(template, templateType);

                MBINField field = new MBINField
                {
                    Name = fieldInfo.Name,
                    Value = value,
                    NMSType = templateType,
                    TemplateType = "nmsstruct"
                };

                //Debug stuff for GaticusHax
                NMSTemplateTypeList.AddToDebugList(fieldInfo.Name, field);
                return field;
            }
            return null;
        }

        private bool GetArrayElementType(FieldInfo field, out Type elementType)
        {
            if (field.FieldType.IsArray)
            {
                string fullName = field.FieldType.FullName.Substring(0, field.FieldType.FullName.Length - 2);
                elementType = Type.GetType(string.Format("{0},{1}", fullName, field.FieldType.Assembly.GetName().Name));
                return true;
            }
            elementType = null;
            return false;
        }

        private bool GetListElementType<T>(IList<T> list, out Type elementType)
        {
            elementType = (list != null) ? typeof(T) : null;
            return (elementType != null);
        }
    }
}
