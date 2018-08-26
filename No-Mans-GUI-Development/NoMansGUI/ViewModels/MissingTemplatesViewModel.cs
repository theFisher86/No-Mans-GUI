using Caliburn.Micro;
using NoMansGUI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels
{
    public class MissingTemplatesViewModel : Screen
    {
        private List<string> _missingTemplates;
        public List<string> MissingTemplates
        {
            get { return _missingTemplates; }
            set
            {
                _missingTemplates = value;
                NotifyOfPropertyChange(() => MissingTemplates);
            }
        }

        public MissingTemplatesViewModel()
        {
            MissingTemplates = TemplateLogHelper.GetMissingTemplates();
        }

        public void SaveToFile()
        {
            string folder = Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);
            string filename = "missingTemplates.txt";

            File.WriteAllLines(Path.Combine(folder, filename), _missingTemplates);
        }       
    }
}
