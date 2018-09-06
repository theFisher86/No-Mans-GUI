using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Models
{
    public class FileViewItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class FileItem : FileViewItem
    {

    }

    public class DirectoryItem : FileViewItem
    {
        public List<FileViewItem> Items { get; set; }

        public DirectoryItem()
        {
            Items = new List<FileViewItem>();
        }
    }
}
