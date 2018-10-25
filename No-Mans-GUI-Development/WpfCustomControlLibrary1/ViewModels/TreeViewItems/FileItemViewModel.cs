using NoMansGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels.Tools.TreeViewItems
{
    public class FileItemViewModel : TreeViewItemViewModel
    {
        readonly FileItem _file;

        public FileItemViewModel(FileItem file, DirectoryItemViewModel parentDir) : base(parentDir, false)
        {
            _file = file;
        }

        public string FilePath { get { return _file.FilePath; } }

        public string FileName { get { return _file.FileName; } }
    }
}
