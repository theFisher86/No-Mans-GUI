using Caliburn.Micro;
using libMBIN;
using NMGUIFramework.LayoutItems;
using NoMansGUI.Models;
using NoMansGUI.Properties;
using NoMansGUI.Utils.Events;
using NoMansGUI.Utils.Parser;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace NoMansGUI.ViewModels
{
    public class MBinViewModel : Document
    {
        private MBin _mbin;
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

        public MBinViewModel(MBin mbin)
        {
            _mbin = mbin;
            //ID = _mbin.Filepath;
            DisplayName = _mbin.Name;
            using (MBINFile mbinFile = new MBINFile(_mbin.Filepath))
            {
                mbinFile.Load();
                _template = mbinFile.GetData();
            }

            MbinParser parser = new MbinParser();
            List<MBINField> fields = parser.IterateFields(_template, _template.GetType());
            Fields = new ObservableCollection<MBINField>(fields);
        }

        public void Save(string path)
        {
            if (path == null)
            {
                path = Settings.Default.OutputFolder;
            }

            switch (Path.GetExtension(path).ToLower())
            {
                case ".mbin":
                    _template.WriteToMbin(path);
                    break;
                case ".exml":
                    _template.WriteToExml(path);
                    break;
                default:
                    MessageBox.Show("Invalid extension.  Must convert to MBIN or EXML");
                    break;
            }

            //if(string.IsNullOrEmpty(path))
            //{
            //    MessageBox.Show("Output Folder is not set, cannot save.");
            //    return;
            //}
            //string file = string.Format("{0}.exml", _mbin.Name);
            //_template.WriteToExml(Path.Combine(path, file));
        }

        public void OpenMbin(object sender, MBINField e)
        {
            //First check to ensure we have the right unpacked path
            if (string.IsNullOrEmpty(Settings.Default.pathUnpakdFiles))
            {
                return;
            }

            string fullpath = Path.Combine(Settings.Default.pathUnpakdFiles, (string)e.Value);

            if(File.Exists(fullpath))
            {
                IoC.Get<IEventAggregator>().PublishOnUIThread(new OpenMBINEvent(fullpath));
            }
            else
            {
                MessageBox.Show("Unable to find file at : " + fullpath);
            }
        }

        public void OpenDir(object sender, MBINField e)
        {
            //string fullpath = Path.Combine(Settings.Default.pathUnpakdFiles, (string)e.Value);
            //FileListViewModel.LoadDirectory(fullpath);

            //TODO The above doesn't work.  Basically here we need to just open the directory in the file tree.  I can't figure out how to reference the existing file tree?
            MessageBoxResult result = MessageBox.Show("This doesn't work yet.");
        }
    }
}