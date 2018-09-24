using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Models
{
    public class FileViewItem : PropertyChangedBase
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class FileItem : FileViewItem
    {

    }

    public class DirectoryItem : FileViewItem
    {
        private ObservableCollection<FileViewItem> _items;

        public ObservableCollection<FileViewItem> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public DirectoryItem()
        {
            Items = new ObservableCollection<FileViewItem>();
        }
    }
}
