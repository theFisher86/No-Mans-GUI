using System.Windows.Input;

namespace NoMansGUI.Docking
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
