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
using System.Windows.Shapes;

namespace NoMansGUI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    /// 
    public partial class Settings : Window
    {

        private MainWindow mainWindow;

        public Settings()
        {
            InitializeComponent();

        }

        private void browseUnpakdFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browseUnpakdFilesDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browseUnpakdFilesDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                boxUnpakdFiles.Text = browseUnpakdFilesDialog.SelectedPath;
            }

        }

        private void browsePcbanks_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browsePcbanksDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browsePcbanksDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                boxPcbanks.Text = browsePcbanksDialog.SelectedPath;
            }
        }

        private void browseModelViewer_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog browseModelViewerDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browseModelViewerDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                boxModelViewer.Text = browseModelViewerDialog.SelectedPath;
            }
        }

        private void btnSaveSettings_Click(object sender, RoutedEventArgs e)
        {
            NoMansGUI.Properties.Settings.Default.Save();
            this.Close();
        }

        private void btnDiscardSettings_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void colorThemePicker_Selected(object sender, RoutedEventArgs e)
        {
            NoMansGUI.Properties.Settings.Default.currentTheme = colorThemePicker.SelectedItem.ToString();
        }
    }
}
