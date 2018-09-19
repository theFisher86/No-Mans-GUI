using Caliburn.Micro;
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
            MessageBoxAction prompt = new MessageBoxAction
            {
                Caption = App.APP_NAME,
                Text = LocalizedStrings.Format(StringResources.DiscardDocumentChangesPrompt,
                    DisplayName),
                Button = MessageBoxButton.YesNo,
                Image = MessageBoxImage.Question
            };
            prompt.Completed += (sender, e) =>
            {
                callback(prompt.Result == MessageBoxResult.Yes);
            };
            prompt.Execute(null);
        }

        public bool CanClose()
        {
            if (!IsDirty()) return true;

            MessageBoxAction prompt = new MessageBoxAction
            {
                Caption = App.APP_NAME,
                Text = LocalizedStrings.Format(StringResources.DiscardDocumentChangesPrompt,
                    DisplayName),
                Button = MessageBoxButton.YesNo,
                Image = MessageBoxImage.Question
            };

            bool bResult = true;
            prompt.Completed += (sender, e) =>
            {
                bResult = prompt.Result == MessageBoxResult.Yes;
            };
            prompt.Execute(null);
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
