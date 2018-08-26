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
using System.Collections;
using System.Windows.Media;

//I've commented this out for the moment, as it needs a rework to move towards MVVM.
namespace NoMansGUI
{
   using System.Windows.Input;
   using TypeHandlerTable = Dictionary<Type, Logic.TypeHandlerCallback>;

    class Logic
    {
        // ===========================
        // = Public Var Declarations =
        // ===========================

        public String mbinPath;
        public StackPanel ControlEditor = new StackPanel(); //NoMansGUI.MainWindow.AppWindow.ControlEditor;  // Set ControlEditor as default Stack Panel (can be changed when calling TypeHandler
        public TreeView mainTree = new TreeView();          //NoMansGUI.MainWindow.AppWindow.mainTree;   // Set mainTree as public to be used everywhere.
        public TreeViewItem treeRoot;
        //public int debugLabel = 0;

        public delegate Control TypeHandlerCallback(string name, object value);
        private TypeHandlerTable TypeHandlers { get; set; }

        // ==============================
        // = Logic Class Initialization =
        // ==============================
        public Logic()
        {                                                        
            // this is the constructor
            CreateTypeHandlerTable();
        }

        public void CreateTypeHandlerTable()
        {
            //StackPanel ControlEditor = NoMansGUI.MainWindow.AppWindow.ControlEditor;

            TypeHandlers = new TypeHandlerTable() {
                { typeof( Boolean     ), HandleBool     },
                { typeof( Byte        ), HandleByte     },
                { typeof( Int16       ), HandleInt16      },
                { typeof( Int32       ), HandleInt32      },
                { typeof( Int64       ), HandleInt64      },
                { typeof( UInt16      ), HandleInt16      },
                { typeof( UInt32      ), HandleInt32      },
                { typeof( UInt64      ), HandleInt64      },
                { typeof( Single      ), HandleSingle   },
                { typeof( Double      ), HandleDouble   },

                { typeof( String      ), HandleString   },

                { typeof( Array       ), HandleArray    },
                { typeof( List<>      ), HandleList     },

                { typeof( NMSTemplate ), HandleTemplate },
            };

        }

        public TypeHandlerCallback GetTypeHandler(Type type)
        {
            TypeHandlerCallback handler = null;

            // explicit types
            TypeHandlers.TryGetValue(type, out handler);
            if (handler != null) return handler;

            // derived types
            TypeHandlers.TryGetValue(type.BaseType, out handler);
            if (handler != null) return handler;

            // generic types
            TypeHandlers.TryGetValue(type.GetGenericTypeDefinition(), out handler);
            return HandleList;
        }

        // =====================
        // = Main Program Loop =
        // =====================

