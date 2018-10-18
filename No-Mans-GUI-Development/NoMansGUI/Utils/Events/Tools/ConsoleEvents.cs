using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils.Events
{
    public class OutputToConsoleEvent
    {
        public string Text { get; private set; }

        public OutputToConsoleEvent(string text)
        {
            Text = text;
        }
    }
}
