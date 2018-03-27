using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SunBlock.DataTransferObjects.Authentication;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.Mobile.Model.Authentication;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Biometrics;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.InAuth;
using SunMobile.Shared.Utilities.Settings;

using SunMobile.Forms.Views;
//using Xamarin.Forms.Platform.Android;

namespace SunMobile.Droid.Authentication
{
	[Activity(Label = "AdaptiveAuthenticationFragment")]
	public class AdaptiveAuthenticationFragment : BaseAuthenticationFragment
	{
		private EditText txtMemberId;
		private EditText txtPin;
		private TextView lblRememberMemberId;
		private TextView lblEnableTouchId;
		private Switch switchRememberMemberId;
		private Switch switchTouchId;
		private TableRow rowFingerprint;
		private TableRow rowTest;
		private Button btnSubmit;
        private ImageView imageUseBiometrics;
        private Fragment contentPageView;

		AuthenticateHostViewModel _authenticateHostResponse;
		AuthenticateFingerprintViewModel _authenticateFingerprintResponse;
		MainActivity _mainActivity;
        private BiometryType _biometryType;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			base.OnCreateView(inflater, container, savedInstanceState);
			var view = inflater.Inflate(Resource.Layout.SunBlockAnalyzeView, null);
			RetainInstance = true;

