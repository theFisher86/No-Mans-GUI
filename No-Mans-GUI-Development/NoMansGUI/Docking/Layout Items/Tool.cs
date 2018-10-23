using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NoMansGUI.Docking
{
    public abstract class Tool : LayoutItemBase, ITool
    {
        private ICommand _closeCommand;
        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => IsVisible = false, p => true)); }
        }

        public abstract PaneLocation PreferredLocation { get; }

        public virtual double PreferredWidth
        {
            get { return 200; }
        }

        public virtual double PreferredHeight
        {
            get { return 200; }
        }

        private bool _isVisibile;
        public bool IsVisible
        {
            get { return _isVisibile; }
            set
            {
                _isVisibile = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }

        public override bool ShouldReopenOnStart
        {
            get { return true; }
        }

        protected Tool()
        {
            IsVisible = true;
        }
    }
}
