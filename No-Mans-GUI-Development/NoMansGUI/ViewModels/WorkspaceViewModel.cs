using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels
{
    public class WorkspaceViewModel : Conductor<IScreen>.Collection.OneActive, IShell
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public BindableCollection<ToolBase> Tools { get; private set; }

        public BindableCollection<DocumentBase> Documents { get; private set; }

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


        public void AddDocument(string id)
        {
            var documents = Items.OfType<DocumentBase>();
            DocumentBase document = documents.FirstOrDefault(e => e.ID == id);
            if(document != null)
            {
                ActivateItem(document);
                return;
            }

            
        }
    }
}
