using Caliburn.Micro;
using NoMansGUI.Utils.Events;
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

namespace NoMansGUI.Views
{
    /// <summary>
    /// Interaction logic for FileListView.xaml
    /// </summary>
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            try
            {
                InitializeComponent();
            }
            catch(Exception ex)
            {
                string e = ex.Message;
            }
        }

        //private void FileList_Expanded(object sender, RoutedEventArgs e)
        //{
        //    TreeViewItem tvi = (TreeViewItem)e.OriginalSource;
        //    IoC.Get<IEventAggregator>().PublishOnUIThread(new ExpandedEvent(tvi));
        //}
    }
}
