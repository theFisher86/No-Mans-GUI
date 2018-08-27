using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NoMansGUI.ViewModels
{
    public enum CustomDialogButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum CustomDialogIcons
    {
        None,
        Information,
        Warning,
        CriticalWarning,
        Error,
        Ok
    }

    public enum CustomDialogResults
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }

    public class MessageViewModel : Screen
    {
        #region Fields

        private CustomDialogButtons _Buttons = CustomDialogButtons.OK;
        private CustomDialogResults _CustomDialogResult = CustomDialogResults.None;
        private CustomDialogIcons _InstructionIcon = CustomDialogIcons.None;

        private Visibility _additionalDetailsVisibility = Visibility.Visible;
        private Exception _exception = null;

        private string _strCaption = string.Empty;
        private string _strInstructionHeading = string.Empty;
        private string _strInstructionText = string.Empty;

        private Visibility _instructionIconVisibility = Visibility.Visible;
        private BitmapImage _InstructionIconImage;

        private Visibility _yesButtonVisibility = Visibility.Visible;
        private Visibility _noButtonVisibility = Visibility.Visible;
        private Visibility _okButtonVisibility = Visibility.Visible;
        private Visibility _cancelButtonVisibility = Visibility.Visible;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the optional text is displayed to the user when they click to Show Details expander.  Used to provide a detailed explaination to the user.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }


        public Visibility AdditionalDetailsVisibility
        {
            get { return _additionalDetailsVisibility; }
            set { _additionalDetailsVisibility = value; }
        }

        /// <summary>
        /// Gets or sets the buttons that will be displayed.  The default is the OK button.
        /// </summary>
        public CustomDialogButtons Buttons
        {
            get { return _Buttons; }
            set { _Buttons = value; }
        }

        /// <summary>
        /// Gets or sets the dialog box window caption.  The caption is displayed in the window chrome.
        /// </summary>
        public string Caption
        {
            get { return _strCaption; }
            set { _strCaption = value; }
        }

        /// <summary>
        /// Gets or sets the heading text displayed in the dialog box.
        /// </summary>
        public string InstructionHeading
        {
            get { return _strInstructionHeading; }
            set { _strInstructionHeading = value; }
        }

        /// <summary>
        /// Gets or sets the icon displayed to the left of the instruction text.  This value defaults to none.
        /// </summary>
        public CustomDialogIcons InstructionIcon
        {
            get { return _InstructionIcon; }
            set { _InstructionIcon = value; }
        }

        public Visibility InstructionIconVisibility
        {
            get { return _instructionIconVisibility; }
            set { _instructionIconVisibility = value; }
        }

        public BitmapImage InstructionIconImage
        {
            get { return _InstructionIconImage; }
            set { _InstructionIconImage = value; }
        }

        /// <summary>
        /// Gets or sets the text message for the dialog.
        /// </summary>
        public string InstructionText
        {
            get { return _strInstructionText; }
            set { _strInstructionText = value; }
        }


        public Visibility OkButtonVisibility
        {
            get { return _okButtonVisibility; }
            set { _okButtonVisibility = value; NotifyOfPropertyChange(() => OkButtonVisibility); }
        }

        public Visibility NoButtonVisibility
        {
            get { return _noButtonVisibility; }
            set { _noButtonVisibility = value; NotifyOfPropertyChange(() => NoButtonVisibility); }
        }

        public Visibility CancelButtonVisibility
        {
            get { return _cancelButtonVisibility; }
            set { _cancelButtonVisibility = value; NotifyOfPropertyChange(() => CancelButtonVisibility); }
        }

        public Visibility YesButtonVisibility
        {
            get { return _yesButtonVisibility; }
            set { _yesButtonVisibility = value; NotifyOfPropertyChange(() => YesButtonVisibility); }
        }

        #endregion

        #region Constructors

        public MessageViewModel(string strCaption, string strInstructionHeading, string strInstructionText, CustomDialogButtons enumButtons, CustomDialogIcons enumInstructionIcon)
        {
            _strCaption = strCaption;
            _strInstructionHeading = strInstructionHeading;
            _strInstructionText = strInstructionText;
            _Buttons = enumButtons;
            _InstructionIcon = enumInstructionIcon;
        }


        public MessageViewModel(string strCaption, string strInstructionHeading, string strInstructionText, Exception ex, CustomDialogButtons enumButtons, CustomDialogIcons enumInstructionIcon)
        {
            _strCaption = strCaption;
            _strInstructionHeading = strInstructionHeading;
            _strInstructionText = strInstructionText;
            _exception = ex;
            _Buttons = enumButtons;
            _InstructionIcon = enumInstructionIcon;
        }

        #endregion

        #region Methods

        public void OkButton()
        {
            _CustomDialogResult = CustomDialogResults.OK;
            TryClose();
        }

        public void YesButton()
        {
            _CustomDialogResult = CustomDialogResults.Yes;
            TryClose();
        }

        public void NoButton()
        {
            _CustomDialogResult = CustomDialogResults.No;
            TryClose();
        }

        public void CancelButton()
        {
            _CustomDialogResult = CustomDialogResults.Cancel;
            TryClose();
        }

        /// <summary>
        ///     Shows the custom dialog described by the constructor and properties set by the caller, returns CustomDialogResults.
        /// </summary>
        /// <returns>
        ///     A emGovPower.Core.CommonDialog.CustomDialogResults value.
        /// </returns>
        public CustomDialogResults Show()
        {
            if (this.InstructionIcon == CustomDialogIcons.None)
            {
                InstructionIconVisibility = Visibility.Collapsed;
            }
            else
            {
                InstructionIconImage = new BitmapImage(GetIcon(this.InstructionIcon));
            }

            if (this.Exception == null)
            {
                AdditionalDetailsVisibility = Visibility.Collapsed;
            }

            SetButtons();

            IoC.Get<IWindowManager>().ShowDialog(this);
            LogDialog();

            return _CustomDialogResult;
        }

        private Uri GetIcon(CustomDialogIcons enumCustomDialogIcon)
        {
            switch (enumCustomDialogIcon)
            {
                case CustomDialogIcons.Information:
                    return new Uri(@"..\Resources\status_icons\status_05_info.png", UriKind.Relative);
                case CustomDialogIcons.None:
                    return null;
                case CustomDialogIcons.Warning:
                    return new Uri(@"..\Resources\status_icons\status_02_warning_a.png", UriKind.Relative);
                case CustomDialogIcons.CriticalWarning:
                    return new Uri(@"..\Resources\status_icons\status_03_warning_b.png", UriKind.Relative);
                case CustomDialogIcons.Error:
                    return new Uri(@"..\Resources\status_icons\status_04_error.png", UriKind.Relative);
                case CustomDialogIcons.Ok:
                    return new Uri(@"..\Resources\status_icons\status_01_ok.png", UriKind.Relative);
                default:
                    throw new ArgumentOutOfRangeException("enumCustomDialogIcon", enumCustomDialogIcon, "Programmer did not program for this icon.");
            }
        }


        private void LogDialog()
        {
            //TODO - developers, you can log the result here.  There is rich information within this class to provides great tracking of your users dialog box activities.
            //ensure that you review each property and include them in your log entry
            //don't forget to log the Windows user name also
        }


        private void SetButtons()
        {
            switch (this.Buttons)
            {
                case CustomDialogButtons.OK:
                    CancelButtonVisibility = Visibility.Collapsed;
                    NoButtonVisibility = Visibility.Collapsed;
                    YesButtonVisibility = Visibility.Collapsed;
                    break;
                case CustomDialogButtons.OKCancel:
                    NoButtonVisibility = Visibility.Collapsed;
                    YesButtonVisibility = Visibility.Collapsed;
                    break;
                case CustomDialogButtons.YesNo:
                    OkButtonVisibility = Visibility.Collapsed;
                    CancelButtonVisibility = Visibility.Collapsed;
                    break;
                case CustomDialogButtons.YesNoCancel:
                    OkButtonVisibility = Visibility.Collapsed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Buttons", this.Buttons, "Programmer did not program for this selection.");
            }
        }

        #endregion
    }
}
