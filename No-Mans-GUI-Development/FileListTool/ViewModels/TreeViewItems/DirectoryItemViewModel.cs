using FileListTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileListTool.ViewModels
{
    public class DirectoryItemViewModel : TreeViewItemViewModel
    {
        readonly DirectoryItem _directory;

        public DirectoryItemViewModel(DirectoryItem dir, TreeViewItemViewModel parent) : base(parent, true)
        {
            _directory = dir;
        }

        public string Dir { get { return _directory.Dir; } }
        public string DirName { get { return _directory.DirName; } }

        protected override void LoadChildren()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Dir);

            foreach(DirectoryInfo dir in dirInfo.GetDirectories())
            {
                base.Children.Add(new DirectoryItemViewModel(new DirectoryItem(dir.FullName), this));
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.mbin"))
            {
                base.Children.Add(new FileItemViewModel(new FileItem(file.FullName), this));
            }
        }
    }
}
