using Caliburn.Micro;
using NoMansGUI.Docking;
using NoMansGUI.Utils.Events;
using System.ComponentModel.Composition;

namespace NoMansGUI.ViewModels.Tools
{
    [Export(typeof(ConsoleViewModel))]
    [Export(typeof(ToolBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ConsoleViewModel : Tool, IHandle<OutputToConsoleEvent>
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
