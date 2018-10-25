using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileListTool.Models
{
    public class DirectoryItem : FileStructureBase
    {
        public string Dir { get; private set; }

        public string DirName
        {
            get { return new DirectoryInfo(Dir).Name; }
        }

        readonly List<FileStructureBase> _files = new List<FileStructureBase>();
        public List<FileStructureBase> Files
        {
            get { return _files; }
        }

        public DirectoryItem(string dir)
        {
            this.Dir = dir;
        }
    }
}
