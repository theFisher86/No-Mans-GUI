using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Utils.Debug;
using System;
using System.Collections.Generic;
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
            if (fields != null)
            {
                //We then loop over all those fields.
                foreach (FieldInfo fieldInfo in fields)
                {
                    //Debug.WriteLine($"type = {fieldInfo.FieldType}, name = {fieldInfo.Name}, value = {fieldInfo.GetValue(data)}");      //write all fields to debug
                    //Check for NMSAttribute ignore -code by @GaticusHax
                    var attributes = (NMSAttribute[])fieldInfo.GetCustomAttributes(typeof(NMSAttribute), false);                        //
                    libMBIN.Models.NMSAttribute attrib = null;                                                                          //
                    if (attributes.Length > 0) attrib = attributes[0];                                                                  //
                    bool ignore = false;                                                                                                //
                    if (attrib != null) ignore = attrib.Ignore;                                                                         //
                    if (!ignore)                                                                                                        // Add the field to the mbinContents list
                    {
                        //This is where things get fun, we need to check if this is a generic type. AFAIK only the collections are.
                        //so we should be ok with this.
                        var isGeneric = fieldInfo.FieldType.IsGenericType;
                        var isArray = fieldInfo.FieldType.IsArray;
                        if (isGeneric)
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
                                NMSType = fieldInfo.FieldType.Name,
                                TemplateType = "list"
                            };
                            //We build a list of MBINFields, which will hold the elements of this list.
                            List<MBINField> v = new List<MBINField>();

                            //The type of objects in the array
                            Type aType;
                            GetListElementType((dynamic)fieldInfo.GetValue(data), out aType);

                            //Loop all the elements in the original list.
                            foreach (dynamic listentry in list)
                            {
                                //We create a new MBINField for each element, as these elements are often Classes we need to recusivly call
                                //IterateFields so we build up a list of the class fields again.                              
                                
                                dynamic innerValue = null;
                                string t = "";

                                if(aType.Name == "NMSTemplate")
                                {
                                    aType = listentry.GetType();
                                }

                                if (aType.IsClass == true)
                                {
                                    innerValue = IterateFields(listentry, listentry.GetType());
                                    t = "list";
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
                                    NMSType = listentry.GetType().ToString(),
                                    TemplateType = t
                                };
                                //Obviously we need to add it to the list we created
                                v.Add(f);
                            }
                            //And then we set the value of the orignal MBINField to the list.
                            mBINField.Value = v;

                            //And finally we all the parent.
                            mbinContents.Add(mBINField);
                        }
                        else if (isArray)
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
                                NMSType = fieldInfo.FieldType.Name,
                                TemplateType = "array"
                            };
                            //We build a list of MBINFields, which will hold the elements of this list.
                            List<MBINField> v = new List<MBINField>();

                            //The type of objects in the array
                            Type aType;
                            GetArrayElementType(fieldInfo, out aType);

                            //Loop all the elements in the original list.
                            int i = 0;
                            foreach (dynamic arrayentry in array)
                            {
                                string name = "";
                                if (attrib?.EnumValue != null)
                                {
                                    name = attrib.EnumValue[i];
                                    i++;
                                }

                                dynamic innerValue = null;
                                string t = "";
                                if (aType.IsClass == true)
                                {
                                    innerValue = IterateFields(arrayentry, aType);
                                    t = "list";
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
                                    NMSType = string.IsNullOrEmpty(name) ? aType.ToString() : name,
                                    TemplateType = t
                                };
                                //Obviously we need to add it to the list we created
                                v.Add(f);
                            }
                            //And then we set the value of the orignal MBINField to the list.
                            mBINField.Value = v;

                            //And finally we all the parent.
                            mbinContents.Add(mBINField);
                        }
                        else if (fieldInfo.FieldType.BaseType == typeof(NMSTemplate))
                        {
                            mbinContents.Add(new MBINField
                            {
                                Name = fieldInfo.Name,
                                Value = IterateFields((NMSTemplate)fieldInfo.GetValue(data), fieldInfo.GetValue(data).GetType()),
                                NMSType = fieldInfo.FieldType.Name,
                                TemplateType = "list"
                            });
                        }
                        //We sometimes get a field of type NMSTemplate that can be any kind of struct, so we need to handle it right.
                        else if (fieldInfo.FieldType == typeof(NMSTemplate))
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
                                    NMSType = templateType.ToString(),
                                    TemplateType = "list"
                                };
                                mbinContents.Add(field);

                                //Debug stuff for GaticusHax
                                NMSTemplateTypeList.AddToDebugList(fieldInfo.Name, field);
                            }
                        }
                        else if(fieldInfo.FieldType.BaseType == typeof(Enum))
                        {
                           
                            mbinContents.Add(new MBINField
                            {
                                Name = fieldInfo.Name,
                                EnumValues = fieldInfo.FieldType.GetEnumValues(),
                                Value = fieldInfo.GetValue(data),
                                NMSType = fieldInfo.FieldType.ToString(),
                                TemplateType = "enum"
                            });
                        }
                        else
                        {
                            //We have a nice simple normal object, no fuss
                            mbinContents.Add(new MBINField
                            {
                                Name = fieldInfo.Name,
                                //Notice the ? after GetValue(Data) this ensure we actually have a return before tying to ToString() as some elements are null.
                                Value = fieldInfo.GetValue(data)?.ToString(),
                                NMSType = fieldInfo.FieldType.ToString(),
                                TemplateType = fieldInfo.FieldType.ToString()
                            });
                        }//
                    }
                }
            }
            else
            {
                Console.WriteLine("Error Getting Fields...", "Couldn't get the fields for some reason.\n Data: " + data.ToString() + "\n Will return blank List");
                mbinContents = null;
            }
            return mbinContents;
        }



        private bool GetArrayElementType(FieldInfo field, out Type elementType)
        {
            if (field.FieldType.IsArray && field.FieldType.FullName.EndsWith("[]"))
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
            if (list != null)
            {
                elementType = typeof(T);
                return true;
            }

            elementType = null;
            return false;
        }
    }
}
