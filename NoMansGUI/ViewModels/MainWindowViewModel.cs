using Caliburn.Micro;
using libMBIN.Models;
using NoMansGUI.Utils.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NoMansGUI.ViewModels
{
    /// <summary>
    /// ViewModel for the main window, needs to be exported so the MEF container can find it.
    /// </summary>
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : Screen
    {
        /* Wannabeuk : Please note i'm doing this following my standard coding styles. I'm aware this might not be ideal
         * everyone, so perhaps we should knock out some guidelines for coding styles? */

        #region Fields
        private double _appVersion;
        private string _appVersionString;
        private string _mbinPath;

        private TreeView _root;
        #endregion

        #region Properties
        public Double AppVersion
        {
            get { return _appVersion; }
            set
            {
                _appVersion = value;
                NotifyOfPropertyChange(() => AppVersion);
            }
        }

        public string AppVersionString
        {
            get { return _appVersionString; }
            set
            {
                _appVersionString = value;
                NotifyOfPropertyChange(() => AppVersionString);
            }
        }
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            //Get the app version
            _appVersionString = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //We need to move this elsewhere, prolly into the bootstrapper and handle from there.
            if (App.Args.Length != 0)
            {
                //Logic logic = new Logic();
                //logic.ParseMbin(App.Args[0]);
            }
        }
        #endregion

        #region Methods
        public void CheckForUpdates()
        {
            // This uses Octokit to get the newest release from GitHub.  It doesn't work yet.
            // Here's the documentation: http://octokitnet.readthedocs.io/en/latest/releases/
            //var client = new GitHubClient(new ProductHeaderValue("NoMansGUI"));

            string onlineVersion = "1.0";
            //Need to add code to check GitHub version here
            Debug.WriteLine("Current Version as Double: " + Double.TryParse(AppVersionString, out _appVersion) + "Current Version as String: " + AppVersionString);

            MessageBoxResult updateCheckBox = MessageBox.Show("You currently have version " + AppVersion + " of this app.  The latest version is " + onlineVersion + ".  Would you like to update?", "Update Available", MessageBoxButton.YesNo);
            if (updateCheckBox == MessageBoxResult.Yes)
            {
                // Open update webpage
            }

            string mbinVersion = libMBIN.Version.GetString();
            MessageBoxResult mbinVersionBox = MessageBox.Show("MBINCompiler DLL is currently using version " + mbinVersion + ".", "libMBIN.dll Version", MessageBoxButton.OK);
        }

        public string GetFolder()
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Title = "Select Folder";
            System.Windows.Forms.DialogResult dialogResult = fileDialog.ShowDialog();
            return fileDialog.ToString();
        }

        //This is the function called when we click the loadMBIN button.
        public void LoadMBIN()
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "MBIN Files | *.mbin; *.MBIN";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _mbinPath = openFileDialog.FileName;
                Debug.WriteLine(_mbinPath.ToString());

                Logic logic = new Logic();
                _root = logic.ParseMbin(_mbinPath.ToString());
                //Hacky as shit, if i've left this in shout at me.
                IoC.Get<IEventAggregator>().PublishOnUIThread(new TreeCreatedEvent(_root));


                //Main MBIN Parsing Code -Disable
                //using (libMBIN.MBINFile mbin = new libMBIN.MBINFile(mbinPath))
                //{
                //    mbin.Load(); // load the header information from the file
                //                 // The type of the actual data is the actual structure type, eg. GcColour
                //    Object data = mbin.GetData(); // populate the data struct.
                //    var type = data.GetType(); //
                //    Debug.WriteLine("Data :" + data);
                //    Debug.WriteLine("Type :" + type);

                //    IOrderedEnumerable<System.Reflection.FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);

                //    //Would rather do this as a Dictionary I think
                //    //IDictionary<string, Array> fieldDict = new Dictionary<string, Array>();
                //    //foreach (var fieldInfo in fields)
                //    //{
                //    //    // Create array of value and type
                //    //    string[] valueType = { fieldInfo.GetValue(data).ToString(), fieldInfo.FieldType.ToString() };

                //    //    // Add stuff as dictionary entry
                //    //    fieldDict.Add(new KeyValuePair<string, Array>(fieldInfo.Name, valueType));
                //    //}


                //    foreach (System.Reflection.FieldInfo fieldinfo in fields)
                //    {
                //        Debug.WriteLine($"type = {fieldinfo.FieldType}, name = {fieldinfo.Name}, value = {fieldinfo.GetValue(data)}");    //write all fields to debug
                //        //Check for NMSAttribute ignore -code by @GaticusHax
                //        var attributes = (libMBIN.Models.NMSAttribute[])fieldinfo.GetCustomAttributes(typeof(libMBIN.Models.NMSAttribute), false);
                //        libMBIN.Models.NMSAttribute attrib = null;
                //        if (attributes.Length > 0) attrib = attributes[0];
                //        bool ignore = false;
                //        if (attrib != null) ignore = attrib.Ignore;

                //        if (ignore == true)                                         // Skip if ignore is set otherwise do stuff
                //        {
                //            Debug.WriteLine("Ignore found... skipping");
                //        }
                //        else
                //        {
                //            Logic.TypeHandlerCallback typeHandlerCallback = new Logic.TypeHandlerCallback(
                //            //if (fieldinfo.FieldType == typeof(Boolean))             //Boolean Type
                //            //{
                //            //    Debug.WriteLine("boolean detected");
                //            //    Label labelname = new Label();
                //            //    labelname.Content = fieldinfo.Name;

                //            //    CheckBox checkbox = new CheckBox();
                //            //    Boolean checkvalue = Convert.ToBoolean(fieldinfo.GetValue(data));
                //            //    checkbox.IsChecked = checkvalue;

                //            //    ControlEditor.Children.Add(labelname);
                //            //    ControlEditor.Children.Add(checkbox);

                //            //}
                //            //else if (fieldinfo.FieldType == typeof(System.Int64))             //int Type
                //            //{
                //            //    Debug.WriteLine("int detected");
                //            //    Label labelname = new Label();
                //            //    labelname.Content = fieldinfo.Name;

                //            //    TextBox intText = new TextBox();
                //            //    intText.Text = fieldinfo.GetValue(data).ToString();

                //            //    ControlEditor.Children.Add(labelname);
                //            //    ControlEditor.Children.Add(intText);

                //            //}
                //            //else if (fieldinfo.FieldType == typeof(System.Byte))                   // byte handling
                //            //{
                //            //    Debug.WriteLine("byte detected");
                //            //    Label labelname = new Label();
                //            //    labelname.Content = fieldinfo.Name;

                //            //    TextBox byteText = new TextBox();
                //            //    byteText.Text = fieldinfo.GetValue(data).ToString();
                //            //}
                //            //else if (fieldinfo.FieldType == typeof(System.String))                // string handling
                //            //{
                //            //    Debug.WriteLine("string detected");
                //            //    Label labelname = new Label();
                //            //    labelname.Content = fieldinfo.Name;

                //            //    TextBox stringText = new TextBox();
                //            //    stringText.Text = fieldinfo.GetValue(data).ToString();
                //            //}
                //            //else if (fieldinfo.Name == nameof(libMBIN.Models.Structs.Colour))       // this can be used to name gcstructs (or color)
                //            //{

                //            //}
                //        }
                //    }
                //} // mbin file is properly disposed/closed automatically


            }
            else
            {
                Debug.WriteLine("No MBIN Selected");
            }
        }

        public void SaveMbin()
        {
            Debug.WriteLine("SaveMbin Clicked");
        }

        public void CloseMbin()
        {
            Debug.WriteLine("CloseMbin Clicked");
        }

        public void Exit()
        {
            Debug.WriteLine("Exit Clicked");
        }

        public void SettingsMenu()
        {
            //Using the IoC container we get the instance of the window manager to show the dialog.
            IoC.Get<IWindowManager>().ShowDialog(new SettingsViewModel());
        }

        public void CheckUpdates()
        {
            CheckForUpdates();
        }

        public void About()
        {
            MessageBoxResult aboutResult = MessageBox.Show("This GUI was made by Aaron Fisher aka theFisher86 on the NMS Modding Discord.  Would you like to visit us?", "About", MessageBoxButton.YesNo);
            if (aboutResult == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start("https://discord.gg/9QBKg6Z");
            }

        }

        public void DontClickk()
        {
            //I... I'm just going to leave this commented out for now i think :/
            //System.Diagnostics.Process.Start("https://www.google.com/search?q=horse+sex&rlz=1C1GCEA_enUS801US801&source=lnms&tbm=isch&sa=X&ved=0ahUKEwiE1quLtcXcAhXEm-AKHeTEDnkQ_AUICygC&biw=1680&bih=868");
            MessageBoxResult toldYou = MessageBox.Show("I warned you", "Told You Not To Click That", MessageBoxButton.OK);
        }
        #endregion
    }
}
