using Caliburn.Micro;
using libMBIN.Models;
using NoMansGUI.Models;
using NoMansGUI.Utils.Parser;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public void Save()
        {
            //MBINFieldParser parser = new MBINFieldParser();

            //NMSTemplate template = parser.ParseTemplateFromMBINFields(_template, _fields.ToList());

            // string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // _template.WriteToExml(Path.Combine(path, @"Ouput Test\test.exml"));
            _template.WriteToExml(@"C:\Users\xfxma\Desktop\Output Test\test.exml");
        }
    }
}