using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using libMBIN;

namespace NoMansGUI
{
    class Logic
    {

        public delegate void TypeHandlerCallback(FieldInfo fieldInfo, StackPanel ControlEditor);
        public Dictionary<Type, TypeHandlerCallback> TypeHandlerTable { get; set; }
        public libMBIN.Models.NMSTemplate mbinData = null;                                      // set mbinData as public and null
        public StackPanel ControlEditor = NoMansGUI.MainWindow.AppWindow.ControlEditor;         // Set ControlEditor as default Stack Panel (can be changed when calling TypeHandler

        public void CreateTypeHandlerTable()                                         // I have no idea what's going on here
        {
            //StackPanel ControlEditor = NoMansGUI.MainWindow.AppWindow.ControlEditor;
            TypeHandlerTable = new Dictionary<Type, TypeHandlerCallback>() {
                { typeof( bool ), HandleBool  },
                { typeof( Int16 ), HandleInt },
                { typeof( Int32 ), HandleInt },
                { typeof( Int64 ), HandleInt },
                { typeof( UInt16 ), HandleInt },
                { typeof( UInt32 ), HandleInt },
                { typeof( UInt64 ), HandleInt },
                { typeof( Byte ), HandleByte },
                { typeof( String), HandleString },
                { typeof( libMBIN.Models.Structs.Vector4f), HandleString },
                { typeof( libMBIN.Models.Structs.Vector2f), HandleString },
                { typeof( libMBIN.Models.Structs.Colour), HandleString },
                { typeof( libMBIN.Models.Structs.GcSeed), HandleString },
                { typeof( libMBIN.Models.Structs.VariableSizeString), HandleString },
                { typeof( System.Collections.Generic.List<System.Single>), HandleString },
                { typeof( Single), HandleString },
                { typeof( Double), HandleString }
            };

        }

        public TypeHandlerCallback GetTypeHandler(Type type)
        {
            return TypeHandlerTable[type];
        }

        public Logic()                                                         // this is the constructor
        {
            CreateTypeHandlerTable();
        }

        public Type loadMbin(string mbinPath)               // sets public var mbinData to data and returns the type
        {
            using (libMBIN.MBINFile mbin = new libMBIN.MBINFile(mbinPath))
            {
                mbin.Load(); // load the header information from the file
                             // The type of the actual data is the actual structure type, eg. GcColour
                libMBIN.Models.NMSTemplate data = mbin.GetData(); // populate the data struct.
                mbinData = data;                    // Assign public Variable
                Type type = data.GetType(); //
                Debug.WriteLine("Data :" + data);
                Debug.WriteLine("Type :" + type);
                return type;
            }
        }

        public void parseMbin(string mbinPath)                // going to use the type from the Tuple created in loadMbin
        {
            Type type = loadMbin(mbinPath);

            IOrderedEnumerable<System.Reflection.FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);

            if (mbinData == null)
            {
                Debug.WriteLine("mbinData is null.  Can't parse Mbin as it's not loaded.");
            }
            else
            {
                foreach (System.Reflection.FieldInfo fieldinfo in fields)
                {
                    Debug.WriteLine($"type = {fieldinfo.FieldType}, name = {fieldinfo.Name}, value = {fieldinfo.GetValue(mbinData)}");    //write all fields to debug
                                                                                                                                          //Check for NMSAttribute ignore -code by @GaticusHax
                    var attributes = (libMBIN.Models.NMSAttribute[])fieldinfo.GetCustomAttributes(typeof(libMBIN.Models.NMSAttribute), false);
                    libMBIN.Models.NMSAttribute attrib = null;
                    if (attributes.Length > 0) attrib = attributes[0];
                    bool ignore = false;
                    if (attrib != null) ignore = attrib.Ignore;

                    if (ignore == true)                                         // Skip if ignore is set otherwise do stuff
                    {
                        Debug.WriteLine("Ignore found... skipping");
                    }
                    else
                    {
                        //TypeHandlerTable[fieldinfo.FieldType](fieldinfo, NoMansGUI.MainWindow.AppWindow.ControlEditor);

                        TypeHandlerCallback handler;                                                                // This stuff allows exceptions
                        TypeHandlerTable.TryGetValue(fieldinfo.FieldType, out handler);                             //   |
                        if (handler != null) handler(fieldinfo, NoMansGUI.MainWindow.AppWindow.ControlEditor);      //   /
                    }
                }
            }
        }

        public void HandleBool(FieldInfo fieldInfo, StackPanel destinationPanel)
        {
            Debug.WriteLine("Boolean Detected");
            Label labelName = new Label();
            labelName.Content = fieldInfo.Name;

            CheckBox checkBox = new CheckBox();
            Boolean checkValue = Convert.ToBoolean(fieldInfo.GetValue(mbinData));
            checkBox.IsChecked = checkValue;

            destinationPanel.Children.Add(labelName);
            destinationPanel.Children.Add(checkBox);
        }

        public void HandleInt(FieldInfo fieldInfo, StackPanel destinationPanel)
        {
            Debug.WriteLine("Int Detected");
            Label labelName = new Label();
            labelName.Content = fieldInfo.Name;

            TextBox intText = new TextBox();
            intText.Text = fieldInfo.GetValue(mbinData).ToString();

            destinationPanel.Children.Add(labelName);
            destinationPanel.Children.Add(intText);
        }

        public void HandleByte(FieldInfo fieldInfo, StackPanel destinationPanel)
        {
            Debug.WriteLine("Byte Detected");
            Label labelName = new Label();
            labelName.Content = fieldInfo.Name;

            TextBox intText = new TextBox();
            intText.Text = fieldInfo.GetValue(mbinData).ToString();

            destinationPanel.Children.Add(labelName);
            destinationPanel.Children.Add(intText);
        }

        public void HandleString(FieldInfo fieldInfo, StackPanel destinationPanel)
        {
            Debug.WriteLine("String Detected");
            Label labelName = new Label();
            labelName.Content = fieldInfo.Name;

            TextBox intText = new TextBox();
            intText.Text = fieldInfo.GetValue(mbinData).ToString();

            destinationPanel.Children.Add(labelName);
            destinationPanel.Children.Add(intText);
        }

        // etc .. 
        //private void Foo()                                                // I'm thinking this is meant to be where all the stuff actually happens?
        //{
        //    // etc ...
        //    foreach (var fieldInfo in fields)
        //    {
        //        Debug.WriteLine($"Type = {fieldInfo.FieldType}, Name = {fieldInfo.Name}, Value = {fieldInfo.GetValue(data)}");    //Write all fields to Debug
        //        TypeHandlerTable[fieldInfo.FieldType](fieldInfo);
        //    }
        //}
    }
}
