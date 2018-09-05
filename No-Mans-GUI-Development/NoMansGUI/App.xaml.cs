using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NoMansGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public enum Skin { Light, Dark }

    public partial class App : Application
    {

        public static string[] Args;

        public static Skin Skin { get; set; } = Skin.Light;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Args = e.Args;
        }
    }
}
