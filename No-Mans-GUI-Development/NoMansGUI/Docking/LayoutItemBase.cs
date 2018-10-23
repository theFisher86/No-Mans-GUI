using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NoMansGUI.Docking
{
    public abstract class LayoutItemBase : Screen, ILayoutItem
    {
        private readonly Guid _id = Guid.NewGuid();

        public abstract ICommand CloseCommand { get; }

        public Guid Id
        {
            get { return _id; }
        }

        public string ContentId
        {
            get { return _id.ToString(); }
        }

        public virtual Uri IconSource
        {
            get { return null; }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public virtual bool ShouldReopenOnStart
        {
            get { return false; }
        }

        public virtual void LoadState(BinaryReader reader)
        {
            
        }

        public virtual void SaveState(BinaryWriter writer)
        {
            
        }
    }
}
