using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NoMansGUI.Utils.Events
{
    public class TreeCreatedEvent
    {
        public TreeView TreeView { get; private set; }

        public TreeCreatedEvent(TreeView tree)
        {
            TreeView = tree;
        }
    }
}
