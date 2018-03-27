using System;
using UIKit;

namespace SunMobile.iOS.Common
{
	public partial class CustomAlertViewController : UIViewController
	{
		public event Action<bool> Completed = delegate {};
		public string Header { get; set; }
		public string Message { get; set; }
		public string CheckBoxText { get; set; }
        public bool CheckBoxHidden { get; set; }
        public string PositiveButtonText { get; set; }
		public string NegativeButtonText { get; set; }
        public bool MakeSwitchOptional { get; set; }
        public bool SwitchOn { get; set; }

		public CustomAlertViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			txtTitle.Text = string.IsNullOrEmpty(Header) ? string.Empty : Header;
			txtMessage.Text = string.IsNullOrEmpty(Message) ? string.Empty : Message;
			txtConfirm.Text = string.IsNullOrEmpty(CheckBoxText) ? string.Empty : CheckBoxText;

			if (!string.IsNullOrEmpty(PositiveButtonText))
			{
				btnPositive.SetTitle(PositiveButtonText, UIControlState.Normal);
			}

			if (!string.IsNullOrEmpty(NegativeButtonText))
			{
				btnNegative.SetTitle(NegativeButtonText, UIControlState.Normal);
			}

            if (MakeSwitchOptional)
            {
                switchConfirm.ValueChanged += SwitchValue;
                btnPositive.Enabled = true;
            }
            else
            {
                switchConfirm.ValueChanged += EnablePositiveButton;
                btnPositive.Enabled = false;
            }

            if (CheckBoxHidden)
            {
                switchConfirm.Hidden = true;
            }

			btnNegative.TouchUpInside += (sender, e) => Completed(false);
			btnPositive.TouchUpInside += (sender, e) => Completed(true);
		}

        private void EnablePositiveButton(object sender, EventArgs e)
        {
            btnPositive.Enabled = ((UISwitch)sender).On;
        }

        private void SwitchValue(object sender, EventArgs e)
        {
            SwitchOn = ((UISwitch)sender).On;
        }
    }
}