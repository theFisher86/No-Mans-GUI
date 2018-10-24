using Caliburn.Micro;
using libMBIN;
using NoMansGUI.Models;
using NoMansGUI.Properties;
using NoMansGUI.Utils.AdminTools;
using NoMansGUI.Utils.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private string _savePath;
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

                MBin mBin = new MBin()
                {
                    Name = Path.GetFileNameWithoutExtension(_mbinPath),
                    Filepath = _mbinPath,
                };

                NMSTemplate template = null;
                using (MBINFile mbin = new MBINFile(_mbinPath))
                {
                    mbin.Load();
                    template = mbin.GetData();
                }

                if (template != null)
                {
                    //We now handle the formatting in this custom control, which is loaded into the MainWindowView when done.
                    MBinViewer = new MBinViewModel(mBin);
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

        public void SaveMbin()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "MBIN Files | *.mbin; *.MBIN| EXML Files | *.exml; *.EXML"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _savePath = saveFileDialog.FileName;

                MBinViewer.Save(_savePath);
            }
        }

        public void CloseMbin()
        {
            MBinViewer.TryClose();
            MBinViewer = null;
        }

        public void Exit()
        {
            TryClose();
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
            MessageViewModel vm = new MessageViewModel("About", "", "This GUI was made by Aaron Fisher aka theFisher86 & Ben Murray aka Wannbeuk on the NMS Modding Discord. Would you like to visit us?", CustomDialogButtons.YesNo, CustomDialogIcons.Information);
            CustomDialogResults result = vm.Show();
            if (result == CustomDialogResults.Yes)
            {
                System.Diagnostics.Process.Start("https://discord.gg/9QBKg6Z");
            }
        }
        #endregion
    }
}
