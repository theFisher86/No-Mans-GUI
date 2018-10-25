using NMGUIFramework.Commands;
using NMGUIFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NMGUIFramework.LayoutItems
{
    public abstract class Document : LayoutItemBase, IDocument
    {
        private ICommand _closeCommand;
        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(null), p => true)); }
        }
    }
}