			return view;
		}

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			SetupView();			
		}

		public override async void SetupView()
		{
			_mainActivity = (MainActivity)Activity;
			txtMemberId = Activity.FindViewById<EditText>(Resource.Id.txtMemberId);
			txtPin = Activity.FindViewById<EditText>(Resource.Id.txtPin);
			switchRememberMemberId = Activity.FindViewById<Switch>(Resource.Id.btnToggleUN);
			switchTouchId = Activity.FindViewById<Switch>(Resource.Id.switchTouchId);
			lblRememberMemberId = Activity.FindViewById<TextView>(Resource.Id.lblRememberMemberId);
			lblEnableTouchId = Activity.FindViewById<TextView>(Resource.Id.lblEnableTouchId);
			switchTouchId.CheckedChange += EnableTouchIdChanged;
			rowFingerprint = Activity.FindViewById<TableRow>(Resource.Id.RowFingerprint);
			rowTest = Activity.FindViewById<TableRow>(Resource.Id.TestRow);

			btnSubmit = Activity.FindViewById<Button>(Resource.Id.btnSubmit);
			btnSubmit.Click += (sender, e) => Analyze();

            imageUseBiometrics = Activity.FindViewById<ImageView>(Resource.Id.imageUseBiometrics);
            imageUseBiometrics.Click += (sender, e) => LoginUsingBiometrics();

			var remember = RetainedSettings.Instance.RememberMemberId;
			var touchIdRegistered = RetainedSettings.Instance.IsTouchIdRegistered;

			if (remember != null && remember == "true")
			{
				switchRememberMemberId.Checked = true;
				txtMemberId.Text = RetainedSettings.Instance.MemberId;
				txtPin.RequestFocus();
			}
			else
			{
				switchRememberMemberId.Checked = false;
				txtMemberId.RequestFocus();
			}

			switchTouchId.Checked = touchIdRegistered == "true";

            _biometryType = await BiometricUtilities.HasBiometricReader();
            imageUseBiometrics.Visibility = ViewStates.Gone;

            if (_biometryType == BiometryType.Fingerprint)
			{
				rowFingerprint.Visibility = ViewStates.Visible;
			}
			else
			{
				rowFingerprint.Visibility = ViewStates.Gone;
			}

			if (touchIdRegistered == "true")
			{
                imageUseBiometrics.Visibility = ViewStates.Visible;

                if (!SessionSettings.Instance.HasSignedOutOrTimedOut)
                {
                    LoginUsingBiometrics();
                }
			}
			else
			{
				GeneralUtilities.ShowKeyboard(Activity);
			}

            #if DEBUG
			CreateTestLoginButton();
            CreateTestContentpage();
            #endif
		}

		public override void SetCultureConfiguration()
		{
			CultureTextProvider.SetMobileResourceText(txtMemberId, "949A3C83-C4A9-45BF-9341-C38AD698E253", "AA7AC99B-7F01-4267-BF45-2FE61B80EA7B");
			CultureTextProvider.SetMobileResourceText(txtPin, "949A3C83-C4A9-45BF-9341-C38AD698E253", "F8B414AA-7A41-4D25-AF8A-0B7F2DD0A662");
			CultureTextProvider.SetMobileResourceText(lblRememberMemberId, "949A3C83-C4A9-45BF-9341-C38AD698E253", "56835EDB-2C55-4B60-99C1-BC3774DEE7C2");
			CultureTextProvider.SetMobileResourceText(lblEnableTouchId, "949A3C83-C4A9-45BF-9341-C38AD698E253", "587FB059-3769-43EC-8720-DA0FB55C70DA");
            btnSubmit.Text = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8", "Login");			
		}

		private void EnableTouchIdChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (!e.IsChecked)
			{
                DisableBiometricAuthentication();
			}
		}

		private async void Analyze()
		{
			// Check to see if it is a new user id and if it is, turn touch id enrollment off
			if (RetainedSettings.Instance.LastAuthenticatedMemberId != txtMemberId.Text.PadLeft(10, '0'))
			{
                DisableBiometricAuthentication(false);
			}

			var touchIdRegistered = RetainedSettings.Instance.IsTouchIdRegistered;

			if (touchIdRegistered == "true" && SessionSettings.Instance.HasSignedOutOrTimedOut)
			{
				LoginUsingBiometrics();
			}
			else
			{
				var methods = new AuthenticationMethods();

				var label = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8", "Login");
				var message = methods.ValidateLoginRequest(txtMemberId.Text, txtPin.Text);

				if (message != string.Empty)
				{
					await AlertMethods.Alert(Activity, label, message, "OK");
				}
				else
				{
					RetainedSettings.Instance.RememberMemberId = switchRememberMemberId.Checked ? "true" : string.Empty;
					RetainedSettings.Instance.MemberId = txtMemberId.Text;

					Login(txtMemberId.Text.Trim(), txtPin.Text.Trim());
				}
			}
		}

		public async Task<AnalyzeViewModel> Analyze(string username, bool forceInAuthUnregister)
		{
			var analyzeResponse = new AnalyzeViewModel();

			try
			{
				ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "6B54CF6A-52F3-4B9D-B365-DB79A9D4A7C0", "Logging in..."));

				var payload = await InAuthService.GetPayloadForLogin(((MainActivity)Activity).Application, Activity, ((MainActivity)Activity).Assets, forceInAuthUnregister);
				var analyzeRequest = new AnalyzeWithPayloadRequest { UserId = username, Payload = payload, Language = SessionSettings.Instance.Language.ToString() };
				var methods = new AuthenticationMethods();
				analyzeResponse = await methods.GenerateTokenAndLogs(analyzeRequest, Activity);

				HideActivityIndicator();
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "AdaptiveAuthenticationFragment:Analyze");
			}

			return analyzeResponse;
		}

		public async void Login(string username, string password)
		{
			try
			{
				GeneralUtilities.CloseKeyboard(Activity);

				username = username.PadLeft(10, '0');

				var analyzeResponse = await Analyze(username, false);

				if (analyzeResponse?.ClientViewState != null && analyzeResponse.ClientViewState == "AuthenticateHost")
				{
					if (analyzeResponse.LogTransactionId == null)
					{
						Logging.Track("Authentication Events", "Failed authentication logging.", txtMemberId.Text);

						// If this fails, lets try to unregister / register inauth.
						analyzeResponse = await Analyze(username, true);
					}
				}

				if (analyzeResponse?.ClientViewState != null && analyzeResponse.ClientViewState == "AuthenticateHost")
				{
					if (analyzeResponse.LogTransactionId == null)
					{
						Logging.Track("Authentication Events", "Failed authentication logging.", txtMemberId.Text);
					}
					else
					{
						Logging.Track("Authentication Events", "Started authentication logging", txtMemberId.Text);
					}

					if (analyzeResponse?.AnalyzeResponse != null && !string.IsNullOrEmpty(analyzeResponse.AnalyzeResponse.Token))
					{
						SessionSettings.Instance.SunBlockToken = analyzeResponse.AnalyzeResponse.Token;
					}
					else
					{
						Logging.Track("Authentication Events", "Token is null", $"ClientViewState: {analyzeResponse.ClientViewState}");
					}

					var authenticateHostRequest = new MobileAuthenticateHostRequest
					{
						Password = password,
						Version = GeneralUtilities.GetAppVersion(Activity),
						DeviceId = GeneralUtilities.GetDeviceId(),
						Payload = RetainedSettings.Instance.Payload
					};

					ShowActivityIndicator(CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "6B54CF6A-52F3-4B9D-B365-DB79A9D4A7C0", "Logging in..."));

					var methods = new AuthenticationMethods();
					_authenticateHostResponse = await methods.AuthenticateHost(authenticateHostRequest, Activity);

					HideActivityIndicator();

					if (_authenticateHostResponse?.ClientViewState != null && _authenticateHostResponse.ClientViewState == "Authenticated")
					{
						OnAuthenticated(username);
					}
					else if (_authenticateHostResponse?.ClientViewState != null && _authenticateHostResponse.ClientViewState == "Challenge")
					{
						var accountVerificationFragment = new AccountVerificationFragment();
						accountVerificationFragment.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
						accountVerificationFragment.OutOfBandTransactionType = OutOfBandTransactionTypes.Login.ToString();
						accountVerificationFragment.CanUseAtmLastEight = _authenticateHostResponse.CanUseAtmLastEight;
						accountVerificationFragment.Completed += (isValidated) =>
						{
							OnAuthenticated(username);
						};

						Activity.SupportFragmentManager.BeginTransaction()
							.Replace(Resource.Id.content_frame, accountVerificationFragment)
							.Commit();
					}
					else
					{

						Logging.Track("Authentication Events", "Failed Login", string.Format("{0} - {1}", txtMemberId.Text, _authenticateHostResponse?.ClientMessage ?? "Error logging in."));
						((MainActivity)Activity).ProcessViewControllerState(_authenticateHostResponse.ClientViewState, _authenticateHostResponse?.ClientMessage ??
                        CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."), _authenticateHostResponse.MobileLoginResponse);
					}
				}
				else
				{
					var loginErrorStr = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "3A6DADC3-F72B-4C55-A91B-EFAF7AB7A1A9", "Login Error");
					await AlertMethods.Alert(Activity, loginErrorStr, analyzeResponse?.ClientMessage ?? CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."), "OK");
				}
			}
			catch (Exception ex)
			{
				Logging.Track("Authentication Events", "Login Exception", $"Message: {ex.Message}, Stack Track {ex.StackTrace ?? string.Empty}");
				Logging.Log(ex, "AdaptiveAuthenticationFragment:Login");
			}
		}

		private void OnAuthenticated(string username)
		{
			SessionSettings.Instance.UserId = username.PadLeft(10, '0');
			SessionSettings.Instance.IsAuthenticated = true;

			if (switchTouchId.Checked && SessionSettings.Instance.UserId != RetainedSettings.Instance.TouchIdRegisteredMemberId)
			{
				BiometricsEnrollment();
			}
			else
			{
				if (!switchTouchId.Checked || SessionSettings.Instance.UserId != RetainedSettings.Instance.TouchIdRegisteredMemberId)
				{
                    DisableBiometricAuthentication();
				}

                _mainActivity.ProcessViewControllerState("Authenticated", _authenticateHostResponse.ClientMessage, _authenticateHostResponse.MobileLoginResponse);
			}
		}

		public async void LoginWithFingerprint(string username, string password)
		{
			try
			{
				GeneralUtilities.CloseKeyboard(View);

				username = username.PadLeft(10, '0');

				var analyzeResponse = await Analyze(username, false);

				if (analyzeResponse?.ClientViewState != null && analyzeResponse.ClientViewState == "AuthenticateHost")
				{
					if (analyzeResponse.LogTransactionId == null)
					{
						Logging.Track("Authentication Events", "Failed authentication logging.", txtMemberId.Text);

						//If this fails, lets try to unregister / register inauth.
						analyzeResponse = await Analyze(username, true);
					}
				}

				if (analyzeResponse?.ClientViewState != null && analyzeResponse.ClientViewState == "AuthenticateHost")
				{
					if (analyzeResponse.LogTransactionId == null)
					{
						Logging.Track("Authentication Events", "Failed authentication logging.", txtMemberId.Text);
					}
					else
					{
						Logging.Track("Authentication Events", "Started authentication logging", txtMemberId.Text);
					}

					if (analyzeResponse?.AnalyzeResponse != null && !string.IsNullOrEmpty(analyzeResponse.AnalyzeResponse.Token))
					{
						SessionSettings.Instance.SunBlockToken = analyzeResponse.AnalyzeResponse.Token;
					}
					else
					{
						Logging.Track("Authentication Events", "Token is null", $"ClientViewState: {analyzeResponse.ClientViewState}");
					}

					ShowActivityIndicator();

					var authenticateFingerprintRequest = new AuthenticateFingerprintRequest
					{
                        Version = GeneralUtilities.GetAppVersion(Activity),
						FingerprintAuthorizationCode = password,
						Payload = RetainedSettings.Instance.Payload
					};

					var methods = new AuthenticationMethods();
					_authenticateFingerprintResponse = await methods.AuthenticateFingerprint(authenticateFingerprintRequest, Activity);

					HideActivityIndicator();

					if (_authenticateFingerprintResponse?.ClientViewState != null && _authenticateFingerprintResponse?.ClientViewState == "Authenticated")
					{
						OnFingerprintAuthenticated(username);
					}
					else if (_authenticateFingerprintResponse?.ClientViewState != null && _authenticateFingerprintResponse.ClientViewState == "Challenge")
					{
						var accountVerificationFragment = new AccountVerificationFragment(); accountVerificationFragment.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
						accountVerificationFragment.OutOfBandTransactionType = OutOfBandTransactionTypes.Login.ToString();
						accountVerificationFragment.CanUseAtmLastEight = _authenticateFingerprintResponse.CanUseAtmLastEight;
						accountVerificationFragment.Completed += (isValidated) =>
						{
							OnFingerprintAuthenticated(username);
						};

						Activity.SupportFragmentManager.BeginTransaction()
							.Replace(Resource.Id.content_frame, accountVerificationFragment)
							.Commit();
					}
					else
					{
						var loginErrorStr = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "3A6DADC3-F72B-4C55-A91B-EFAF7AB7A1A9", "Login Error");

						if (_authenticateFingerprintResponse?.ClientMessage != null)
						{
							await AlertMethods.Alert(Activity, loginErrorStr, _authenticateFingerprintResponse.ClientMessage, "OK");

							// for some reason the entire phrase is not a match (could have to do with spaces) so we just search for a couple key words
							if (_authenticateFingerprintResponse.ClientMessage.Contains("password") && _authenticateFingerprintResponse.ClientMessage.Contains("changed"))
							{
                                DisableBiometricAuthentication();
							}
						}
						else
						{
							await AlertMethods.Alert(Activity, loginErrorStr, CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."), "OK");
						}
					}
				}
				else
				{
					var loginErrorStr = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "3A6DADC3-F72B-4C55-A91B-EFAF7AB7A1A9", "Login Error");

					if (analyzeResponse?.ClientMessage != null)
					{
						await AlertMethods.Alert(Activity, loginErrorStr, analyzeResponse.ClientMessage, "OK");
					}
					else
					{
						await AlertMethods.Alert(Activity, loginErrorStr, CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."), "OK");
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Track("Authentication Events", "Login Exception", $"Message: {ex.Message}, Stack Trace: {ex.StackTrace ?? string.Empty}");
				Logging.Log(ex, "AdaptiveAuthenticationViewController:LoginWithFingerprint");
			}
		}

		private void OnFingerprintAuthenticated(string username)
		{
			SessionSettings.Instance.UserId = username.PadLeft(10, '0');
			SessionSettings.Instance.IsAuthenticated = true;

            _mainActivity.ProcessViewControllerState("Authenticated", _authenticateFingerprintResponse.ClientMessage, _authenticateFingerprintResponse.MobileLoginResponse);
		}

        private async void BiometricsEnrollment()
		{
            try
            {
                string touchIdAgreement = string.Empty;

                var request = new BiometricInformationRequest
                {
                    DeviceType = "Android",
                    BiometricType = BiometryType.Fingerprint.ToString()
                };

                ShowActivityIndicator();

                var methods = new AuthenticationMethods();
                var response = await methods.GetBiometricInformation(request, Activity);

                HideActivityIndicator();

                if (response != null && !string.IsNullOrEmpty(response.AgreementText))
                {
                    touchIdAgreement = response.AgreementText.Replace("\\n", "\n");
                }

                var fingerprintTitle = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "B3BDA3F5-8539-42BB-9F93-3643F90C2319", "SunMobile Authentication Agreement");
                var acceptBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "4D271CAB-66A9-4A57-96EA-F9B6BFD27DD9", "Accept");
                var emailBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "ED7A98CE-7350-4805-9F4F-4AE807E1F072", "Email Agreement");
                var declineBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "5D536C6D-AEF6-43EC-86FC-9D276EA7BEDD", "Decline");

                var agreementResponse = await AlertMethods.Alert(Activity, fingerprintTitle, touchIdAgreement, acceptBtnText, emailBtnText, declineBtnText);

                if (agreementResponse.Equals(acceptBtnText))
                {
                    AcceptEnrollment();
                }
                else if (agreementResponse.Equals(emailBtnText))
                {
                    GeneralUtilities.SendEmail(Activity, null, fingerprintTitle, touchIdAgreement, true);
                }
                else if (agreementResponse.Equals(declineBtnText))
                {
                    ((MainActivity)Activity).ProcessViewControllerState(_authenticateHostResponse.ClientViewState, _authenticateHostResponse.ClientMessage, _authenticateHostResponse.MobileLoginResponse);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(ex, "AdaptiveAuthenticationFragment:BiometricsEnrollment");
            }
		}

		private async void AcceptEnrollment()
		{
			var messageText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "5DDCF791-E4DD-40C6-93B7-EBFDE461988A", "Touch your finger to the home button to match a fingerprint to your Member Number {0}.");
			messageText = string.Format(messageText, SessionSettings.Instance.UserId);			
			var cancelBtnTxt = CultureTextProvider.GetMobileResourceText("49A3C83-C4A9-45BF-9341-C38AD698E253", "89F7EB6D-174A-450B-9871-6C4A43820A72", "Cancel");
			var fallbackBtnTxt = CultureTextProvider.GetMobileResourceText("49A3C83-C4A9-45BF-9341-C38AD698E253", "7DD27E20-FD9C-4210-8340-4B9C5FA1E89F", "Use Fallback");

            if (await BiometricUtilities.AuthenticateFingerprint(messageText, cancelBtnTxt, fallbackBtnTxt))
			{
				EnrollFingerprint();
			}
		}

		private async void EnrollFingerprint()
		{
			try
			{
				ShowActivityIndicator();

				var methods = new AuthenticationMethods();
				var response = await methods.EnrollFingerprintAuthorization(null, Activity);

				HideActivityIndicator();

				if (response != null && response.Success)
				{
					RetainedSettings.Instance.IsTouchIdRegistered = "true";
					RetainedSettings.Instance.TouchIdRegisteredMemberId = SessionSettings.Instance.UserId;
					RetainedSettings.Instance.TouchIdRegisteredPin = response.FingerprintAuthorizationCode;
                    ((MainActivity)Activity).ProcessViewControllerState(_authenticateHostResponse.ClientViewState, _authenticateHostResponse.ClientMessage, _authenticateHostResponse.MobileLoginResponse);
				}
				else
				{
					await AlertMethods.Alert(Activity, "SunMobile", CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "B78A37DC-F661-4BD1-A601-2679A409ED8E", "Error while enrolling fingerprint authentication."), "OK");
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "AdaptiveAuthenticationViewFragment:EnrollFingerprint");
			}
		}

		private async void LoginUsingBiometrics()
		{
			var messageText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "E02F4E28-4C38-4617-9B06-C5A74F0EDC95", "Touch your finger to logon as {0}.");
			messageText = string.Format(messageText, RetainedSettings.Instance.TouchIdRegisteredMemberId);			
			var cancelBtnTxt = CultureTextProvider.GetMobileResourceText("49A3C83-C4A9-45BF-9341-C38AD698E253", "89F7EB6D-174A-450B-9871-6C4A43820A72", "Cancel");
			var fallbackBtnTxt = CultureTextProvider.GetMobileResourceText("49A3C83-C4A9-45BF-9341-C38AD698E253", "7DD27E20-FD9C-4210-8340-4B9C5FA1E89F", "Use Fallback");

            if (await BiometricUtilities.AuthenticateFingerprint(messageText, cancelBtnTxt, fallbackBtnTxt))
			{
				LoginWithFingerprint(RetainedSettings.Instance.TouchIdRegisteredMemberId, RetainedSettings.Instance.TouchIdRegisteredPin);
			}
		}

        private void DisableBiometricAuthentication(bool turnOffTouchIdSwitch = true)
		{
			if (turnOffTouchIdSwitch)
			{
				switchTouchId.Checked = false;
			}

            imageUseBiometrics.Visibility = ViewStates.Gone;
			RetainedSettings.Instance.IsTouchIdRegistered = "false";
			RetainedSettings.Instance.TouchIdRegisteredMemberId = string.Empty;
			RetainedSettings.Instance.TouchIdRegisteredPin = string.Empty;
		}

        #if DEBUG
		private void CreateTestLoginButton()
		{
			var testButton = new Button(Activity);
			testButton.Text = "Test Login";

			var layoutParams = new TableRow.LayoutParams
			{
				Column = 2,
				Span = 1
			};

			testButton.Click += (sender, e) =>
			{
				Login("4022018", "Q@111222");
			};

			rowTest.AddView(testButton, layoutParams);
		}

        private void CreateTestContentpage()
        {
            var testButton = new Button(Activity);
            testButton.Text = "Test Contact page";

            var layoutParams = new TableRow.LayoutParams
            {
                Column = 2,
                Span = 2
            };

            testButton.Click += (sender, e) =>
            {
                ((MainActivity)Activity).ShowCOntentPage();
              
            };

            rowTest.AddView(testButton, layoutParams);
        }

       
        #endif
	}
}