using FileListTool.Models;

namespace FileListTool.ViewModels
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
