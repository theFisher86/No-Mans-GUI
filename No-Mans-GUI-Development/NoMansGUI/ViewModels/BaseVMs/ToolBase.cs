using Caliburn.Micro;
using System.Windows.Media;

namespace NoMansGUI.ViewModels
{
    public abstract class ToolBase : Screen
    {
        private bool _isSelected;
        private bool _isVisible;
        private string _name;

        public ImageSource IconSource { get; protected set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value.Equals(_isSelected)) return;
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public bool IsVisible
        {
            get { return _isSelected; }
            set
            {
                if (value.Equals(_isVisible)) return;
                _isVisible = value;
                NotifyOfPropertyChange(() = IsVisible);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value.Equals(_name)) return;
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        protected ToolBase(string name)
        {
            _isVisible = true;
            _name = name;
        }
    }
}
