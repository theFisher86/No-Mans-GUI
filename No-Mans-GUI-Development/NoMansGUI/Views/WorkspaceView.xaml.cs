using NoMansGUI.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace NoMansGUI.Views
{
    /// <summary>
    /// Interaction logic for WorkspaceView.xaml
    /// </summary>
    public partial class WorkspaceView : Window
    {
        public WorkspaceView()
        {
            InitializeComponent();

            _dockMgr.Loaded += (sender, e) =>
            {
                //if (Settings.Default.IsAutoLayoutRestoreEnabled)
                //{
                //    Width = Settings.Default.MainWidth;
                //    Height = Settings.Default.MainHeight;
                //    RestoreLayout();
                //} //eif
            };
        }

        private void OnDocumentClosing(object sender, DocumentClosingEventArgs e)
        {
            DocumentBase document = e.Document.Content as DocumentBase;
            if (document == null) return;

            e.Cancel = !document.CanClose();
        }

        private void OnDocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            DocumentBase document = e.Document.Content as DocumentBase;
            if (document != null) document.Close();
        }

        static private string GetLayoutFilePath()
        {
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "WpfCalavaLayout.xml");
        }

        private void SaveLayout()
        {
            //Settings.Default.MainWidth = ActualWidth;
            //Settings.Default.MainHeight = ActualHeight;

            XmlLayoutSerializer serializer = new XmlLayoutSerializer(_dockMgr);
            string sFilePath = GetLayoutFilePath();
            serializer.Serialize(sFilePath);
        }

        private void OnSaveLayoutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveLayout();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void RestoreLayout()
        {
            string sFilePath = GetLayoutFilePath();
            if (!File.Exists(sFilePath)) return;

            // check for well-formedness before restoring:
            // if not well-formed, remove it so we avoid getting stuck.
            try
            {
                XDocument.Load(sFilePath);
            }
            catch (Exception)
            {
                File.Delete(sFilePath);
                throw;
            }

            XmlLayoutSerializer serializer = new XmlLayoutSerializer(_dockMgr);
            serializer.Deserialize(sFilePath);
        }

        private void OnRestoreLayoutClick(object sender, RoutedEventArgs e)
        {
            try
            {
                RestoreLayout();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }
    }
}
