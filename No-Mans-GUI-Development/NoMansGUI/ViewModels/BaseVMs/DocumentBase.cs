using Caliburn.Micro;
using NoMansGUI.Resources;
using NoMansGUI.Utils.Localization;
using System;

namespace NoMansGUI.ViewModels
{
    public abstract class DocumentBase : Screen
    {
        private bool _isForcedClosing;
        private string _id;

        /// <summary>
        /// Gets or sets the document content identifier. This might be a file path,
        /// a database PK, etc.
        /// </summary>
        public string ID
        {
            get { return _id; }
            set
            {
                if(value != _id)
                {
                    _id = value;
                    NotifyOfPropertyChange(() => ID);
                }
            }
        }

        public abstract bool IsDirty();

        public override void CanClose(Action<bool> callback)
        {
            // if forcing (@@hack) or not dirty it's OK to close
            if ((_isForcedClosing) || (!IsDirty()))
            {
                callback(true);
                return;
            }

            // else prompt user

            MessageViewModel vm = new MessageViewModel(
                "NoMansGUI",
                "",
                LocalizedStrings.Format(StringResources.DiscardChangesPrompt, DisplayName), 
                CustomDialogButtons.YesNo,
                CustomDialogIcons.Information
            );

            CustomDialogResults result = vm.Show();
            callback(result == CustomDialogResults.Yes);

        }

        public bool CanClose()
        {
            if (!IsDirty()) return true;

            MessageViewModel vm = new MessageViewModel(
               "NoMansGUI",
               "",
               LocalizedStrings.Format(StringResources.DiscardChangesPrompt, DisplayName),
               CustomDialogButtons.YesNo,
               CustomDialogIcons.Information
            );
            CustomDialogResults result = vm.Show();
            bool bResult;
            bResult = result == CustomDialogResults.Yes;
            return bResult;
        }

        public void Close()
        {
            _isForcedClosing = true;
            try
            {
                TryClose();
            }
            finally
            {
                _isForcedClosing = false;
            }
        }
    }
}
