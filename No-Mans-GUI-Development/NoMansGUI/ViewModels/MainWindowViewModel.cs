using Caliburn.Micro;
using libMBIN;
using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Properties;
using NoMansGUI.Utils.AdminTools;
using NoMansGUI.Utils.Events;
using NoMansGUI.Utils.Parser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace NoMansGUI.ViewModels
{
    /// <summary>
    /// ViewModel for the main window, needs to be exported so the MEF container can find it.
    /// </summary>
    [Export(typeof(MainWindowViewModel))]
    public class MainWindowViewModel : Caliburn.Micro.Screen
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

            //string onlineVersion = "1.0";
            ////Need to add code to check GitHub version here
            //Debug.WriteLine("Current Version as Double: " + Double.TryParse(AppVersionString, out _appVersion) + "Current Version as String: " + AppVersionString);

            ////MessageBoxResult updateCheckBox = MessageBox.Show("You currently have version " + AppVersion + " of this app.  The latest version is " + onlineVersion + ".  Would you like to update?", "Update Available", MessageBoxButton.YesNo);
            //if (updateCheckBox == MessageBoxResult.Yes)
            //{
            //    // Open update webpage
            //}

            //string mbinVersion = libMBIN.Version.GetString();
            ////MessageBoxResult mbinVersionBox = MessageBox.Show("MBINCompiler DLL is currently using version " + mbinVersion + ".", "libMBIN.dll Version", MessageBoxButton.OK);
        }

        public string GetFolder()
        {
            System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Title = "Select Folder";
            System.Windows.Forms.DialogResult dialogResult = fileDialog.ShowDialog();
            return fileDialog.ToString();
        }

        public void LoadMBINs()
        {
            List<string> files = null;
            MbinParser parser = new MbinParser();
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Settings.Default.RecentFolder;
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Settings.Default.RecentFolder = fbd.SelectedPath;
                    Settings.Default.Save();
                    var ext = new List<string> { ".mbin", ".MBIN" };
                    files = Directory.GetFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories).Where(s => ext.Contains(Path.GetExtension(s))).ToList(); 
                }
            }

            if(files != null)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    NMSTemplate template = null;
                    try
                    {
                        using (MBINFile mbin = new MBINFile(files[i]))
                        {
                            mbin.Load();
                            template = mbin.GetData();
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Unable to parse mbin " + files[i]);
                        Console.WriteLine("Exception: " + ex.ToString());
                    }

                    if (template != null)
                    {
                        parser.IterateFields(template, template.GetType());
                        Console.WriteLine("Parsed : " + Path.GetFileName(files[i]));
                    }
                    NMSTemplateTypeList.PrintToFile(Path.GetFileName(files[i]));
                }
            }
        }

        //This is the function called when we click the loadMBIN button.
        public void LoadMBIN()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MBIN Files | *.mbin; *.MBIN"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _mbinPath = openFileDialog.FileName;

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
                //NMSTemplateTypeList.PrintToFile(Path.GetFileName(_mbinPath));
            }
            else
            {
                Debug.WriteLine("No MBIN Selected");
            }
        }

        public void ShowMissingTemplates()
        {
            IoC.Get<IWindowManager>().ShowDialog(new MissingTemplatesViewModel());
        }

        public void PrintDebugList()
        {
           //Defunct
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
            //MessageBoxResult aboutResult = MessageBox.Show("This GUI was made by Aaron Fisher aka theFisher86 on the NMS Modding Discord.  Would you like to visit us?", "About", MessageBoxButton.YesNo);
            //if (aboutResult == MessageBoxResult.Yes)
            //{
            //    System.Diagnostics.Process.Start("https://discord.gg/9QBKg6Z");
            //}
        }

        public void DontClickk()
        {
            //I... I'm just going to leave this commented out for now i think :/
            //System.Diagnostics.Process.Start("https://www.google.com/search?q=horse+sex&rlz=1C1GCEA_enUS801US801&source=lnms&tbm=isch&sa=X&ved=0ahUKEwiE1quLtcXcAhXEm-AKHeTEDnkQ_AUICygC&biw=1680&bih=868");
            //MessageBoxResult toldYou = MessageBox.Show("I warned you", "Told You Not To Click That", MessageBoxButton.OK);
        }
        #endregion
    }
}
