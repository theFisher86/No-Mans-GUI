using Caliburn.Micro;
using NoMansGUI.Models;
using NoMansGUI.Properties;
using NoMansGUI.Utils.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace NoMansGUI.ViewModels
{
    public class FileListViewModel : ToolBase, IHandle<UnpackedPathSetEvent>
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
            if (!string.IsNullOrEmpty(Settings.Default.pathUnpakdFiles) && Directory.Exists(Settings.Default.pathUnpakdFiles))
            {
                LoadFiles();
            }
        }

        public void LoadFiles()
        {
            Items = new ObservableCollection<FileViewItem>(GetItems(Settings.Default.pathUnpakdFiles));
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
                    Items = GetItems(directory.FullName)
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
    }
}

