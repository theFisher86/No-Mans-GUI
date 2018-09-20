using NoMansGUI.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace NoMansGUI.ViewModels
{
    [Export(typeof(MissingTemplatesViewModel))]
    [Export(typeof(ToolBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MissingTemplatesViewModel : ToolBase
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

        public MissingTemplatesViewModel() : base("Missing Templates")
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
