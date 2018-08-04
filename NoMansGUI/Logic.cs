using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using libMBIN;
using libMBIN.Models;
using libMBIN.Models.Structs;
using System.Windows.Documents;
//using System.Drawing;
using System.Windows.Media;

namespace NoMansGUI
{

    using TypeHandlerTable = Dictionary<Type, Logic.TypeHandlerCallback>;

    class Logic
    {
        // ===========================
        // = Public Var Declarations =
        // ===========================

        public NMSTemplate mbinData = null;                                      // set mbinData as public and null
        public Type mbinType = null;
        public String mbinPath;
        public StackPanel ControlEditor = NoMansGUI.MainWindow.AppWindow.ControlEditor;         // Set ControlEditor as default Stack Panel (can be changed when calling TypeHandler
        public TreeView mainTree = NoMansGUI.MainWindow.AppWindow.mainTree;                     // Set mainTree as public to be used everywhere.
        public TreeViewItem treeRoot;

        public delegate void TypeHandlerCallback( object owner, FieldInfo field
                                                , string name, object value
                                                , UIElement destinationControl );

        private TypeHandlerTable TypeHandlers  { get; set; }

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

            TypeHandlers = new TypeHandlerTable() {
                { typeof( Boolean     ), HandleBool   },
                { typeof( Byte        ), HandleByte   },
                { typeof( Int16       ), HandleInt    },
                { typeof( Int32       ), HandleInt    },
                { typeof( Int64       ), HandleInt    },
                { typeof( UInt16      ), HandleInt    },
                { typeof( UInt32      ), HandleInt    },
                { typeof( UInt64      ), HandleInt    },
                { typeof( Single      ), HandleString },
                { typeof( Double      ), HandleString },
                                      
                { typeof( String      ), HandleString },

                { typeof( Array       ), HandleArray  },

                { typeof( NMSTemplate ), HandleTemplate },                  //BaseType
            };

            //TypeHandlerTable = new Dictionary<Type, TypeHandlerCallback>() {
            //    { typeof( Boolean ), HandleBool   },
            //    { typeof( Byte    ), HandleByte   },
            //    { typeof( Int32   ), HandleInt    },
            //    { typeof( Int64   ), HandleInt    },
            //    { typeof( UInt16  ), HandleInt    },
            //    { typeof( UInt32  ), HandleInt    },
            //    { typeof( UInt64  ), HandleInt    },
            //    { typeof( Int16   ), HandleInt    },
            //    { typeof( String  ), HandleString },
            //    { typeof( Single  ), HandleString },
            //    { typeof( Double  ), HandleString },
            //    { typeof( Vector4f), HandleStruct },
            //    { typeof( Vector2f), HandleStruct },
            //    { typeof( Colour), HandleStruct },
            //    { typeof( GcSeed), HandleStruct },
            //    { typeof( VariableSizeString), HandleStruct },
            //    { typeof( System.Collections.Generic.List<System.Single> ), HandleString },
            //    { typeof( Single[]), HandleString },
            //    { typeof( List<NMSTemplate> ), HandleStruct },
            //    { typeof( List<NMSAttribute>), HandleStruct },
            //    { typeof( List<libMBIN.Models.Structs>), HandleStruct },
            //    { typeof( NMSAttribute), HandleStruct }
            //    { typeof( NMSTemplate), HandleStruct },
            //};

            //Debug.WriteLine("Trying to get all the classes");
            //Debug.WriteLine("Should use this assembly :" + Assembly.GetAssembly(typeof(GcActionTrigger)));
            //foreach (Type aClass in GetTypesInNamespace("libMbin"))
            //{
            //    Debug.WriteLine("Adding :" + aClass.ToString());
            //    TypeHandlerTable.Add(aClass, HandleStruct);
            //}

        }

        public TypeHandlerCallback GetTypeHandler( Type type ) {
            TypeHandlerCallback handler = null;

            // try to get an explicit type handler
            TypeHandlers.TryGetValue( type, out handler );
            if (handler != null)
            {
                return handler;
            }
            else {                                                          // Only look for BaseType if explicit type not found
                // derived types use a generic handler, so we lookup the BaseType
                TypeHandlers.TryGetValue(type.BaseType, out handler);
                return handler;
            }
        }

        // =====================
        // = Main Program Loop =
        // =====================

