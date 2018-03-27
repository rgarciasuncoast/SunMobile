using System;
using Foundation;
using LocalAuthentication;
using SunBlock.DataTransferObjects.Authentication;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.Culture;

using SunMobile.iOS.Common;
using SunMobile.Shared.Biometrics;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.InAuth;
using SunMobile.Shared.Utilities.Settings;
using UIKit;

namespace SunMobile.iOS.Authentication
{
	public partial class AdaptiveAuthenticationViewController : BaseAuthenticationViewController, ICultureConfigurationProvider
	{
        public event Action<string, MobileLoginResponse> Authenticated = delegate { };
        private MobileLoginResponse _mobileLoginResponse = null;
        private BiometryType _biometryType;

		public AdaptiveAuthenticationViewController(IntPtr handle) : base(handle)
		{
		}	

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            switchTouchId.AccessibilityLabel = "Enable Touch ID";
            switchRememberMemberId.AccessibilityLabel = "Remember Member Number";

			txtMemberId.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 10;
			};

			txtMemberId.ShouldReturn += TextFieldShouldReturn;

			txtPin.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				var newLength = textField.Text.Length + replacementString.Length - range.Length;
				return newLength <= 16;
			};

			txtPin.ShouldReturn += TextFieldShouldReturn;

			switchTouchId.ValueChanged += EnableTouchIdChanged;

			var remember = RetainedSettings.Instance.RememberMemberId;
			var touchIdRegistered = RetainedSettings.Instance.IsTouchIdRegistered;

			if (remember != null && remember == "true")
			{
				switchRememberMemberId.On = true;
				txtMemberId.Text = RetainedSettings.Instance.MemberId;
				txtPin.BecomeFirstResponder();
			}
			else
			{
				txtMemberId.BecomeFirstResponder();
				switchRememberMemberId.On = false;
			}

			switchTouchId.On = touchIdRegistered == "true";

            _biometryType = BiometricUtilities.HasBiometricReader();
            imageUseBiometrics.Hidden = true;

            var tapGesture = new UITapGestureRecognizer();
            tapGesture.AddTarget(() => LoginUsingBiometrics());
            imageUseBiometrics.AddGestureRecognizer(tapGesture);

            if (_biometryType != BiometryType.None)
			{
				labelTouchId.Hidden = false;
				switchTouchId.Hidden = false;

                switch (_biometryType)
                {
                    case BiometryType.Fingerprint:
                        labelTouchId.Text = "Enable Touch ID:";
                        imageUseBiometrics.Image = UIImage.FromBundle("biometrics_touchid");
                        break;
                    case BiometryType.Face:
                        labelTouchId.Text = "Enable Face ID:";
                        imageUseBiometrics.Image = UIImage.FromBundle("biometrics_faceid");
                        break;
                    case BiometryType.None:
                        break;
                }
			}
			else
			{
				labelTouchId.Hidden = true;
				switchTouchId.Hidden = true;
			}

			btnLogin.TouchUpInside += (sender, e) => Analyze();
			AppStyles.SetViewBorder(btnLogin, true);

            if (touchIdRegistered == "true")
            {
                imageUseBiometrics.Hidden = false;

                if (!SessionSettings.Instance.HasSignedOutOrTimedOut)
                {
                    LoginUsingBiometrics();
                }
            }			

            #if DEBUG
			CreateTestLoginButton();
            CreateTestContactUsButton();
            #endif
		}

		public override void SetCultureConfiguration()
		{
			CultureTextProvider.SetMobileResourceText(txtMemberId, "949A3C83-C4A9-45BF-9341-C38AD698E253", "AA7AC99B-7F01-4267-BF45-2FE61B80EA7B");
			CultureTextProvider.SetMobileResourceText(txtPin, "949A3C83-C4A9-45BF-9341-C38AD698E253", "F8B414AA-7A41-4D25-AF8A-0B7F2DD0A662");
			CultureTextProvider.SetMobileResourceText(lblRememberMemberId, "949A3C83-C4A9-45BF-9341-C38AD698E253", "56835EDB-2C55-4B60-99C1-BC3774DEE7C2");
            CultureTextProvider.SetMobileResourceText(labelTouchId, "949A3C83-C4A9-45BF-9341-C38AD698E253", "587FB059-3769-43EC-8720-DA0FB55C70DA");
			CultureTextProvider.SetMobileResourceText(btnLogin, "949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8");
		}

		private void EnableTouchIdChanged(object sender, EventArgs e)
		{
			if (!switchTouchId.On)
			{
				DisableBiometricAuthentication();
			}
		}

		private bool TextFieldShouldReturn(UITextField textfield)
		{
			if (textfield == txtMemberId)
			{
				txtPin.BecomeFirstResponder();
			}

			if (textfield == txtPin)
			{
				Analyze();
			}

			return false;
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			GeneralUtilities.CloseKeyboard(View);
		}

		private async void Analyze()
		{
			GeneralUtilities.CloseKeyboard(View);

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
					await AlertMethods.Alert(View, label, message, "OK");
				}
				else
				{
					RetainedSettings.Instance.RememberMemberId = switchRememberMemberId.On ? "true" : string.Empty;
					RetainedSettings.Instance.MemberId = txtMemberId.Text;

					Login(txtMemberId.Text.Trim(), txtPin.Text.Trim());
				}
			}
		}

		public async void Login(string username, string password)
		{
			try
			{
				GeneralUtilities.CloseKeyboard(View);

				username = username.PadLeft(10, '0');

				ShowActivityIndicator();

				var payload = await InAuthService.GetPayloadForLogin();
				var analyzeRequest = new AnalyzeWithPayloadRequest { UserId = username, Payload = payload, Language = SessionSettings.Instance.Language.ToString() };
				var methods = new AuthenticationMethods();
				var analyzeResponse = await methods.GenerateTokenAndLogs(analyzeRequest, View);

				HideActivityIndicator();

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

					var authenticateHostRequest = new MobileAuthenticateHostRequest
					{
						Password = password,
						Version = GeneralUtilities.GetAppVersion(),
						DeviceId = GeneralUtilities.GetDeviceId(),
						Payload = RetainedSettings.Instance.Payload
					};

					var authenticateHostResponse = await methods.AuthenticateHost(authenticateHostRequest, null);

					HideActivityIndicator();

					if (authenticateHostResponse?.ClientViewState != null && authenticateHostResponse.ClientViewState == "Authenticated")
					{
                        _mobileLoginResponse = authenticateHostResponse.MobileLoginResponse;
						OnAuthenticated(username);
					}
					else if (authenticateHostResponse?.ClientViewState != null && authenticateHostResponse.ClientViewState == "Challenge")
					{
                        _mobileLoginResponse = authenticateHostResponse.MobileLoginResponse;
						var accountVerificationViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountVerificationViewController") as AccountVerificationViewController;
						accountVerificationViewController.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
						accountVerificationViewController.OutOfBandTransactionType = OutOfBandTransactionTypes.Login.ToString();
						accountVerificationViewController.CanUseAtmLastEight = authenticateHostResponse.CanUseAtmLastEight;
						accountVerificationViewController.Completed += (isValidated) =>
						{
							OnAuthenticated(username);
						};

						NavigationController.PushViewController(accountVerificationViewController, true);
					}
					else
					{
						Logging.Track("Authentication Events", "Failed Login", string.Format("{0} - {1}", txtMemberId.Text, authenticateHostResponse?.ClientMessage ?? "Error logging in."));
						CancelLogin(authenticateHostResponse?.ClientMessage ?? CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."));
					}
				}
				else
				{
					CancelLogin(analyzeResponse?.ClientMessage ?? CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."));
				}
			}
			catch (Exception ex)
			{
				Logging.Track("Authentication Events", "Login Exception", $"Message: {ex.Message}, Stack Track {ex.StackTrace ?? string.Empty}");
				Logging.Log(ex, "AdaptiveAuthenticationViewController:Login");
			}
		}

		private void OnAuthenticated(string username)
		{
			SessionSettings.Instance.UserId = username.PadLeft(10, '0');
			SessionSettings.Instance.IsAuthenticated = true;

			if (switchTouchId.On && SessionSettings.Instance.UserId != RetainedSettings.Instance.TouchIdRegisteredMemberId)
			{
                BiometricsEnrollment();
			}
			else
			{
				if (!switchTouchId.On || SessionSettings.Instance.UserId != RetainedSettings.Instance.TouchIdRegisteredMemberId)
				{
					DisableBiometricAuthentication();
				}

				NavigationController.PopToRootViewController(false);
                Authenticated("authenticated", _mobileLoginResponse);
			}
		}

		public async void LoginWithBiometrics(string username, string password)
		{
			try
			{
				GeneralUtilities.CloseKeyboard(View);

				ShowActivityIndicator();

				var payload = await InAuthService.GetPayloadForLogin();
				var analyzeRequest = new AnalyzeWithPayloadRequest { UserId = username, Payload = payload, Language = SessionSettings.Instance.Language.ToString() };
				var methods = new AuthenticationMethods();
				var analyzeResponse = await methods.GenerateTokenAndLogs(analyzeRequest, View);

				HideActivityIndicator();

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
                        Version = GeneralUtilities.GetAppVersion(),
						FingerprintAuthorizationCode = password,
						Payload = RetainedSettings.Instance.Payload
					};

					var authenticateFingerprintResponse = await methods.AuthenticateFingerprint(authenticateFingerprintRequest, null);

					HideActivityIndicator();

					if (authenticateFingerprintResponse?.ClientViewState != null && authenticateFingerprintResponse.ClientViewState == "Authenticated")
					{
                        _mobileLoginResponse = authenticateFingerprintResponse.MobileLoginResponse;
                        OnBiometricAuthenticated(username);
					}
					else if (authenticateFingerprintResponse?.ClientViewState != null && authenticateFingerprintResponse.ClientViewState == "Challenge")
					{
                        _mobileLoginResponse = authenticateFingerprintResponse.MobileLoginResponse;
						var accountVerificationViewController = AppDelegate.StoryBoard.InstantiateViewController("AccountVerificationViewController") as AccountVerificationViewController;
						accountVerificationViewController.Header = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "14D29C9D-B918-4864-9006-CD0B3F7FA7A7", "Account Verification");
						accountVerificationViewController.OutOfBandTransactionType = OutOfBandTransactionTypes.Login.ToString();
						accountVerificationViewController.CanUseAtmLastEight = authenticateFingerprintResponse.CanUseAtmLastEight;
						accountVerificationViewController.Completed += (isValidated) =>
						{
                            OnBiometricAuthenticated(username);
						};

						NavigationController.PushViewController(accountVerificationViewController, true);
					}
					else
					{
						if (authenticateFingerprintResponse?.ClientMessage != null)
						{
							CancelLogin(authenticateFingerprintResponse.ClientMessage);

							// for some reason the entire phrase is not a match (could have to do with spaces) so we just search for a couple key words
							if (authenticateFingerprintResponse.ClientMessage.Contains("password") && authenticateFingerprintResponse.ClientMessage.Contains("changed"))
							{
								DisableBiometricAuthentication();
							}
						}
						else
						{
							CancelLogin(CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."));
						}
					}
				}
				else
				{
					if (analyzeResponse?.ClientMessage != null)
					{
						CancelLogin(analyzeResponse.ClientMessage);
					}
					else
					{
						CancelLogin(CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "FFA55643-3144-4365-9B6C-26722AEEF5BD", "Error Logging in."));
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Track("Authentication Events", "Login Exception", $"Message: {ex.Message}, Stack Track {ex.StackTrace ?? string.Empty}");
				Logging.Log(ex, "AdaptiveAuthenticationViewController:LoginWith");
			}
		}

		private void OnBiometricAuthenticated(string username)
		{
			SessionSettings.Instance.UserId = username.PadLeft(10, '0');
			SessionSettings.Instance.IsAuthenticated = true;

			NavigationController.PopToRootViewController(false);
            Authenticated("authenticated", _mobileLoginResponse);
		}

		private async void BiometricsEnrollment()
		{
			try
			{
				string touchIdAgreement = string.Empty;

                var request = new BiometricInformationRequest
				{
					DeviceType = "Apple",
                    BiometricType = _biometryType.ToString()
				};

				ShowActivityIndicator();

				var methods = new AuthenticationMethods();
                var response = await methods.GetBiometricInformation(request, View);

				HideActivityIndicator();

				if (response != null && !string.IsNullOrEmpty(response.AgreementText))
				{
					touchIdAgreement = response.AgreementText.Replace("\\n", "\n");
				}

                var agreementTitle = request.BiometricType == BiometryType.Face.ToString() ? "SunMobile Face ID Agreement" : "SunMobile Touch ID Agreement";				
				var acceptBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "4D271CAB-66A9-4A57-96EA-F9B6BFD27DD9", "Accept");
				var emailBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "ED7A98CE-7350-4805-9F4F-4AE807E1F072", "Email Agreement");
				var declineBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "5D536C6D-AEF6-43EC-86FC-9D276EA7BEDD", "Decline");

				var agreementResponse = await AlertMethods.Alert(View, agreementTitle, touchIdAgreement, acceptBtnText, emailBtnText, declineBtnText);

				if (agreementResponse.Equals(acceptBtnText))
				{
					AcceptEnrollment();
				}
				else if (agreementResponse.Equals(emailBtnText))
				{
					GeneralUtilities.SendEmail(this, null, agreementTitle, touchIdAgreement, true);
				}
				else if (agreementResponse.Equals(declineBtnText))
				{
					NavigationController.PopToRootViewController(false);
                    Authenticated("authenticated", _mobileLoginResponse);
				}
			}
			catch (Exception ex)
			{
                Logging.Log(ex, "AdaptiveAuthenticationViewController:BiometricsEnrollment");
			}
		}

		public async void CancelLogin(string message = "")
		{
			if (!string.IsNullOrEmpty(message))
			{
				var label = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "EA7F09B2-3E63-4BE8-AA05-5594FDAE4FC8", "Login");

				await AlertMethods.Alert(View, label, message, "OK");
			}
		}

		private void AcceptEnrollment()
		{
			var context = new LAContext();
			NSError AuthError;

            var messageText = _biometryType == BiometryType.Face ? "Use Face ID for Member Number {0}" : "Touch your finger to the home button to match a Fingerprint to your Member Number {0}.";			
			messageText = string.Format(messageText, SessionSettings.Instance.UserId);
			var touchIdMessage = new NSString(messageText);

			if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError))
			{
				var replyHandler = new LAContextReplyHandler((success, error) =>
					InvokeOnMainThread(EnrollBiometrics));

				context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, touchIdMessage, replyHandler);
			}
		}

		private async void EnrollBiometrics()
		{
			try
			{
				ShowActivityIndicator();

				var methods = new AuthenticationMethods();
				var response = await methods.EnrollFingerprintAuthorization(null, null);

				HideActivityIndicator();

				if (response != null && response.Success)
				{
					RetainedSettings.Instance.IsTouchIdRegistered = "true";
					RetainedSettings.Instance.TouchIdRegisteredMemberId = SessionSettings.Instance.UserId;
					RetainedSettings.Instance.TouchIdRegisteredPin = response.FingerprintAuthorizationCode;
					NavigationController.PopToRootViewController(false);
                    Authenticated("authenticated", _mobileLoginResponse);
				}				
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "AdaptiveAuthenticationViewController:EnrollBiometrics");
			}
		}

		public void LoginUsingBiometrics()
		{
            var touchIdRegistered = RetainedSettings.Instance.IsTouchIdRegistered;

            if (touchIdRegistered == "true")
            {

                var context = new LAContext();
                NSError AuthError;

                var messageText = $"Touch your finger to logon as {RetainedSettings.Instance.TouchIdRegisteredMemberId}.";
                var touchIdMessage = new NSString(messageText);

                if (context.CanEvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out AuthError))
                {
                    var replyHandler = new LAContextReplyHandler((success, error) =>
                        InvokeOnMainThread(() =>
                        {
                            if (success)
                            {
                                LoginWithBiometrics(RetainedSettings.Instance.TouchIdRegisteredMemberId, RetainedSettings.Instance.TouchIdRegisteredPin);
                            }
                        }));

                    context.EvaluatePolicy(LAPolicy.DeviceOwnerAuthenticationWithBiometrics, touchIdMessage, replyHandler);
                }
            }
		}

        private void DisableBiometricAuthentication(bool turnOffBiometricSwitch = true)
		{
			if (turnOffBiometricSwitch)
			{
				switchTouchId.On = false;
			}

            imageUseBiometrics.Hidden = true;
			RetainedSettings.Instance.IsTouchIdRegistered = "false";
			RetainedSettings.Instance.TouchIdRegisteredMemberId = string.Empty;
			RetainedSettings.Instance.TouchIdRegisteredPin = string.Empty;
		}

        #if DEBUG
		private void CreateTestLoginButton()
		{
			var testButton = new UIButton();
			testButton.SetTitle("Test Login", UIControlState.Normal);
			testButton.BackgroundColor = AppStyles.ButtonColor;
			testButton.Font = btnLogin.Font;
			testButton.Frame = new CoreGraphics.CGRect(btnLogin.Frame.X, btnLogin.Frame.Y + 120, btnLogin.Frame.Width, btnLogin.Frame.Height);
			AppStyles.SetViewBorder(testButton, true);
			testButton.TouchUpInside += (sender, e) =>
			{
                Login("4022018", "Q@111222");

             
			};

			View.AddSubview(testButton);
		}

        private void CreateTestContactUsButton()
        {
            var testButton = new UIButton();
            testButton.SetTitle("Test Contact us page", UIControlState.Normal);
            testButton.BackgroundColor = AppStyles.ButtonColor;
            testButton.Font = btnLogin.Font;
            testButton.Frame = new CoreGraphics.CGRect(btnLogin.Frame.X, btnLogin.Frame.Y + 180, btnLogin.Frame.Width, btnLogin.Frame.Height);
            AppStyles.SetViewBorder(testButton, true);
            testButton.TouchUpInside += (sender, e) =>
            {
                AppDelegate app = (SunMobile.iOS.AppDelegate)UIApplication.SharedApplication.Delegate;
                app.ShowContactUsPage();


            };

            View.AddSubview(testButton);
        }


     

        #endif
	}
}