using Caliburn.Micro;
using NoMansGUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels
{
    [Export(typeof(SettingsViewModel))]
    public class SettingsViewModel : Screen
    {
        #region Fields
        private string _unpakdFiles;
        private string _pcbanks;
        private string _modelViewer;
        private string _selectedTheme;
        #endregion

        #region Properties
        public string UnpakdFiles
        {
            get { return _unpakdFiles; }
            set
            {
                _unpakdFiles = value;
                NoMansGUI.Properties.Settings.Default.pathUnpakdFiles = _unpakdFiles;
                NotifyOfPropertyChange(() => UnpakdFiles);
            }
        }

        public string PcBanks
        {
            get { return _pcbanks; }
            set
            {
                _pcbanks = value;
                NoMansGUI.Properties.Settings.Default.pathPCBanks = _pcbanks;
                NotifyOfPropertyChange(() => PcBanks);
            }
        }

        public string ModelViewer
        {
            get { return _modelViewer; }
            set
            {
                _modelViewer = value;
                NoMansGUI.Properties.Settings.Default.pathModelViewer = _modelViewer;
                NotifyOfPropertyChange(() => ModelViewer);
            }
        }

        public string SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                NoMansGUI.Properties.Settings.Default.currentTheme = _selectedTheme;
                NotifyOfPropertyChange(() => SelectedTheme);
            }
        }
        #endregion

        #region Constructor
        [ImportingConstructor]
        public SettingsViewModel()
        {
            //Load the inital settings when we create this viewmodel
            LoadSettings();
        }
        #endregion

        #region Methods
        private void LoadSettings()
        {
            //Don't use the public properties, as that would trigger the set and uselessly update the settings. I'll move them out of the sets 
            //at some point, as it's not the best place for them.
            _unpakdFiles = Settings.Default.pathUnpakdFiles;
            _pcbanks = Settings.Default.pathPCBanks;
            _modelViewer = Settings.Default.pathModelViewer;
            _selectedTheme = Settings.Default.currentTheme;
        }

        public void BrowseUnpakdFiles()
        {
            System.Windows.Forms.FolderBrowserDialog browseUnpakdFilesDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browseUnpakdFilesDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                UnpakdFiles = browseUnpakdFilesDialog.SelectedPath;
            }

        }

        public void BrowsePcbanks()
        {
            System.Windows.Forms.FolderBrowserDialog browsePcbanksDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browsePcbanksDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PcBanks = browsePcbanksDialog.SelectedPath;
            }
        }

        public void BrowseModelViewer()
        {
            System.Windows.Forms.FolderBrowserDialog browseModelViewerDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browseModelViewerDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ModelViewer = browseModelViewerDialog.SelectedPath;
            }
        }

        public void SaveSettings()
        {
            NoMansGUI.Properties.Settings.Default.Save();
            this.TryClose();
        }

        public void DiscardSettings()
        {
            this.TryClose();
        }
        #endregion
    }
}