        public void parseMbin(string mbinPath)                // going to use the type from the Tuple created in loadMbin
        {
            Type mbinType = loadMbin(mbinPath);

            //IOrderedEnumerable<System.Reflection.FieldInfo> fields = mbinType.GetFields().OrderBy(field => field.MetadataToken);

            if (mbinData == null)
            {
                Debug.WriteLine("mbinData is null.  Can't parse Mbin as it's not loaded.");
            }
            else
            {
                TreeViewItem treeRoot = new TreeViewItem();
                mainTree.Items.Add(treeRoot);
                //iterateFields(fields, treeRoot);
                iterateFields( mbinData, mbinType, treeRoot );
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

        //public void iterateFields(IOrderedEnumerable<System.Reflection.FieldInfo> fields, TreeViewItem treeViewItem)
        public void iterateFields(NMSTemplate data, Type type, UIElement destinationControl)
        {
            IOrderedEnumerable<System.Reflection.FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);
            foreach (FieldInfo fieldinfo in fields)
            {
                Debug.WriteLine($"type = {fieldinfo.FieldType}, name = {fieldinfo.Name}, value = {fieldinfo.GetValue(data)}");    //write all fields to debug
                                                                                                                                      //Check for NMSAttribute ignore -code by @GaticusHax
                var attributes = (NMSAttribute[])fieldinfo.GetCustomAttributes(typeof(NMSAttribute), false);
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
                    doHandlerStuff(data, fieldinfo, destinationControl);
                }
            }
        }

        public void doHandlerStuff(NMSTemplate data, FieldInfo fieldInfo, UIElement destinationControl)
        {
            //TypeHandlerTable[fieldinfo.FieldType](fieldinfo, NoMansGUI.MainWindow.AppWindow.ControlEditor);

            Type type = fieldInfo.FieldType;
            TypeHandlerCallback handler = GetTypeHandler( type );
            //if ( handler == null )                                                            //Gaticus way to handle a missingType (throw error and break)
            //{
            //    throw new System.NotImplementedException($"{type}");
            //}
            if (handler == null)                                                                //theFisher86 way to handle a missingType (Debug output and treat as string)
            {
                //And this handles the exception as a string
                Debug.WriteLine("<!!!!BIG ERROR YOU WANT TO SEE!!!!>");
                Debug.WriteLine("Field Type not found in dictionary :" + type.ToString());
                Debug.WriteLine("Going to default it as STRING type");
                //MessageBoxResult messageBoxResult = MessageBox.Show( "The " + fieldinfo.FieldType.ToString() + " Field Type was not found in the dictionary.  Please send a message to @theFisher86 on the NMS Modding Discord and let him know you received this error.  Please mention the Field Type from this error message and what MBIN you were opening.  Or just hit Alt+PrtScrn and send him a screenshot of this error box. \n" + "\n Field Type: " + fieldinfo.FieldType.ToString() + "\n MBIN :" );
                // Need to implement OctoKit here to send an issue to the GitHub.  Include error message, user Discord name and everything contained in the MessageBox above.
                TextBox stringText = new TextBox();
                stringText.Text = fieldInfo.GetValue(data).ToString();

                CreateControl(fieldInfo.Name, stringText, destinationControl);
            }
                                                                                                // When everything's working do this stuff
            else
            {
                string name = fieldInfo.Name;
                object value = fieldInfo.GetValue(data);
                handler(data, fieldInfo, name, value, destinationControl);
            }
        }

        // ==================
        // = Misc Functions =
        // ==================

