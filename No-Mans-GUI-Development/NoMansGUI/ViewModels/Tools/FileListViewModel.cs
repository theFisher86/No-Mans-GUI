using Caliburn.Micro;
using NoMansGUI.Models;
using NoMansGUI.Properties;
using NoMansGUI.Utils.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace NoMansGUI.ViewModels
{
    [Export(typeof(FileListViewModel))]
    [Export(typeof(ToolBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FileListViewModel : ToolBase, IHandle<UnpackedPathSetEvent>, IHandle<ExpandedEvent>
    {
        ObservableCollection<FileViewItem> _items;

        public ObservableCollection<FileViewItem> Items
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
            Items = new ObservableCollection<FileViewItem>(LoadDirectory(Settings.Default.pathUnpakdFiles));
        }

        public List<FileViewItem> LoadDirectory(string path)
        {
            var items = new List<FileViewItem>();
            var dirInfo = new DirectoryInfo(path);

            //Load Directories.
            foreach(var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryItem
                {
                    Name = directory.Name,
                    Path = directory.FullName
                };

                item.Items.Add(null);

                items.Add(item);
            }

            foreach(var file in dirInfo.GetFiles("*.mbin"))
            {
                var item = new FileItem
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                items.Add(item);
            }

            return items;
        }

        public List<FileViewItem> GetItems(string path)
        {
            var items = new List<FileViewItem>();

            var dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryItem
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = new ObservableCollection<FileViewItem>(GetItems(directory.FullName)),
                };

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileItem
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                items.Add(item);
            }

            return items;
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

                IoC.Get<IEventAggregator>().PublishOnUIThread(new OpenMBINEvent(i.Path));
                return;
            }
            DirectoryItem header = (DirectoryItem)item.Header;
            header.Items = new ObservableCollection<FileViewItem>(LoadDirectory(header.Path));
            NotifyOfPropertyChange(() => Items);
        }
    }
}

