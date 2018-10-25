using NMGUIFramework.Interfaces;
using NMGUIFramework.Layout;
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
            throw new NotImplementedException();
        }
    }
}
