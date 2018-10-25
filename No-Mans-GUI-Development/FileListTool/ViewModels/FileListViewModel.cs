using Caliburn.Micro;
using FileListTool.Models;
using NMGUIFramework.Layout;
using NMGUIFramework.LayoutItems;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileListTool.ViewModels
{
    [Export(typeof(IFileListTool))]
    public class FileListViewModel : Tool, IFileListTool
    {
        ObservableCollection<TreeViewItemViewModel> _items;

        public ObservableCollection<TreeViewItemViewModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public override double PreferredWidth
        {
            get { return 200; }
        }

        public FileListViewModel()
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
            Items = new ObservableCollection<TreeViewItemViewModel>(LoadDirectory(Settings.Default.pathUnpakdFiles));
        }

        public List<TreeViewItemViewModel> LoadDirectory(string path)
        {
            var items = new List<TreeViewItemViewModel>();
            DirectoryInfo dirInfo = new DirectoryInfo(path);

            foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            {
               items.Add(new DirectoryItemViewModel(new DirectoryItem(dir.FullName), null));
            }

            foreach (FileInfo file in dirInfo.GetFiles("*.mbin"))
            {
                items.Add(new FileItemViewModel(new FileItem(file.FullName), null));
            }

            return items;
        }

        public void OpenFile(MouseButtonEventArgs e, string path)
        {
            if (e.ClickCount == 2)
            {
                // System.Windows.MessageBox.Show("Opened : " + path);
                IoC.Get<IEventAggregator>().PublishOnUIThread(new OpenMBINEvent(path));
            }
        }
    }
}

