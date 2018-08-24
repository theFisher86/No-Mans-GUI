using Caliburn.Micro;
using NoMansGUI.Utils.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.ViewModels
{
    //As you can see we use the IHandle interface here, passing the event to handle. if you look at the bottom of the class
    //you will see the required method that actually handles the event.
    public class SplashWindowViewModel : Screen, IHandle<LoadingStatusMessage>
    {
        private string _status;

        public string StatusMessage
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyOfPropertyChange(() => StatusMessage);
            }
        }

        public SplashWindowViewModel()
        {
            //When using IHandle interfaces, we need to call this in the constructor so it knows it should be listening for events.
            IoC.Get<IEventAggregator>().Subscribe(this);
        }

        //Handles the specific event based on the class. it passes in the class(event) that was published.
        //This way it's very easy to pass around data with the event. This is a very simplistic example.
        public void Handle(LoadingStatusMessage message)
        {
            //The LoadingStatusMessage contains a string for the status message, we simply update our status message with that.
            StatusMessage = message.Message;
        }
    }
}
