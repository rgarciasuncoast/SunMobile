using System;
using System.Collections.Generic;
using Foundation;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Authentication
{
	public partial class AccountVerificationViewController : BaseViewController
	{
		public string Header { get; set; }
		public string OutOfBandTransactionType { get; set; }
		public bool CanUseAtmLastEight { get; set; }
		public event Action<bool> Completed = delegate { };
        private string _last8Text;

		public AccountVerificationViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (!string.IsNullOrEmpty(Header))
			{
				Title = Header;
			}

			txtAnswer.EditingChanged += (sender, e) =>
			{
				if (string.IsNullOrEmpty(((UITextField)sender).Text))
				{
					btnContinue.Enabled = false;
					btnContinue.BackgroundColor = AppStyles.ButtonDisabledColor;
				}
				else
				{
					btnContinue.Enabled = true;
					btnContinue.BackgroundColor = AppStyles.ButtonColor;
				}
			};

			btnSendCode.TouchUpInside += (sender, e) => SendVerificationCode();
			btnContinue.TouchUpInside += (sender, e) => Submit();
			btnContinue.Enabled = false;
			btnContinue.BackgroundColor = AppStyles.ButtonDisabledColor;

			GetAccountVerificationOptions();
		}

		public override void SetCultureConfiguration()
		{
			_last8Text = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "94B16716-0470-4D0A-ADB7-5A9F75B83A59", "Last 8 digits of ATM or Debit Card");			
            lblHeader.Text = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "EDA7EBCC-4366-4E6B-A21C-98FDDCDB4DA8", "To help protect your account, we want to make sure this is you. How should we verify your account?");
            CultureTextProvider.SetMobileResourceText(btnSendCode, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "59E9F9A7-5946-422C-8063-29060A266AEA");
            CultureTextProvider.SetMobileResourceText(btnContinue, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "7DAA9ECD-0758-4121-935A-949A9103C076");
		}

		private async void GetAccountVerificationOptions()
		{
			ShowActivityIndicator();

			var methods = new AuthenticationMethods();
			var response = await methods.GetAccountVerificationOptions(null, View);

			HideActivityIndicator();

			var items = new List<string>();

			if (response != null && response.Success)
			{
				foreach (var notificationOption in response.NotificationOptions)
				{
					items.Add(notificationOption.Destination);
				}

				if (response.HasCreditCard && CanUseAtmLastEight)
				{
					items.Add(_last8Text);
				}

				txtVerificationType.Text = items[0];
			}

			CommonMethods.CreateDropDownFromTextFieldWithDelegate(txtVerificationType, items, (text) =>
			{
				PickerChanged(text);
			});

			PickerChanged(txtVerificationType.Text);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			GeneralUtilities.CloseKeyboard(View);
		}

		private void PickerChanged(string text)
		{
            if (text.StartsWith(_last8Text, StringComparison.Ordinal))
			{
				btnSendCode.Enabled = false;
				btnSendCode.BackgroundColor = AppStyles.ButtonDisabledColor;
				txtAnswer.Placeholder = "xxxxxxxx";
			}
			else
			{
				btnSendCode.Enabled = true;
				btnSendCode.BackgroundColor = AppStyles.ButtonColor;
                var codeText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "4FC2EDEC-D50D-43EA-8F91-47B679290054", "Code");
				txtAnswer.Placeholder = codeText;
			}

			txtAnswer.Text = string.Empty;
			btnContinue.Enabled = false;
			btnContinue.BackgroundColor = AppStyles.ButtonDisabledColor;
		}

		private async void SendVerificationCode()
		{
			try
			{
				var request = new SendOutOfBandCodeRequest
				{
					TransactionType = OutOfBandTransactionType,
					OutOfBandMessageType = txtVerificationType.Text.Substring(0, txtVerificationType.Text.IndexOf(" ", StringComparison.Ordinal)),
					Payload = RetainedSettings.Instance.Payload
				};

				ShowActivityIndicator();

				var methods = new AuthenticationMethods();
				await methods.SendOutOfBandCode(request, View);

				HideActivityIndicator();

                var codeText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "0746A82B-4ED1-4013-8CBF-BE92E3A2DAE2", "Verification code sent.");
                await AlertMethods.Alert(View, "SunMobile", codeText, "OK");
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "AccountVerificationViewController:SendVerficiationCode");
			}
		}

		private async void Submit()
		{
			var request = new VerifyOutOfBandCodeRequest
			{
				TransactionType = OutOfBandTransactionType,
				Payload = RetainedSettings.Instance.Payload
			};

			if (txtVerificationType.Text.StartsWith("Email", StringComparison.Ordinal))
			{
				request.Code = txtAnswer.Text;
			}
			else if (txtVerificationType.Text.StartsWith("Text", StringComparison.Ordinal))
			{
				request.Code = txtAnswer.Text;
			}
			else
			{
				request.LastEight = txtAnswer.Text;
			}

			ShowActivityIndicator();

			var methods = new AuthenticationMethods();
			var response = await methods.VerifyOutOfBandCode(request, View);

			HideActivityIndicator();

			if (response?.Result?.VerificationState != null && response.Result.VerificationState == "Passed")
			{
				Completed(true);
			}
			else
			{
                var failedText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "32D27C0C-106F-43D1-95D0-6F52ED68ADB4", "Verification failed.");
				await AlertMethods.Alert(View, "SunMobile", response?.FailureMessage ?? failedText, "OK");
				Logging.Track("Verification Events", "Failed verification.", request.Code);
			}
		}
	}
}