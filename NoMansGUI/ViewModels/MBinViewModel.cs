using Caliburn.Micro;
using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Utils.Debug;
using NoMansGUI.Utils.Parser;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels
{
    public class MBinViewModel : Screen
    {
        private NMSTemplate _template;
        private ObservableCollection<MBINField> _fields;

        public ObservableCollection<MBINField> Fields
        {
            get { return _fields; }
            set
            {
                _fields = value;
                NotifyOfPropertyChange(() => Fields);
            }
        }

        public MBinViewModel(NMSTemplate template)
        {
            _template = template;
            //This is where the magic happens

            MbinParser parser = new MbinParser();
            List<MBINField> fields = parser.IterateFields(template, template.GetType());
            Fields = new ObservableCollection<MBINField>(fields);
        }
    }
}
