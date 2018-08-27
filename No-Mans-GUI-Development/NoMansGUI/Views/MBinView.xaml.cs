using NoMansGUI.Models;
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
    /// Interaction logic for MBinView.xaml
    /// </summary>
    public partial class MBinView : UserControl
    {
        public MBinView()
        {
            InitializeComponent();
        }

        private void Stringx10_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
