using System.Collections.Generic;
using System.IO;

namespace NoMansGUI.Models
{
    public class FileStructureBase
    { }


    public class FileItem : FileStructureBase
    {
        public string FilePath { get; private set; }
        public string FileName
        {
            get
            {
                return Path.GetFileName(FilePath);
            }
        }

        public FileItem(string FilePath)
        {
            this.FilePath = FilePath;
        }
    }

    public class DirectoryItem : FileStructureBase
    {
        public string Dir { get; private set; }

        public string DirName
        {
            get
            {
                return new DirectoryInfo(Dir).Name;
            }
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
