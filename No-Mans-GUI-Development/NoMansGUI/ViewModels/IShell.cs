using Caliburn.Micro;
using NoMansGUI.Docking;

namespace NoMansGUI.ViewModels
{
    public interface IShell
    {
        // TODO: Rename this to ActiveItem.
        ILayoutItem ActiveLayoutItem { get; set; }

        // TODO: Rename this to SelectedDocument.
        IDocument ActiveItem { get; }

        IObservableCollection<IDocument> Documents { get; }
        IObservableCollection<ITool> Tools { get; }

        void ShowTool<TTool>() where TTool : ITool;
        void ShowTool(ITool model);

        void OpenDocument(IDocument model);
        void CloseDocument(IDocument document);

        void Close();
    }
}
