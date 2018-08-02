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
        // ===========================
        // = Public Var Declarations =
        // ===========================

        public libMBIN.Models.NMSTemplate mbinData = null;                                      // set mbinData as public and null
        public Type mbinType = null;
        public String mbinPath = null;
        public StackPanel ControlEditor = NoMansGUI.MainWindow.AppWindow.ControlEditor;         // Set ControlEditor as default Stack Panel (can be changed when calling TypeHandler
        public TreeView mainTree = NoMansGUI.MainWindow.AppWindow.mainTree;                     // Set mainTree as public to be used everywhere.
        public TreeViewItem treeRoot;
        public delegate void TypeHandlerCallback(FieldInfo fieldInfo, TreeViewItem treeViewItem);
        public Dictionary<Type, TypeHandlerCallback> TypeHandlerTable { get; set; }


        // ==============================
        // = Logic Class Initialization =
        // ==============================
        public Logic()                                                         // this is the constructor
        {
            CreateTypeHandlerTable();
        }

        public void CreateTypeHandlerTable()
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
                { typeof( libMBIN.Models.Structs.Vector4f), HandleStruct },
                { typeof( libMBIN.Models.Structs.Vector2f), HandleStruct },
                { typeof( libMBIN.Models.Structs.Colour), HandleStruct },
                { typeof( libMBIN.Models.Structs.GcSeed), HandleStruct },
                { typeof( libMBIN.Models.Structs.VariableSizeString), HandleStruct },
                { typeof( System.Collections.Generic.List<System.Single>), HandleString },
                { typeof( Single), HandleString },
                { typeof( Single[]), HandleString },
                { typeof( Double), HandleString },
            };

        }

        public TypeHandlerCallback GetTypeHandler(Type type)
        {
            return TypeHandlerTable[type];
        }

        // =====================
        // = Main Program Loop =
        // =====================

        public void parseMbin(string mbinPath)                // going to use the type from the Tuple created in loadMbin
        {
            Type mbinType = loadMbin(mbinPath);

            IOrderedEnumerable<System.Reflection.FieldInfo> fields = mbinType.GetFields().OrderBy(field => field.MetadataToken);

            if (mbinData == null)
            {
                Debug.WriteLine("mbinData is null.  Can't parse Mbin as it's not loaded.");
            }
            else
            {
                TreeViewItem treeRoot = new TreeViewItem();
                mainTree.Items.Add(treeRoot);
                iterateFields(fields, treeRoot);
            }
        }

        public Type loadMbin(string mbinPath)               // sets public var mbinData to data and returns the type
        {
            using (libMBIN.MBINFile mbin = new libMBIN.MBINFile(mbinPath))
            {
                mbin.Load(); // load the header information from the file
                             // The type of the actual data is the actual structure type, eg. GcColour
                libMBIN.Models.NMSTemplate data = mbin.GetData(); // populate the data struct.
                mbinData = data;                    // Assign public Variable
                mbinType = mbinData.GetType();      //  /
                Debug.WriteLine("Data :" + mbinData);
                Debug.WriteLine("Type :" + mbinType);
                return mbinType;
            }
        }

        public void iterateFields(IOrderedEnumerable<System.Reflection.FieldInfo> fields, TreeViewItem treeViewItem)
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
                    doHandlerStuff(fieldinfo, treeViewItem);
                }
            }
        }

        public void doHandlerStuff(FieldInfo fieldinfo, TreeViewItem treeViewItem)
        {
            //TypeHandlerTable[fieldinfo.FieldType](fieldinfo, NoMansGUI.MainWindow.AppWindow.ControlEditor);

            TypeHandlerCallback handler;                                                                // This stuff allows exceptions
            TypeHandlerTable.TryGetValue(fieldinfo.FieldType, out handler);
            if (handler != null) handler(fieldinfo, treeViewItem);
            else
            {                                                                                           // And this handles the exception as a string
                Debug.WriteLine("<!!!!BIG ERROR YOU WANT TO SEE!!!!>");
                Debug.WriteLine("Field Type not found in dictionary :" + fieldinfo.FieldType.ToString());
                Debug.WriteLine("Going to default it as STRING type");
                System.Windows.MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("The " + fieldinfo.FieldType.ToString() + " Field Type was not found in the dictionary.  Please send a message to @theFisher86 on the NMS Modding Discord and let him know you received this error.  Please mention the Field Type from this error message and what MBIN you were opening.  Or just hit Alt+PrtScrn and send him a screenshot of this error box. \n" + "\n Field Type: " + fieldinfo.FieldType.ToString() + "\n MBIN :" + mbinPath.ToString());
                // Need to implement OctoKit here to send an issue to the GitHub.  Include error message, user Discord name and everything contained in the MessageBox above.
                TextBox stringText = new TextBox();
                stringText.Text = fieldinfo.GetValue(mbinData).ToString();

                createControl(fieldinfo.Name, stringText, treeViewItem);

            }
        }

        public void createControl(string label, Control control, TreeViewItem treeViewItem)
        {
            Debug.WriteLine("Creating Control " + control.ToString() + " in " + treeViewItem.ToString());
            Label labelName = new Label();
            labelName.Content = label;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(labelName);
            stackPanel.Children.Add(control);

            treeViewItem.Items.Add(stackPanel);
        }

        // =================
        // = Type Handlers =
        // =================

        public void HandleBool(FieldInfo fieldInfo, TreeViewItem treeViewItem)
        {
            Debug.WriteLine("Boolean Detected");

            CheckBox checkBox = new CheckBox();
            Boolean checkValue = Convert.ToBoolean(fieldInfo.GetValue(mbinData));
            checkBox.IsChecked = checkValue;

            createControl(fieldInfo.Name, checkBox, treeViewItem);
        }

        public void HandleInt(FieldInfo fieldInfo, TreeViewItem treeViewItem)
        {
            Debug.WriteLine("Int Detected");

            TextBox intText = new TextBox();
            intText.Text = fieldInfo.GetValue(mbinData).ToString();

            createControl(fieldInfo.Name, intText, treeViewItem);
        }

        public void HandleByte(FieldInfo fieldInfo, TreeViewItem treeViewItem)
        {
            Debug.WriteLine("Byte Detected");

            TextBox byteText = new TextBox();
            byteText.Text = fieldInfo.GetValue(mbinData).ToString();

            createControl(fieldInfo.Name, byteText, treeViewItem);
        }

        public void HandleString(FieldInfo fieldInfo, TreeViewItem treeViewItem)
        {
            Debug.WriteLine("String Detected");

            TextBox stringText = new TextBox();
            stringText.Text = fieldInfo.GetValue(mbinData).ToString();

            createControl(fieldInfo.Name, stringText, treeViewItem);
        }

        public void HandleStruct(FieldInfo fieldInfo, TreeViewItem treeViewItem)
        {
            Debug.WriteLine("Struct " + fieldInfo.Name.ToString() + " Detected");

            TreeViewItem structroot = new TreeViewItem();
            structroot.Header = fieldInfo.GetValue(mbinData).ToString();
            IOrderedEnumerable<System.Reflection.FieldInfo> fields = fieldInfo.GetType().GetFields().OrderBy(field => field.MetadataToken);
            iterateFields(fields, structroot);
            //treeViewItem.Items.Add(structroot);
            createControl(fieldInfo.Name, structroot, treeViewItem);
        }
    }
}