        public TreeView ParseMbin(string mbinPath)
        {               
            // going to use the type from the Tuple created in loadMbin
            NMSTemplate template = null;
            using (libMBIN.MBINFile mbin = new libMBIN.MBINFile(mbinPath))
            {
                mbin.Load(); // load the header information from the file
                template = mbin.GetData(); // populate the data struct.
            }

            if (template != null)
            {
                TreeViewItem root = new TreeViewItem();
                mainTree.Items.Add(root);
                IterateFields(template, template.GetType(), root);
                return mainTree;
            }
            else
            {
                MessageBox.Show($"Unable to load file!\n{mbinPath}", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public void IterateFields(NMSTemplate data, Type type, TreeViewItem destinationControl)
        {
            IOrderedEnumerable<System.Reflection.FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);
            foreach (FieldInfo fieldInfo in fields)
            {
                //Debug.WriteLine($"type = {fieldInfo.FieldType}, name = {fieldInfo.Name}, value = {fieldInfo.GetValue(data)}");    //write all fields to debug
                                                                                                                                  //Check for NMSAttribute ignore -code by @GaticusHax
                var attributes = (NMSAttribute[])fieldInfo.GetCustomAttributes(typeof(NMSAttribute), false);
                libMBIN.Models.NMSAttribute attrib = null;
                if (attributes.Length > 0) attrib = attributes[0];
                bool ignore = false;
                if (attrib != null) ignore = attrib.Ignore;

                if (!ignore) AddField(destinationControl, data, fieldInfo);
            }
        }

        // ================
        // = Misc Methods =
        // ================
        public void AddField(TreeViewItem parent, object dataOwner, FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;
            TypeHandlerCallback handler = GetTypeHandler(fieldType);
            if (handler == null) throw new System.NotImplementedException($"{fieldType}, {fieldType.UnderlyingSystemType}");

            Control propertyField = handler(fieldInfo.Name, fieldInfo.GetValue(dataOwner));
            AddItem(parent, fieldInfo.Name, propertyField);
        }

        public StackPanel CreateStackPanel(string label, Control control)
        {
            StackPanel panel = new StackPanel();

            if (control.GetType() != typeof(TreeViewItem)) panel.Children.Add(new Label() { Content = label });
            panel.Children.Add(control);

            return panel;
        }

        public void AddItem(TreeViewItem parent, string label, Control control)
        {

            //Just for Debugging Purposes
            //debugLabel = debugLabel + 1;
            //label = label + debugLabel.ToString();

            Debug.WriteLine("Creating Label :" + label);
            parent.Items.Add(CreateStackPanel(label, control));
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

        public TreeViewItem SpecialCaseChecker(Object value)
        {
            // Create TreeViewItem for the Special Case to use
            TreeViewItem root = new TreeViewItem();

            // Handle Vectors
            if (value.GetType().ToString().Contains("Vector"))
            {
                IOrderedEnumerable<FieldInfo> fields = value.GetType().GetFields().OrderBy(field => field.MetadataToken);
                if (fields.Count() <= 5)
                {
                    StackPanel headerPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                    foreach (FieldInfo f in fields)
                    {
                        //Debug.WriteLine($"type = {f.FieldType}, name = {f.Name}, value = {f.GetValue(value)}");    //write all fields to debug

                        TextBox txtBox = new TextBox();
                        txtBox.Text = f.GetValue(value).ToString();
                        headerPanel.Children.Add(CreateStackPanel(f.Name, txtBox));
                    }
                    root.Header = headerPanel;
                }
                else
                {
                    Debug.WriteLine("There's too many field in this Vector, not adding them to the Header");
                }
                return root;
            }

            // Handle NMS Colour
            else if (value.GetType() == typeof(Colour))
            {
                // Watch the spelling here as the NMS Color type is spelled with the u (Colour) while the normal one is without (Color)
                Colour color = (Colour)value;

                // And the headerPanel background as well
                SolidColorBrush solidColorBrush = new SolidColorBrush();
                Label colorLabel = new Label();
                solidColorBrush.Color = (Color)ConvertNMSColorToColor(color);
                Xceed.Wpf.Toolkit.ColorPicker colorPicker = new Xceed.Wpf.Toolkit.ColorPicker();
                colorPicker.SelectedColor = solidColorBrush.Color;
                root.Header = colorPicker;
                //colorLabel.Background = solidColorBrush;
                //colorLabel.Content = "(R:" + color.R.ToString() + " G:" + color.G.ToString() + "B: " + color.B.ToString() + "A: " + color.A.ToString() + ")";
                //colorLabel.MouseDoubleClick += new MouseButtonEventHandler(colorLabelClick);
                //colorLabel.MouseDoubleClick += (sender, e) => colorLabelClick(sender, e, solidColorBrush);      //Honestly don't konw what's happening here but it works?
                //root.Header = colorLabel;
                return root;
            }

            // No Special Case Found, return regular TreeViewItem
            else
            {
                root.Header = value.GetType();
                return root;
            }
        }

        // ==================
        // = Event Handlers = 
        // ==================
        private void colorLabelClick(object sender, EventArgs e, SolidColorBrush solidColorBrush)
        {
            // Create Color Picker Dialog in WinForms since WPF doesn't have one.
            Xceed.Wpf.Toolkit.ColorPicker colorPicker = new Xceed.Wpf.Toolkit.ColorPicker();
            colorPicker.SelectedColor = solidColorBrush.Color;

        }

        // =================
        // = Type Handlers =
        // =================

        public Control HandleBool(string name, object value) => new CheckBox() { IsChecked = (bool)value };

        public Control HandleByte(string name, object value) => new TextBox() { Text = value.ToString() };

        public Control HandleInt16(string name, object value) => new TextBox() { Text = value.ToString() };
        public Control HandleInt32(string name, object value) => new TextBox() { Text = value.ToString() };
        public Control HandleInt64(string name, object value) => new TextBox() { Text = value.ToString() };

        public Control HandleUInt16(string name, object value) => new TextBox() { Text = value.ToString() };
        public Control HandleUInt32(string name, object value) => new TextBox() { Text = value.ToString() };
        public Control HandleUInt64(string name, object value) => new TextBox() { Text = value.ToString() };

        public Control HandleSingle(string name, object value) => new TextBox() { Text = value.ToString() };
        public Control HandleDouble(string name, object value) => new TextBox() { Text = value.ToString() };

        public Control HandleString(string name, object value) => new TextBox() { Text = value.ToString() };

        public Control HandleArray(string name, object value)
        {
            Debug.WriteLine("Array Found! Type: " + value.GetType().ToString() + " Name: " + name.ToString());
            Type type = value.GetType();
            TypeHandlerCallback handler = GetTypeHandler(type.GetElementType());
            if (handler == null) throw new System.NotImplementedException($"{type}");

            //TreeViewItem root = new TreeViewItem() { Header = type };
            TreeViewItem root = new TreeViewItem() { Header = name };

            int i = 0;
            foreach (var element in (Array)value)
            {
                string elementName = $"{name}[{i++}]";
                Control propertyField = handler(elementName, element);
                AddItem(root, elementName, propertyField);
            }

            return root;
        }

        public Control HandleList(string name, object value)
        {
            Debug.WriteLine("List Found! Type: " + value.GetType().ToString() + " Name: " + name.ToString());
            Type type = value.GetType();
            TypeHandlerCallback handler = GetTypeHandler(type.GenericTypeArguments[0]);
            if (handler == null) throw new System.NotImplementedException($"{type}");

            //TreeViewItem root = new TreeViewItem() { Header = type };
            TreeViewItem root = new TreeViewItem() { Header = name };

            int i = 0;
            foreach (var element in (IList)value)
            {
                string elementName = $"{name}[{i++}]";
                Control propertyField = handler(elementName, element);
                AddItem(root, elementName, propertyField);
            }

            return root;
        }

        public Control HandleTemplate(string name, object value)
        {
            Debug.WriteLine("Template Found! Type: " + value.GetType().ToString() + " Name: " + name.ToString());
            Type type = value.GetType();
            //TreeViewItem root = new TreeViewItem() { Header = type };

            TreeViewItem root = SpecialCaseChecker(value);
            IterateFields((NMSTemplate)value, value.GetType(), root);
            return root;
        }
    }
}
