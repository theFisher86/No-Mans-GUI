using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Deployment.Application;
//using Octokit;                      // For checking GitHub version
using System.Diagnostics;           // Will want to comment this out in production
using libMBIN;


namespace NoMansGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double appVersion;
        public string appVersionString;
        public string mbinPath;
        public static MainWindow AppWindow;
        //
        //  Stopping Point Notes:
        //      Need to figure out how to share the DataContext for WPF stuff between MainWindow and Settings.
        //      Not sure how to share a data context between classes but that's what needs a figuring.
        //
        //

        public MainWindow()
        {
            InitializeComponent();
            // This stuff is for update checking, I don't think it works in Debug mode though so it's currently disabled.
            string appVersionString = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //string appVersionString = "0.1";
            //appVersion = Convert.ToDouble(appVersionString);
            //checkFilePaths();

            // Make this window public so it can be accessed elsewhere (logic.cs)
            AppWindow = this;

            if (App.Args.Length != 0)
            {
                Logic logic = new Logic();
                logic.ParseMbin(App.Args[0]);
            }
        }



        public void checkForUpdates()
        {
            // This uses Octokit to get the newest release from GitHub.  It doesn't work yet.
            // Here's the documentation: http://octokitnet.readthedocs.io/en/latest/releases/
            //var client = new GitHubClient(new ProductHeaderValue("NoMansGUI"));

            string onlineVersion = "1.0";
            //Need to add code to check GitHub version here
            Debug.WriteLine("Current Version as Double: " + Double.TryParse(appVersionString, out appVersion) + "Current Version as String: " + appVersionString);

            MessageBoxResult updateCheckBox = MessageBox.Show("You currently have version " + appVersion + " of this app.  The latest version is " + onlineVersion + ".  Would you like to update?", "Update Available", MessageBoxButton.YesNo);
            if (updateCheckBox == MessageBoxResult.Yes)
            {
                // Open update webpage
            }

            string mbinVersion = libMBIN.Version.GetVersionString();
            MessageBoxResult mbinVersionBox = MessageBox.Show("MBINCompiler DLL is currently using version " + mbinVersion + ".", "libMBIN.dll Version", MessageBoxButton.OK);
        }

        //public void checkFilePaths()
        //{
        //   if(pathUnpakdFiles == null)
        //    {
        //        pathUnpakdFiles = getFolder();
        //    }
        //   if(pathPcbanks == null)
        //    {
        //        pathPcbanks = getFolder();
        //    }
        //}

        public string getFolder()
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Title = "Select Folder";
            System.Windows.Forms.DialogResult dialogResult = fileDialog.ShowDialog();
            return fileDialog.ToString();
        }

        //public void createInputLine(string label, Control control)
        //{
        //    Label theLabel = new Label();
        //    theLabel.Content = label;

        //}

        private void loadMbin_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "MBIN Files | *.mbin; *.MBIN";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mbinPath = openFileDialog.FileName;
                Debug.WriteLine(mbinPath.ToString());

                Logic logic = new Logic();
                logic.ParseMbin(mbinPath.ToString());

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

        private void saveMbin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void closeMbin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settingsMenu_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            DataContext = this;
            settings.Show();
        }

        private void checkUpdates_Click(object sender, RoutedEventArgs e)
        {
            checkForUpdates();
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult aboutResult = MessageBox.Show("This GUI was made by Aaron Fisher aka theFisher86 on the NMS Modding Discord.  Would you like to visit us?", "About", MessageBoxButton.YesNo);
            if (aboutResult == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start("https://discord.gg/9QBKg6Z");
            }

        }

        private void dontClick_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.com/search?q=horse+sex&rlz=1C1GCEA_enUS801US801&source=lnms&tbm=isch&sa=X&ved=0ahUKEwiE1quLtcXcAhXEm-AKHeTEDnkQ_AUICygC&biw=1680&bih=868");
            MessageBoxResult toldYou = MessageBox.Show("I warned you", "Told You Not To Click That", MessageBoxButton.OK);
        }
    }
}
