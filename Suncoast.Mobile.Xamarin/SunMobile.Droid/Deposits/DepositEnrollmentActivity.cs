using Android.App;
using Android.OS;
using Android.Widget;
using SunBlock.DataTransferObjects.RemoteDeposits;
using SunMobile.Droid.Common;
using SunMobile.Shared.Methods;
using SunMobile.Shared.StringUtilities;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using Android.Content;
using SunBlock.DataTransferObjects.Culture;
using SunMobile.Shared.Culture;
using System;
using SunMobile.Shared.Logging;

namespace SunMobile.Droid.Deposits
{
	[Activity(Label = "DepositEnrollmentActivity", Theme = "@style/CustomHoloLightTheme")]
    public class DepositEnrollmentActivity : BaseActivity, ICultureConfigurationProvider
    {
		private ImageButton btnCloseWindow;
		private Button btnNext;
		private TextView txtTitle;
		private EditText txtEmail;
		private EditText txtCellPhone;
		private TextView lblAgreedToSms;
		private CheckBox checkBoxCellPhone;
		private TextView txtAgreement;
		private CheckBox checkBoxAgree;
		private Button btnEmail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetupView();			
        }

		public override void SetCultureConfiguration()
		{
            try
            {
                CultureTextProvider.SetMobileResourceText(txtTitle, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "46de5fa8-11ee-4be9-b67c-2125edb6ccd2");
                CultureTextProvider.SetMobileResourceText(txtEmail, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "eb55c122-1c1c-462a-b0c8-a69a0079d836");
                CultureTextProvider.SetMobileResourceText(txtCellPhone, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "8b6097d5-dec2-4a23-b400-0092be7786fd");
                CultureTextProvider.SetMobileResourceText(lblAgreedToSms, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "aa717b37-119c-4000-b881-6a1411a687ee");
                CultureTextProvider.SetMobileResourceText(btnEmail, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "81035d23-1539-4f59-833c-fd3ae44a8413");
            }
			catch (Exception ex)
			{
				Logging.Log(ex, "DepositEnrollmentActivity:SetCultureConfiguration");
			}
		}

        public override void SetupView()
        {
            SetContentView(Resource.Layout.RemoteDepositsEnrollmentView);

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => CancelEnrollment();

            btnNext = FindViewById<Button>(Resource.Id.btnNext);     
			btnNext.Click += (sender, e) => SetMemberRemoteDepositsInfo();
			btnNext.Enabled = false;

			txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);
			txtEmail.TextChanged += (sender, e) => Validate();            

            txtCellPhone = FindViewById<EditText>(Resource.Id.txtCellPhone); 
			txtCellPhone.TextChanged += (sender, e) => Validate();

            checkBoxCellPhone = FindViewById<CheckBox>(Resource.Id.checkBoxCellPhone);
			checkBoxCellPhone.CheckedChange += (sender, e) => Validate();

			lblAgreedToSms = FindViewById<TextView>(Resource.Id.lblAgreedToSms);

			txtAgreement = FindViewById<TextView>(Resource.Id.txtAgreement);
			var agreementText = Intent.GetStringExtra("AgreementText");
			agreementText = agreementText.Replace("<br />", "\n");
			txtAgreement.Text = agreementText;

			checkBoxAgree = FindViewById<CheckBox>(Resource.Id.checkBoxAgree);
			checkBoxAgree.CheckedChange += (sender, e) => Validate();

			btnEmail = FindViewById<Button>(Resource.Id.btnEmail);
			btnEmail.Click += (sender, e) => EmailAgreement();
        }

        private void CancelEnrollment()
        {
			var intent = new Intent();
			intent.PutExtra("ClassName", "DepositEnrollmentActivity");
			intent.PutExtra("Success", false); 

			SetResult(Result.Ok, intent);
			Finish();
        }

		public override void OnBackPressed()
		{
			CancelEnrollment();
		}

		private void EmailAgreement()
		{
            var titleText = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "795339E0-532D-4D98-A3CD-CF912276569C", "Remote Deposits Agreement");
			GeneralUtilities.SendEmail(this, null, titleText, txtAgreement.Text);
		}

		private SetMemberRemoteDepositsInfoRequest PopulateSetMemberRemoteDepositsInfoRequest()
		{
			var request = new SetMemberRemoteDepositsInfoRequest
			{
				AgreedToTerms = checkBoxAgree.Checked,
				MemberId = SessionSettings.Instance.UserId,
				SmsAlertsEnabled = checkBoxCellPhone.Checked,
				PhoneNumber = txtCellPhone.Text,
				Email = txtEmail.Text
			};

			return request;
		}

        private void Validate()
        {
			var request = PopulateSetMemberRemoteDepositsInfoRequest();

            var methods = new DepositMethods();
			btnNext.Enabled = methods.ValidateSetMemberRemoteDepositsInfoRequest(request);            
        }

        private async void SetMemberRemoteDepositsInfo()
        {
			var request = PopulateSetMemberRemoteDepositsInfoRequest();

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
				var response = await methods.SetMemberRemoteDepositsInfo(request, this);

				HideActivityIndicator();

				if (response != null)
				{
					var intent = new Intent();
					intent.PutExtra("ClassName", "DepositEnrollmentActivity");
					intent.PutExtra("Success", true); 
					Finish();
				}
			}
			else
			{
				await AlertMethods.Alert(this, "SunMobile", message, "OK");
			}
        }
    }
}