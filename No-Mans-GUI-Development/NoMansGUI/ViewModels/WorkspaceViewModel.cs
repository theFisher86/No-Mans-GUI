using Caliburn.Micro;
using libMBIN;
using NoMansGUI.Docking;
using NoMansGUI.Docking.Interfaces;
using NoMansGUI.Models;
using NoMansGUI.Utils.Events;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NoMansGUI.ViewModels
{
    [Export(typeof(IShell))]
    public class WorkspaceViewModel : Conductor<IDocument>.Collection.OneActive, IShell, IHandle<OpenMBINEvent>
    {
        #region Fields
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private ILayoutItem _activeLayoutItem;
        private IShellView _shellView;
        private bool _closing;
        private bool _activateItemGuard = false;

        public event EventHandler ActiveDocumentChanging;
        public event EventHandler ActiveDocumentChanged;

        #region Importing

        /// <summary>
        /// This will import any classes that export with type ITool.
        /// </summary>
        [ImportMany(typeof(ITool))]
        private readonly BindableCollection<ITool> _tools;

        [Import]
        private ILayoutItemStatePersister _layoutItemStatePersister;
        #endregion

        #endregion

        #region Properties
        public virtual string StateFile
        {
            get { return @".\ApplicationState.bin"; }
        }

        public IObservableCollection<ITool> Tools
        {
            get { return _tools; }
        }

        public IObservableCollection<IDocument> Documents
        {
            get { return Items; }
        }

        public ILayoutItem ActiveLayoutItem
        {
            get
            {
                return _activeLayoutItem;
            }
            set
            {
                if (ReferenceEquals(_activeLayoutItem, value))
                {
                    return;
                }
                _activeLayoutItem = value;

                if (value is IDocument)
                {
                    ActivateItem((IDocument)value);
                }
                NotifyOfPropertyChange(() => ActiveLayoutItem);
            }
        }
        #endregion

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

        //TODO: Move this out of the shell.
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

        protected override void OnViewLoaded(object view)
        {
            _shellView = (IShellView)view;

            if (!_layoutItemStatePersister.LoadState(this, _shellView, StateFile))
            {
            }
            foreach(ITool t in Tools)
            {
                IoC.Get<IEventAggregator>().PublishOnUIThread(new OutputToConsoleEvent(string.Format("Tool {0} loaded", t.DisplayName)));
            }
            base.OnViewLoaded(view);
        }

        protected override void OnDeactivate(bool close)
        {
            _closing = true;

            _layoutItemStatePersister.SaveState(this, _shellView, StateFile);
            base.OnDeactivate(close);
        }

        protected override void OnActivationProcessed(IDocument item, bool success)
        {
            if(!ReferenceEquals(ActiveLayoutItem, item))
            {
                ActiveLayoutItem = item;
            }

            base.OnActivationProcessed(item, success);
        }

        public override void ActivateItem(IDocument item)
        {
            if(_closing || _activateItemGuard)
            {
                return;
            }

            _activateItemGuard = true;

            try
            {
                if(ReferenceEquals(item, ActiveItem))
                {
                    return;
                }

                RaiseActiveDocumentChanging();
                var currentActiveItem = ActiveItem;
                base.ActivateItem(item);
                RaiseActiveDocumentChanged();

            }
            finally
            {
                _activateItemGuard = false;
            }

        }

        private void RaiseActiveDocumentChanging()
        {
            ActiveDocumentChanging?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseActiveDocumentChanged()
        {
            ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);
        }

        //TODO: Move this out of the shell.
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
