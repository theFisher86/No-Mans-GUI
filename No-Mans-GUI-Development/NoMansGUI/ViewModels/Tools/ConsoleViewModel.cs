using Caliburn.Micro;
using NMGUIFramework.Layout;
using NMGUIFramework.LayoutItems;
using NoMansGUI.Utils.Events;
using System.ComponentModel.Composition;

namespace NoMansGUI.ViewModels.Tools
{
    [Export(typeof(IConsoleTool))]
    public class ConsoleViewModel : Tool, IConsoleTool, IHandle<OutputToConsoleEvent>
    {
        #region Fields
        private string _output;
        #endregion

        #region Properties
        public string Output
        {
            get { return _output; }
            private set { _output = value; }
        }

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Bottom; }
        }
        #endregion

        #region Constructor
        public ConsoleViewModel()
        {
            DisplayName = "Output";
            IoC.Get<IEventAggregator>().Subscribe(this);
        }
        #endregion

        #region Methods
        public void AddLine(string text)
        {
            Output += text;
            Output += "\u2028"; // Linebreak, not paragraph break
            NotifyOfPropertyChange(() => Output);
        }

        public void Handle(OutputToConsoleEvent message)
        {
            AddLine(message.Text);
        }
        #endregion
    }
}
