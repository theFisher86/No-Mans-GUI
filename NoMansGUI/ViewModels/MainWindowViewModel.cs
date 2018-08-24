using Caliburn.Micro;
using libMBIN;
using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Utils.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
        private MBinViewModel _mbinViewer;
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

        public MBinViewModel MBinViewer
        {
            get { return _mbinViewer; }
            set
            {
                _mbinViewer = value;
                NotifyOfPropertyChange(() => MBinViewer);
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

                NMSTemplate template = null;
                using (MBINFile mbin = new MBINFile(_mbinPath))
                {
                    mbin.Load();
                    template = mbin.GetData();
                }

                if (template != null)
                {
                    //We now handle the formatting in this custom control, which is loaded into the MainWindowView when done.
                    MBinViewer = new MBinViewModel(template);
                }
            }
            else
            {
                Debug.WriteLine("No MBIN Selected");
            }
        }

        public static List<MBINField> IterateFields(NMSTemplate data, Type type)
        {
            List<MBINField> mbinContents = new List<MBINField>();

            IOrderedEnumerable<FieldInfo> fields = type.GetFields().OrderBy(field => field.MetadataToken);
            if (fields != null)
            {
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
                    {                                                                                                                   //
                        mbinContents.Add(new MBINField                                                                                  //
                        {                                                                                                               //
                            Name = fieldInfo.Name,                                                                                      //
                            Value = fieldInfo.GetValue(data).ToString(),                                                                //
                            NMSType = fieldInfo.FieldType.ToString()                                                                    //
                        });                                                                                                             //
                    }                                                                                                                   //
                }
            }
            else
            {
                // Helpers.BasicDialogBox("Error Getting Fields...", "Couldn't get the fields for some reason.\n Data: " + data.ToString() + "\n Will return blank List");
                mbinContents = null;
            }
            return mbinContents;
        } 

        public void ShowMissingTemplates()
        {
            IoC.Get<IWindowManager>().ShowDialog(new MissingTemplatesViewModel());
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
