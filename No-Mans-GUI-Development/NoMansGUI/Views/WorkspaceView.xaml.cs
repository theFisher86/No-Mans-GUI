using NoMansGUI.Docking;
using NoMansGUI.Properties;
using NoMansGUI.ViewModels;
using System;
using System.Collections.Generic;
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
    public partial class WorkspaceView : IShellView
    {
        public WorkspaceView()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                string m = ex.Message;
            }
        }

        static private string GetLayoutFilePath()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(folder, "NoMansGUI");
            return System.IO.Path.Combine(specificFolder, "NoMansGUI.layout");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        public void LoadLayout(Stream stream, Action<ITool> addToolCallback, Action<IDocument> addDocumentCallback, Dictionary<string, ILayoutItem> itemsState)
        {
            LayoutUtility.LoadLayout(Manager, stream, addDocumentCallback, addToolCallback, itemsState);
        }

        public void SaveLayout(Stream stream)
        {
            LayoutUtility.SaveLayout(Manager, stream);
        }

        public void UpdateFloatingWindows()
        {
            //var mainWindow = Window.GetWindow(this);
            //var mainWindowIcon = (mainWindow != null) ? mainWindow.Icon : null;
            //var showFloatingWindowsInTaskbar = ((WorkspaceViewModel)DataContext).ShowFloatingWindowsInTaskbar;
            //foreach (var window in Manager.FloatingWindows)
            //{
            //    window.Icon = mainWindowIcon;
            //    window.ShowInTaskbar = showFloatingWindowsInTaskbar;
            //}
        }
    }
}
