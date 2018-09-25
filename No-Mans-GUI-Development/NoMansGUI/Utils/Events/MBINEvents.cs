using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils.Events
{
    public class OpenMBINEvent
    {
        public string Path { get; private set; }

        public OpenMBINEvent(string path)
        {
            Path = path;
        }
    }
}
