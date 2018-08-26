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
using Caliburn.Micro;
using NoMansGUI.Utils.Events;

namespace NoMansGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window, IHandle<TreeCreatedEvent>
    {
        //
        //  Stopping Point Notes:
        //      Need to figure out how to share the DataContext for WPF stuff between MainWindow and Settings.
        //      Not sure how to share a data context between classes but that's what needs a figuring.
        //
        //

        public MainWindowView()
        {
            InitializeComponent();
            IoC.Get<IEventAggregator>().Subscribe(this);
            // This stuff is for update checking, I don't think it works in Debug mode though so it's currently disabled.
            //string appVersionString = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //string appVersionString = "0.1";
            //appVersion = Convert.ToDouble(appVersionString);
            //checkFilePaths();

        }

        public void Handle(TreeCreatedEvent message)
        {
            ControlEditor.Children.Add(message.TreeView);
        }
    }
}
