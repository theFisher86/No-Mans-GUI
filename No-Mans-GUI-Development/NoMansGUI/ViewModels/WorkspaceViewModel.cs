using Caliburn.Micro;
using libMBIN;
using NoMansGUI.Docking;
using NoMansGUI.Models;
using NoMansGUI.Utils.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NoMansGUI.ViewModels
{
    [Export(typeof(IShell))]
    public class WorkspaceViewModel : Conductor<IDocument>.Collection.OneActive, IShell, IHandle<OpenMBINEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        private readonly BindableCollection<ITool> _tools;
        public IObservableCollection<ITool> Tools
        {
            get { return _tools; }
        }

        public IObservableCollection<IDocument> Documents
        {
            get { return Items; }
        }

        public ILayoutItem ActiveLayoutItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        [ImportingConstructor]
        public WorkspaceViewModel()
        {


            _eventAggregator = IoC.Get<IEventAggregator>();
            _windowManager = IoC.Get<IWindowManager>();

            _tools = new BindableCollection<ITool>();
            _eventAggregator.Subscribe(this);
        }

        public void OpenDocument(IDocument doc)
        {
            ActivateItem(doc);
        }

        public void CloseDocument(IDocument doc)
        {
            DeactivateItem(doc, true);
        }

        public void LoadMbin()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "MBIN Files | *.mbin; *.MBIN"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string mbinPath = openFileDialog.FileName;

                MBin mBin = new MBin()
                {
                    Name = Path.GetFileNameWithoutExtension(mbinPath),
                    Filepath = mbinPath,
                };

                NMSTemplate template = null;
                using (MBINFile mbin = new MBINFile(mbinPath))
                {
                    mbin.Load();
                    template = mbin.GetData();
                }

                if (template != null)
                {
                    OpenDocument(new MBinViewModel(mBin));
                }              
            }
            else
            {
                Debug.WriteLine("No MBIN Selected");
            }
        }

        public void UpdateFloatingWindows()
        {
            throw new NotImplementedException();
        }

        public void ShowTool<TTool>()
           where TTool : ITool
        {
            ShowTool(IoC.Get<TTool>());
        }

        public void ShowTool(ITool model)
        {
            if (Tools.Contains(model))
            {
                model.IsVisible = true;
            }
            else
            {
                Tools.Add(model);
            }
            model.IsSelected = true;
            ActiveLayoutItem = model;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        #region Menu
        public void SettingsMenu()
        {
            //Using the IoC container we get the instance of the window manager to show the dialog.
            IoC.Get<IWindowManager>().ShowDialog(new SettingsViewModel());
        }

        public void About()
        {
            MessageViewModel vm = new MessageViewModel("About", "", "This GUI was made by Aaron Fisher aka theFisher86 & Ben Murray aka Wannbeuk on the NMS Modding Discord. Would you like to visit us?", CustomDialogButtons.YesNo, CustomDialogIcons.Information);
            CustomDialogResults result = vm.Show();
            if (result == CustomDialogResults.Yes)
            {
                System.Diagnostics.Process.Start("https://discord.gg/9QBKg6Z");
            }
        }

        public void Handle(OpenMBINEvent message)
        {
            MBin mBin = new MBin()
            {
                Name = Path.GetFileNameWithoutExtension(message.Path),
                Filepath = message.Path,
            };

            NMSTemplate template = null;
            using (MBINFile mbin = new MBINFile(message.Path))
            {
                mbin.Load();
                template = mbin.GetData();
            }

            if (template != null)
            {
                OpenDocument(new MBinViewModel(mBin));
            }
        }
        #endregion

    }
}
