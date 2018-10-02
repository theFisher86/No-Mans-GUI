using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Utils.Events
{
    public class LoadingStatusMessage
    {
        public string Message { get; private set; }

        public LoadingStatusMessage(string message)
        {
            Message = message;
        }
    }

    public class LoadingCompletedEvent
    {
        public LoadingCompletedEvent() { }
    }

    public class SplashClickEvent
    {
        public SplashClickEvent() { }
    }
}
