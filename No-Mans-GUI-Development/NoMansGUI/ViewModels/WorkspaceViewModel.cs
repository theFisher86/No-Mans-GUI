using Caliburn.Micro;
using libMBIN;
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
    //[PartCreationPolicy(CreationPolicy.Shared)]
    public class WorkspaceViewModel : Conductor<IScreen>.Collection.OneActive, IShell, IHandle<OpenMBINEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public BindableCollection<ToolBase> Tools { get; private set; }

        public BindableCollection<DocumentBase> Documents { get; private set; }

        [ImportingConstructor]
        public WorkspaceViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, [ImportMany]IEnumerable<ToolBase> tools)
        {
            if (tools == null)
            {
                throw new ArgumentNullException("tools");
            }

            _eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            _windowManager = windowManager ?? throw new ArgumentNullException("windowManager");

            Tools = new BindableCollection<ToolBase>(tools);
            Documents = new BindableCollection<DocumentBase>();

            Items.CollectionChanged += (sender, args) =>
            {
                if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in args.OldItems.OfType<DocumentBase>())
                    {
                        Documents.Remove(item);
                    }
                }
            };

            _eventAggregator.Subscribe(this);
        }


        public void AddDocument(MBinViewModel doc)
        {
            var documents = Items.OfType<DocumentBase>();
            DocumentBase document = documents.FirstOrDefault(e => e.ID == doc.ID);
            if(document != null)
            {
                ActivateItem(document);
                return;
            }

            // In this example we deal with a single document type, but you
            // could easily add some logic to add different types.
            Documents.Add(doc);
            ActivateItem(doc);

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
                    AddDocument(new MBinViewModel(mBin));
                }              
            }
            else
            {
                Debug.WriteLine("No MBIN Selected");
            }
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
                AddDocument(new MBinViewModel(mBin));
            }
        }
        #endregion

    }
}
