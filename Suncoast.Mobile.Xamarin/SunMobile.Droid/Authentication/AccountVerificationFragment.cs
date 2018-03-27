using System;
using System.Collections.Generic;
using Android.OS;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.Authentication
{
	public class AccountVerificationFragment : BaseFragment
	{
		private Spinner spinnerValidationMethod;
		private EditText txtAnswer;
		private Button btnSendCode;
		private Button btnContinue;

        private string _last8Text;

		public string Header { get; set; }
		public string OutOfBandTransactionType { get; set; }
		public bool CanUseAtmLastEight { get; set; }
		public event Action<bool> Completed = delegate { };
		private GetAccountVerificationOptionsResponse _getAccountVerificationOptionsResponse;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.AccountVerificationView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			try
			{
				outState.PutString("Header", Header);
				outState.PutString("OutOfBandTransactionType", OutOfBandTransactionType);
				outState.PutBoolean("CanUseAtmLastEight", CanUseAtmLastEight);
				outState.PutInt("SpinnerSelection", spinnerValidationMethod.SelectedItemPosition);
				outState.PutString("Answer", txtAnswer.Text);

				var json = JsonConvert.SerializeObject(_getAccountVerificationOptionsResponse);
				outState.PutString("GetAccountVerificationOptionsResponse", json);

				base.OnSaveInstanceState(outState);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "AccountVerificationFragment:OnSavedInstanceState");
			}
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			if (savedInstanceState != null)
			{
				Header = savedInstanceState.GetString("Header");
				OutOfBandTransactionType = savedInstanceState.GetString("OutOfBandTransactionType");
				CanUseAtmLastEight = savedInstanceState.GetBoolean("CanUseAtmLastEight");
				var json = savedInstanceState.GetString("GetAccountVerificationOptionsResponse");
				_getAccountVerificationOptionsResponse = JsonConvert.DeserializeObject<GetAccountVerificationOptionsResponse>(json);
			}

            var txtQuestion = Activity.FindViewById<TextView>(Resource.Id.txtQuestion);
            CultureTextProvider.SetMobileResourceText(txtQuestion, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "EDA7EBCC-4366-4E6B-A21C-98FDDCDB4DA8", "To help protect your account, we want to make sure this is you. How should we verify your account?");

			spinnerValidationMethod = Activity.FindViewById<Spinner>(Resource.Id.spinnerValidationMethod);
			spinnerValidationMethod.Prompt = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "A97853A3-4A4C-468D-9703-F929F7973FB8", "Select Validation Method");

            _last8Text = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "94B16716-0470-4D0A-ADB7-5A9F75B83A59", "Last 8 digits of ATM or Debit Card");

			txtAnswer = Activity.FindViewById<EditText>(Resource.Id.txtAnswer);
			txtAnswer.AfterTextChanged += (sender, e) =>
			{
				btnContinue.Enabled = !string.IsNullOrWhiteSpace(((TextView)sender).Text);
			};

			btnSendCode = Activity.FindViewById<Button>(Resource.Id.btnSendCode);
			btnSendCode.Click += (sender, e) => SendVerificationCode();

			btnContinue = Activity.FindViewById<Button>(Resource.Id.btnContinue);
			btnContinue.Click += (sender, e) => Submit();
			btnContinue.Enabled = false;

			CultureTextProvider.SetMobileResourceText(btnSendCode, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "59E9F9A7-5946-422C-8063-29060A266AEA");
			CultureTextProvider.SetMobileResourceText(btnContinue, "f37ac18a-0550-49dc-82ad-101ffea9bfad", "7DAA9ECD-0758-4121-935A-949A9103C076");

			if (savedInstanceState != null)
			{
				GetAccountVerificationOptions(savedInstanceState.GetString("Answer"), savedInstanceState.GetInt("SpinnerSelection"));
			}
			else
			{
				GetAccountVerificationOptions(string.Empty, -1);
			}
		}

		private async void GetAccountVerificationOptions(string answer, int selection)
		{
			try
			{
				if (_getAccountVerificationOptionsResponse == null)
				{
					ShowActivityIndicator();
					var methods = new AuthenticationMethods();
					_getAccountVerificationOptionsResponse = await methods.GetAccountVerificationOptions(null, Activity);
					HideActivityIndicator();
				}

				var items = new List<string>();

				if (_getAccountVerificationOptionsResponse != null && _getAccountVerificationOptionsResponse.Success)
				{
					foreach (var notificationOption in _getAccountVerificationOptionsResponse.NotificationOptions)
					{
						items.Add(notificationOption.Destination);
					}

					if (_getAccountVerificationOptionsResponse.HasCreditCard && CanUseAtmLastEight)
					{
						items.Add(_last8Text);
					}
				}

				var adapter = new ArrayAdapter<string>(Activity, Resource.Layout.support_simple_spinner_dropdown_item, items);
				spinnerValidationMethod.Adapter = adapter;

				if (selection > -1)
				{
					spinnerValidationMethod.SetSelection(selection);
				}

				spinnerValidationMethod.ItemSelected += (sender, e) =>
				{
					var text = spinnerValidationMethod.SelectedItem.ToString();
					SpinnerChanged(text);
				};

				// Delay the setting of txtAnswer until after SpinnerChanged.
				var updateAnswer = new Handler();
				updateAnswer.PostDelayed(() =>
			   {
				   txtAnswer.Text = answer;
				   txtAnswer.SetSelection(txtAnswer.Text.Length);
			   }, 200);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "AccountVerificationFragment:GetAccountVerificationOptions");
			}
		}

		private void SpinnerChanged(string text)
		{
			if (text.Equals(_last8Text))
			{
				btnSendCode.Enabled = false;
				btnSendCode.Visibility = ViewStates.Invisible;
				txtAnswer.Hint = "xxxxxxxx";
			}
			else
			{
				btnSendCode.Enabled = true;
				btnSendCode.Visibility = ViewStates.Visible;
				txtAnswer.Hint = CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "4FC2EDEC-D50D-43EA-8F91-47B679290054", "Code");
			}

			txtAnswer.Text = string.Empty;
			btnContinue.Enabled = false;
		}

		private async void SendVerificationCode()
		{
			try
			{
				var request = new SendOutOfBandCodeRequest
				{
					TransactionType = OutOfBandTransactionType,
					OutOfBandMessageType = spinnerValidationMethod.SelectedItem.ToString().Substring(0, spinnerValidationMethod.SelectedItem.ToString().IndexOf(" ", StringComparison.Ordinal)),
					Payload = RetainedSettings.Instance.Payload
				};

				ShowActivityIndicator();

				var methods = new AuthenticationMethods();
				await methods.SendOutOfBandCode(request, Activity);

				HideActivityIndicator();

				await AlertMethods.Alert(Activity, "SunMobile", CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "0746A82B-4ED1-4013-8CBF-BE92E3A2DAE2", "Verification code sent."), "OK");
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "AccountVerificationFragment:SendVerficiationCode");
			}
		}

		private async void Submit()
		{
			var request = new VerifyOutOfBandCodeRequest
			{
				TransactionType = OutOfBandTransactionType,
				Payload = RetainedSettings.Instance.Payload
			};

			if (spinnerValidationMethod.SelectedItem.ToString().StartsWith("Email", StringComparison.Ordinal))
			{
				request.Code = txtAnswer.Text;
			}
			else if (spinnerValidationMethod.SelectedItem.ToString().StartsWith("Text", StringComparison.Ordinal))
			{
				request.Code = txtAnswer.Text;
			}
			else
			{
				request.LastEight = txtAnswer.Text;
			}

			ShowActivityIndicator();

			var methods = new AuthenticationMethods();
			var response = await methods.VerifyOutOfBandCode(request, Activity);

			HideActivityIndicator();

			if (response?.Result?.VerificationState != null && response.Result.VerificationState == "Passed")
			{
				Completed(true);
			}
			else
			{
				await AlertMethods.Alert(Activity, "SunMobile", response?.FailureMessage ?? CultureTextProvider.GetMobileResourceText("f37ac18a-0550-49dc-82ad-101ffea9bfad", "32D27C0C-106F-43D1-95D0-6F52ED68ADB4", "Verification failed."), "OK");
				Logging.Track("Verification Events", "Failed verification.", request.Code);
			}
		}
	}
}