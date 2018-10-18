using Caliburn.Micro;
using NoMansGUI.Models;
using NoMansGUI.Properties;
using NoMansGUI.Utils.Events;
using NoMansGUI.ViewModels.Tools.TreeViewItems;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace NoMansGUI.ViewModels
{
    [Export(typeof(FileListViewModel))]
    [Export(typeof(ToolBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FileListViewModel : ToolBase, IHandle<UnpackedPathSetEvent>, IHandle<ExpandedEvent>
    {
        ObservableCollection<TreeViewItemViewModel> _items;

        public ObservableCollection<TreeViewItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public FileListViewModel() : base("File List")
        {
            DisplayName = "File List";
            if (!string.IsNullOrEmpty(Settings.Default.pathUnpakdFiles) && Directory.Exists(Settings.Default.pathUnpakdFiles))
            {
                LoadFiles();
            }
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        public void LoadFiles()
        {
            Items = new ObservableCollection<TreeViewItemViewModel>(LoadDirectory(Settings.Default.pathUnpakdFiles));
        }

        public List<TreeViewItemViewModel> LoadDirectory(string path)
        {
            var items = new List<TreeViewItemViewModel>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
               items.Add(new DirectoryItemViewModel(new DirectoryItem(dir.FullName), null));
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.mbin"))
            {
                items.Add(new FileItemViewModel(new FileItem(file.FullName), null));
            }

            return items;
        }

        public void OpenFile(MouseButtonEventArgs e, string path)
        {
            if (e.ClickCount == 2)
            {
                // System.Windows.MessageBox.Show("Opened : " + path);
                IoC.Get<IEventAggregator>().PublishOnUIThread(new OpenMBINEvent(path));
            }
        }

        public void Handle(UnpackedPathSetEvent message)
        {
            LoadFiles();
        }

        public void Handle(ExpandedEvent message)
        {
            TreeViewItem item = message.TreeViewItem;
            if(item.Header is FileItem)
            {
                //Open the file (Messy, we shouldn't be handling this in expand.
                FileItem i = item.Header as FileItem;

                IoC.Get<IEventAggregator>().PublishOnUIThread(new OpenMBINEvent(i.FilePath));
                return;
            }
            DirectoryItem header = (DirectoryItem)item.Header;
            //header.Files = new ObservableCollection<TreeViewItemViewModel>(LoadDirectory(header.Dir));
            NotifyOfPropertyChange(() => Items);
        }
    }
}

