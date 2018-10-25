using System.IO;

namespace FileListTool.Models
{
    public class FileItem : FileStructureBase
    {
        public string FilePath { get; private set; }
        public string FileName
        {
            get { return Path.GetFileName(FilePath); }
        }

        public FileItem(string filePath)
        {
            this.FilePath = FilePath;
        }
    }
}
