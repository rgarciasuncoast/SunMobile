using System;
using Foundation;
using SunBlock.DataTransferObjects.Culture;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.iOS.Common;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Deposits
{
    public partial class DepositsEnrollmentViewController : BaseViewController, ICultureConfigurationProvider
	{
        public event Action<bool> EnrollmentFinished = delegate{};

		public string AgreementText { get; set; }

        public DepositsEnrollmentViewController(IntPtr handle) : base(handle)
        {
        }

		public override void SetCultureConfiguration()
		{
            NavigationItem.Title = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "EA21E6DE-7F1E-495D-874F-0F8B5BFBD2B4", "Deposits");
			CultureTextProvider.SetMobileResourceText(lblRemoteDepositsEnrollmentHeader, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "46de5fa8-11ee-4be9-b67c-2125edb6ccd2");
			CultureTextProvider.SetMobileResourceText(txtEmail, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "eb55c122-1c1c-462a-b0c8-a69a0079d836");
			CultureTextProvider.SetMobileResourceText(txtCellPhone, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "8b6097d5-dec2-4a23-b400-0092be7786fd");
			CultureTextProvider.SetMobileResourceText(lblAgreedToSms, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "aa717b37-119c-4000-b881-6a1411a687ee");
			CultureTextProvider.SetMobileResourceText(lblAcceptAgreement, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "aeb16a61-1380-4cff-8fab-283ead069470");
			CultureTextProvider.SetMobileResourceText(btnEmailAgreement, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "81035d23-1539-4f59-833c-fd3ae44a8413");
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();			

            var cancelText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "18AED375-2518-4118-843A-0C9007D70043", "Cancel");
            var leftButton = new UIBarButtonItem(cancelText, UIBarButtonItemStyle.Plain, null);
            leftButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetLeftBarButtonItem(leftButton, false);
			leftButton.Enabled = true;
            leftButton.Clicked += (sender, e) => CancelEnrollment();

            var nextText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "4A53F32A-831E-4EA5-82AA-E95E1756BA34", "Next");
            var rightButton = new UIBarButtonItem(nextText, UIBarButtonItemStyle.Plain, null);
            rightButton.TintColor = AppStyles.TitleBarItemTintColor;
            NavigationItem.SetRightBarButtonItem(rightButton, false);
            NavigationItem.RightBarButtonItem.Enabled = false;
			rightButton.Clicked += (sender, e) => SetMemberRemoteDepositsInfo();

			txtEmail.EditingChanged += (sender, e) => Validate();
			txtCellPhone.EditingChanged += (sender, e) => Validate();
			txtCellPhone.ShouldChangeCharacters = (textField, range, replacementString) => 
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return replacementString.IsNumericOrEmpty() && newLength <= 10;
			};

			switchAgreedToSms.ValueChanged += (sender, e) => Validate();
			switchAcceptedAgreement.ValueChanged += (sender, e) => Validate();
			btnEmailAgreement.TouchUpInside += (sender, e) => EmailAgreement();

			webViewAgreementText.LoadHtmlString(AgreementText, null);

			txtEmail.BecomeFirstResponder();
        }

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			GeneralUtilities.CloseKeyboard(View);
		}

		private void EmailAgreement()
		{
            var titleText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "795339E0-532D-4D98-A3CD-CF912276569C", "Remote Deposits Agreement");
			GeneralUtilities.SendEmail(this, null, "Remote Deposits Agreement", AgreementText, true);
		}

        private void CancelEnrollment()
        {
            NavigationController.PopViewController(true);
            EnrollmentFinished(false);
        }

		private SetMemberRemoteDepositsInfoRequest PopulateRequest()
		{
			var request = new SetMemberRemoteDepositsInfoRequest
			{
				AgreedToTerms = switchAcceptedAgreement.On,
				MemberId = SessionSettings.Instance.UserId,
				SmsAlertsEnabled = switchAgreedToSms.On,
				PhoneNumber = txtCellPhone.Text,
				Email = txtEmail.Text
			};

			return request;
		}

        private void Validate()
        {
			var request = PopulateRequest();

            var methods = new DepositMethods();
			NavigationItem.RightBarButtonItem.Enabled = methods.ValidateSetMemberRemoteDepositsInfoRequest(request);            
        }

        private async void SetMemberRemoteDepositsInfo()
        {
			var request = PopulateRequest();
			var message = string.Empty;

			if (!string.IsNullOrEmpty(request.Email) && !StringUtilities.IsValidEmail(request.Email))
			{
				message = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "bc064415-7cac-439c-88b9-4b1b646b2eb8", "Email Address is not valid.");
				message += "\n";
			}

			if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber.Length != 10)
			{
				message += CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "6115eb73-b46c-490e-b69a-70a1fbd7d5c8", "Phone Number must be 10 digits.");
			}

			if (message == string.Empty)
			{				
				ShowActivityIndicator();

				var methods = new DepositMethods();
				var response = await methods.SetMemberRemoteDepositsInfo(request, null);

				HideActivityIndicator();

				if (response != null)
				{
					EnrollmentFinished(true);
					NavigationController.PopViewController(true);
				}
			}
			else
			{
				await AlertMethods.Alert(View, "SunMobile", message, "OK");
			}
        }
	}
}