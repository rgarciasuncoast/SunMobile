using System;
using System.Web;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.Authentication;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Profile
{
	public partial class UpdatePasswordViewController : BaseViewController
	{
		public event Action<bool> Completed = delegate {};
		private string _passwordRegex;

		public UpdatePasswordViewController(IntPtr handle) : base(handle)
		{
		}		

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var update = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "B5A98FB9-A45C-4EDD-9D3A-7A5290220BC6", "Update");
			var rightButton = new UIBarButtonItem(update, UIBarButtonItemStyle.Plain, null);
			rightButton.TintColor = AppStyles.TitleBarItemTintColor;
			NavigationItem.SetRightBarButtonItem(rightButton, false);
			rightButton.Enabled = false;
			rightButton.Clicked += (sender, e) => UpdatePassword();

			txtPassword.EditingChanged += Validate;
			txtConfirmPassword.EditingChanged += Validate;

			txtPassword.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 16;
			};

			txtConfirmPassword.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 16;
			};

			ClearAll();

			GetChangePasswordInformation();
		}

		public override void SetCultureConfiguration()
		{
			Title = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "64DA1CFB-D334-43CC-ACC1-B3B805D615E4", "Update Password");
			txtPassword.AccessibilityHint = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "EB211187-F1B2-47CE-8454-1A7D40FE4628", "New Password");
			txtConfirmPassword.AccessibilityHint = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "572133AE-7C0A-4C2D-9BCF-DA627FC4EB8F", "Confirm Password");
		}

		private void ClearAll()
		{
			txtPassword.Text = string.Empty;
			txtConfirmPassword.Text = string.Empty;
			textViewPasswordInstructions.Text = string.Empty;
			labelInvalidPassword.Text = string.Empty;
			labelInvalidConfirmPassword.Text = string.Empty;
		}

		private async void GetChangePasswordInformation()
		{
			var request = new GetChangePasswordInformationRequest();

			ShowActivityIndicator();

			var methods = new AuthenticationMethods();
			var response = await methods.GetChangePasswordInformation(request, View);

			HideActivityIndicator();

			if (response != null && response.Success)
			{
				_passwordRegex = HttpUtility.UrlDecode(response.PasswordRegex);
				textViewPasswordInstructions.Text = response.CompletePasswordInstructions.Replace("\\n", "\n");
			}
		}

		private void Validate(object sender, EventArgs e)
		{
			var methods = new AuthenticationMethods();

			var validatePasswordResponse = methods.ValidatePassword(txtPassword.Text, _passwordRegex);
			var validateConfirmPasswordResponse = methods.ValidateConfirmPassword(txtPassword.Text, txtConfirmPassword.Text, _passwordRegex);
			var invalidPassString = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "4409721A-E6E5-4E63-93F0-E6DC74915A8B", "Invalid password - check requirements.");

			if (sender == txtPassword)
			{
				labelInvalidPassword.Text = validatePasswordResponse ? string.Empty : invalidPassString;
			}

			if (sender == txtConfirmPassword)
			{
				labelInvalidConfirmPassword.Text = validateConfirmPasswordResponse ? string.Empty : invalidPassString;
			}

			NavigationItem.RightBarButtonItem.Enabled = validateConfirmPasswordResponse;
		}

		private async void UpdatePassword()
		{
			var request = new MobileDeviceVerificationRequest<UpdatePasswordRequest> { Payload = RetainedSettings.Instance.Payload.Payload, Request = new UpdatePasswordRequest() };
			request.Request.Password = txtPassword.Text;

			ShowActivityIndicator();

			var methods = new AuthenticationMethods();
			var response = await methods.UpdatePassword(request, View, NavigationController);

			HideActivityIndicator();

			var ok = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "E35C146F-6CFD-48C1-B1EE-98A1B911815D", "OK");

			if (response != null && response.Result)
			{
				var passSuccessUpdate = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "BA36A8A9-821A-4400-BEF7-F2DF94656673", "Password was successfully updated.");
				await AlertMethods.Alert(View, "SunMobile", passSuccessUpdate, ok);
				Logging.Track("Complex password updated.");
				SessionSettings.Instance.ShowPasswordReminder = false;
				NavigationController.PopViewController(true);
				Completed(true);
			}
			else if ((response != null && !response.OutOfBandChallengeRequired) || response == null)
			{
				var passUpdateFailedString = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "3158C0C9-0F4C-4957-8BFE-09C2212C9B79", "Password update failed.  {0}");
				await AlertMethods.Alert(View, "SunMobile", string.Format(passUpdateFailedString, response?.FailureMessage ?? string.Empty), ok);
			}
		}
	}
}