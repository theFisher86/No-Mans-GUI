using System.Windows.Controls;

namespace NoMansGUI.Utils.Events
{
    public class ExpandedEvent
    {
        public TreeViewItem TreeViewItem { get; private set; }

        public ExpandedEvent(TreeViewItem item)
        {
            TreeViewItem = item;
        }
    }
}