        public void CreateControl( string label, Control control, UIElement destinationControl ) {
            Debug.WriteLine( "Creating Control " + control.ToString() + " in " + destinationControl.ToString() );
            Label labelName = new Label();
            labelName.Content = label;

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add( labelName );
            stackPanel.Children.Add( control );

            // New logic so that any control can be created instead of just a TreeViewItem
            if ( destinationControl.GetType() == typeof( TreeViewItem ) ) {
                TreeViewItem treeViewItem = (TreeViewItem) Convert.ChangeType( destinationControl, typeof( TreeViewItem ) );
                treeViewItem.Items.Add( stackPanel );
            }
            else if(destinationControl.GetType() == typeof(StackPanel))
            {
                StackPanel sp = (StackPanel)Convert.ChangeType(destinationControl, typeof(StackPanel));
                sp.Children.Add(stackPanel);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        public Object ConvertNMSColorToColor(Colour nmsColor, bool returnAsWinFormsColor = false)
        {
            Byte A = Convert.ToByte(255 * Convert.ToDouble(nmsColor.A.ToString()));
            Byte R = Convert.ToByte(255 * Convert.ToDouble(nmsColor.R.ToString()));
            Byte G = Convert.ToByte(255 * Convert.ToDouble(nmsColor.G.ToString()));
            Byte B = Convert.ToByte(255 * Convert.ToDouble(nmsColor.B.ToString()));

            if (returnAsWinFormsColor)
            {
                System.Drawing.Color newColor = System.Drawing.Color.FromArgb(R, G, B, A);
                return newColor;
            }
            else
            {
                return Color.FromArgb(A, R, G, B);
            }
        }
        // =================
        // = Type Handlers =
        // =================

        public void HandleArray( object owner, FieldInfo fieldInfo, string name, object value, UIElement destinationControl ) {
            Type type = value.GetType();
            TypeHandlerCallback handler = GetTypeHandler( type.GetElementType() );

            TreeViewItem root = new TreeViewItem();
            root.Header = type.ToString();
            CreateControl( $"{name}", root, destinationControl );

            int i = 0;
            foreach (var element in (Array) value) {
                handler( value, null, $"{fieldInfo.Name}[{i++}]", element, root );
            }
        }

        public void HandleTemplate( object owner, FieldInfo fieldInfo, string name, object value, UIElement destinationControl ) {
            Debug.WriteLine( $"Struct {name} Detected" );

            TreeViewItem structroot = new TreeViewItem();
            structroot.Header = value.ToString();
            CreateControl( name, structroot, destinationControl );

            StackPanel SpecialCaseFound()
            {
                Debug.WriteLine("Special Template Case :" + fieldInfo.GetType().ToString());

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                structroot.Header = stackPanel;
                return stackPanel;
            }

            // Vectors
            Debug.WriteLine("checking for special cases...  Type is :" + value.GetType());
            if (value.GetType().ToString().Contains("Vector")){

                StackPanel headerPanel = SpecialCaseFound();

                IOrderedEnumerable<System.Reflection.FieldInfo> fields = value.GetType().GetFields().OrderBy(field => field.MetadataToken);
                if (fields.Count<FieldInfo>() <= 5)
                {
                    foreach (FieldInfo f in fields)
                    {
                        Debug.WriteLine($"type = {f.FieldType}, name = {f.Name}, value = {f.GetValue(value)}");    //write all fields to debug

                        TextBox txtBox = new TextBox();
                        txtBox.Text = f.GetValue(value).ToString();
                        CreateControl(f.Name.ToString(), txtBox, headerPanel);
                    }
                }
                else
                {
                    Debug.WriteLine("There's too many field in this Vector, not adding them to the Header");
                }

            }
            
            // Color Picker
            else if(value.GetType() == typeof(Colour))
            {
                StackPanel headerPanel = SpecialCaseFound();

                                    //You have to watch it here as ColoUr is the NMS Colour Type and Color is the Regular one.
                Colour color = (Colour)value;

                                    // Create a WinForms ColorDialog b/c WPF is stupid and doesn't have it's own.
                System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();

                //Debug.WriteLine("Red is: " + Color.Red.A.ToString() + ", " + Color.Red.R.ToString() + ", " + Color.Red.G.ToString() + ", " + Color.Red.B.ToString());
                Debug.WriteLine("NMS Color is: " + ConvertNMSColorToColor((Colour)value).ToString());

                                    // Actually Convert the NMS Style RGBA to a System.Drawing.Color type that is compatible with the winForms dialog
                colorDialog.Color = (System.Drawing.Color)ConvertNMSColorToColor((Colour)value, true);

                                    // Now convert it to the System.Windows.Media.Color type and then convert that into a Brush
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = (Color)ConvertNMSColorToColor((Colour)value);
                                    
                                    // Make headerPanel Background color the selected color, throw on a label that displays the RGBA values in NMS Style
                headerPanel.Background = brush;
                Label label = new Label();
                label.Content = "(R:" + color.R.ToString() + " G:" + color.G.ToString() + "B: " + color.B.ToString() + "A: " + color.A.ToString() + ")";
                headerPanel.Children.Add(label);                
            }
            
            // Standard "Build a Tree Branch 
            Debug.WriteLine( "Data Type:" + value.GetType() );
            iterateFields( (NMSTemplate) value, value.GetType(), structroot );
        }

        public void HandleBool( object owner, FieldInfo fieldInfo, string name, object value, UIElement destinationControl ) {
            Debug.WriteLine( "Boolean Detected" );

            CheckBox checkBox = new CheckBox();
            Boolean checkValue = Convert.ToBoolean( value );
            checkBox.IsChecked = checkValue;

            CreateControl( name, checkBox, destinationControl );
        }

        public void HandleByte( object owner, FieldInfo fieldInfo, string name, object value, UIElement destinationControl ) {
            Debug.WriteLine( "Byte Detected" );

            TextBox byteText = new TextBox();
            byteText.Text = value.ToString();

            CreateControl( name, byteText, destinationControl );
        }

        public void HandleInt( object owner, FieldInfo fieldInfo, string name, object value, UIElement destinationControl ) {
            Debug.WriteLine( "Int Detected" );

            TextBox intText = new TextBox();
            intText.Text = value.ToString();

            CreateControl( name, intText, destinationControl );
        }

        public void HandleString( object owner, FieldInfo fieldInfo, string name, object value, UIElement destinationControl ) {
            Debug.WriteLine( "String Detected" );

            TextBox stringText = new TextBox();
            stringText.Text = value.ToString();

            CreateControl( name, stringText, destinationControl );
        }

    }
}
