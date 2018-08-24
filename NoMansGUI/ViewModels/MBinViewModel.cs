using Caliburn.Micro;
using libMBIN.Models;
using NoMansGUI.Models;
using System;
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
            Fields = new ObservableCollection<MBINField>(IterateFields(_template, _template.GetType()));
        }


        /// <summary>
        /// Loops all the fields on the NMSTemplate, and then recursevly loops and lists it find's to ensure all objects
        /// are converted into MBINField
        /// </summary>
        /// <param name="data">the NMSTemplate</param>
        /// <param name="type">the MNSTemplate type</param>
        /// <returns></returns>
        public static List<MBINField> IterateFields(NMSTemplate data, Type type)
        {
            List<MBINField> mbinContents = new List<MBINField>();

            //It starts pretty much the same as usual, we get a collection of all the fields in the NMSTemplate.
            IOrderedEnumerable<FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);
            if (fields != null)
            {
                //We then loop over all those fields.
                foreach (FieldInfo fieldInfo in fields)
                {
                    Debug.WriteLine($"type = {fieldInfo.FieldType}, name = {fieldInfo.Name}, value = {fieldInfo.GetValue(data)}");      //write all fields to debug
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
                                NMSType = "list"
                            };
                            //We build a list of MBINFields, which will hold the elements of this list.
                            List<MBINField> v = new List<MBINField>();

                            //Loop all the elements in the original list.
                            foreach (dynamic listentry in list)
                            {
                                //We create a new MBINField for each element, as these elements are often Classes we need to recusivly call
                                //IterateFields so we build up a list of the class fields again.
                                //There is one big issue with this still, what if this is just a list of strings or something similar, we need a way
                                //to tell the difference i've not got to that yet.
                                MBINField f = new MBINField()
                                {
                                    Name = listentry.GetType().ToString(),
                                    Value = IterateFields(listentry as NMSTemplate, listentry.GetType()),
                                    NMSType = "list" //Once again we use list to avoid the stupidly long string.
                                };
                                //Obviously we need to add it to the list we created
                                v.Add(f);                               
                            }
                            //And then we set the value of the orignal MBINField to the list.
                            mBINField.Value = v;

                            //And finally we all the parent.
                            mbinContents.Add(mBINField);
                        }
                        else
                        {
                            //We have a nice simple normal object, no fuss
                            mbinContents.Add(new MBINField                                                                                  //
                            {                                                                                                               //
                                Name = fieldInfo.Name,                                                                                      //
                                Value = fieldInfo.GetValue(data).ToString(),                                                                //
                                NMSType = fieldInfo.FieldType.ToString()                                                                    //
                            });
                        }//
                    }                                                                                                                   //
                }
            }
            else
            {
                Debug.WriteLine("Error Getting Fields...", "Couldn't get the fields for some reason.\n Data: " + data.ToString() + "\n Will return blank List");
                mbinContents = null;
            }
            return mbinContents;
        }
    }
}
