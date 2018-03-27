using System;
using System.Web;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects.Authentication;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Navigation;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Profile
{
	public class UpdatePasswordFragment : BaseFragment
	{
		public event Action<bool> Updated = delegate {};
		private EditText txtPassword;
		private EditText txtConfirmPassword;
		private TextView labelInvalidPassword;
		private TextView labelInvalidConfirmPassword;
		private TextView btnUpdate;
		private TextView lblRequirements;
		private GetChangePasswordInformationResponse _getChangePasswordInformationResponse;
		private string _passwordRegex;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.UpdatePasswordView, null);
			RetainInstance = true;

			return view;
		}

		private void ClearAll()
		{
			txtPassword.Text = string.Empty;
			txtConfirmPassword.Text = string.Empty;
			lblRequirements.Text = string.Empty;
			labelInvalidPassword.Text = string.Empty;
			labelInvalidConfirmPassword.Text = string.Empty;
		}

		public override void SetCultureConfiguration()
		{
            try
            {
                ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "64DA1CFB-D334-43CC-ACC1-B3B805D615E4", "Update Password"));
                CultureTextProvider.SetMobileResourceText(btnUpdate, "1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "B5A98FB9-A45C-4EDD-9D3A-7A5290220BC6", "Update");
                txtPassword.Hint = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "EB211187-F1B2-47CE-8454-1A7D40FE4628", "New Password");
                txtConfirmPassword.Hint = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "572133AE-7C0A-4C2D-9BCF-DA627FC4EB8F", "Confirm Password");
            }
            catch(Exception ex)
            {
                Logging.Log(ex, "UpdatePasswordFragment:SetCultureConfiguration");
            }
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			outState.PutString("NewPassword", txtPassword.Text);
			outState.PutString("ConfirmPassword", txtConfirmPassword.Text);

			if (_getChangePasswordInformationResponse != null)
			{
				var json = JsonConvert.SerializeObject(_getChangePasswordInformationResponse);
				outState.PutString("GetChangePasswordInformationResponse", json);
			}

			Validate(txtConfirmPassword, null);

			base.OnSaveInstanceState(outState);
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			txtPassword = Activity.FindViewById<EditText>(Resource.Id.txtNewPassword);
			txtConfirmPassword = Activity.FindViewById<EditText>(Resource.Id.txtConfirmPassword);
			labelInvalidPassword = Activity.FindViewById<TextView>(Resource.Id.lblInvalidPassword);
			labelInvalidConfirmPassword = Activity.FindViewById<TextView>(Resource.Id.lblInvalidConfirmPassword);
			btnUpdate = Activity.FindViewById<TextView>(Resource.Id.btnUpdatePassword);
			btnUpdate.Enabled = false;
			btnUpdate.Click += (sender, e) => UpdatePassword();
			lblRequirements = Activity.FindViewById<TextView>(Resource.Id.txtRequirements);
			labelInvalidPassword.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.Red)));
			labelInvalidConfirmPassword.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(Activity, Resource.Color.Red)));
			txtPassword.TextChanged += Validate;
			txtConfirmPassword.TextChanged += Validate;

			if (savedInstanceState != null)
			{
				txtPassword.Text = savedInstanceState.GetString("NewPassword");
				txtConfirmPassword.Text = savedInstanceState.GetString("ConfirmPassword");

				var json = savedInstanceState.GetString("GetChangePasswordInformationResponse");
                _getChangePasswordInformationResponse =  JsonConvert.DeserializeObject<GetChangePasswordInformationResponse>(json);
			}

			GetChangePasswordInformation();			
		}

		private async void GetChangePasswordInformation()
		{
			if (_getChangePasswordInformationResponse == null)
			{
				var request = new GetChangePasswordInformationRequest();

				ShowActivityIndicator();
				var methods = new AuthenticationMethods();
				_getChangePasswordInformationResponse = await methods.GetChangePasswordInformation(request, Activity);
				HideActivityIndicator();
			}

			if (_getChangePasswordInformationResponse != null && _getChangePasswordInformationResponse.Success)
			{
				_passwordRegex = HttpUtility.UrlDecode(_getChangePasswordInformationResponse.PasswordRegex);
				lblRequirements.Text = _getChangePasswordInformationResponse.CompletePasswordInstructions.Replace("\\n", "\n");
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

			btnUpdate.Enabled = validateConfirmPasswordResponse;
		}

		private async void UpdatePassword()
		{
			var request = new MobileDeviceVerificationRequest<UpdatePasswordRequest> { Payload = RetainedSettings.Instance.Payload.Payload, Request = new UpdatePasswordRequest() };
			request.Request.Password = txtPassword.Text;

			ShowActivityIndicator();

			var methods = new AuthenticationMethods();
			var response = await methods.UpdatePassword(request, Activity, null);

			HideActivityIndicator();

			var ok = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "E35C146F-6CFD-48C1-B1EE-98A1B911815D", "OK");

			if (response != null && response.Result)
			{
				var passSuccessUpdate = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "BA36A8A9-821A-4400-BEF7-F2DF94656673", "Password was successfully updated.");
				await AlertMethods.Alert(Activity, "SunMobile", passSuccessUpdate, ok);
				Logging.Track("Complex password updated.");
				SessionSettings.Instance.ShowPasswordReminder = false;
				((MainActivity)Activity).ShowInfoActionButton(false);
				NavigationService.NavigatePop(false);
				Updated(true);
			}
			else if ((response != null && !response.OutOfBandChallengeRequired) || response == null)
			{
				var passUpdateFailedString = CultureTextProvider.GetMobileResourceText("1BA09B13-24CD-4C24-9DA3-44F2F06639A4", "3158C0C9-0F4C-4957-8BFE-09C2212C9B79", "Password update failed.  {0}");
				await AlertMethods.Alert(Activity, "SunMobile", string.Format(passUpdateFailedString, response?.FailureMessage ?? string.Empty), ok);
			}
		}
	}
}