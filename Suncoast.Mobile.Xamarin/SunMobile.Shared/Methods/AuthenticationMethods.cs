using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSFCU.Gateway.DataTransferObjects.Host.Symitar.Account;
using SunBlock.DataTransferObjects;
using SunBlock.DataTransferObjects.Authentication;
using SunBlock.DataTransferObjects.Authentication.Adaptive;
using SunBlock.DataTransferObjects.Authentication.Adaptive.InAuth.InMobile;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor;
using SunBlock.DataTransferObjects.Authentication.Adaptive.Multifactor.Enums;
using SunBlock.DataTransferObjects.CreditUnion.Memberships.OnlineAccess;
using SunBlock.DataTransferObjects.Culture;
using SunBlock.DataTransferObjects.Mobile;
using SunBlock.DataTransferObjects.Mobile.Model.Authentication;
using SunBlock.DataTransferObjects.Mobile.Model.Notifications;
using SunBlock.DataTransferObjects.Session;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;
using SunMobile.Shared.Utilities.Web;

namespace SunMobile.Shared.Methods
{
	public class AuthenticationMethods : SunBlockServiceBase
	{
		public Task<AnalyzeViewModel> GenerateTokenAndLogs(AnalyzeWithPayloadRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v1/GenerateTokenAndLogs";
			var response = PostToSunBlock<AnalyzeViewModel>(url, request, @"", view);

			return response;
		}

		public Task<StatusResponse<string>> RegisterInAuth(PayloadMessage request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v1/RegisterInAuth";
			var response = PostToSunBlock<StatusResponse<string>>(url, request, @"", view);

			return response;
		}

		public Task<StatusResponse> UnRegisterInAuth(PayloadMessage request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v1/UnregisterInAuth";
			var response = PostToSunBlock<StatusResponse>(url, request, @"", view);

			return response;
		}

        public Task<GetInAuthLogConfigResponse> GetInAuthLogConfig(PayloadMessage request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v2/GetInAuthLogConfig";
			var response = PostToSunBlock<GetInAuthLogConfigResponse>(url, request, @"", view);

			return response;
		}

		public Task<StatusResponse<string>> GetInAuthLogs(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v1/GetInAuthLogs";
			var response = PostToSunBlock<StatusResponse<string>>(url, request, @"", view);

			return response;
		}

		public Task<StatusResponse<CultureConfiguration>> GetCultureConfiguration(DateTime request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v1/GetCultureConfiguration";
			var response = PostToSunBlock<StatusResponse<CultureConfiguration>>(url, request, @"", view);

			return response;
		}

		public async Task<MobileStatusResponse> IsOutOfBandChallengeRequired(OutOfBandTransactionTypes outOfBandTransactionType, object view, object navigationController = null, object viewToRunAfterValidation = null)
		{
			var returnValue = new MobileStatusResponse { Success = true };

            try
            {
                var request = new OutOfBandChallengeRequiredRequest
                {
                    Payload = RetainedSettings.Instance.Payload,
                    TransactionType = outOfBandTransactionType.ToString()
                };

                var response = await OutOfBandChallengeRequired(request, view, outOfBandTransactionType, navigationController, viewToRunAfterValidation);

                if (response != null && response.Success && !response.OutOfBandChallengeRequired)
                {
                    returnValue.OutOfBandChallengeRequired = false;
                }
                else
                {
                    returnValue.OutOfBandChallengeRequired = true;
                    returnValue.CanUseAtmLastEight = response.CanUseAtmLastEight;
                }
            }
            catch(Exception ex)
            {
                Logging.Logging.Log(ex, "AuthenticatinMethods:IsOutOfBandChallengeRequired");
            }

			return returnValue;
		}

		public Task<MobileStatusResponse> OutOfBandChallengeRequired(OutOfBandChallengeRequiredRequest request, object view, OutOfBandTransactionTypes outOfBandTransactionType, object navigationController, object viewToRunAfterValidation)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/OutOfBandChallengeRequired";
			var response = PostToSunBlock<MobileStatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view, false, outOfBandTransactionType, navigationController, viewToRunAfterValidation);

			return response;
		}

		public Task<StatusResponse<bool>> SendOutOfBandCode(SendOutOfBandCodeRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SendOutOfBandCode";
			var response = PostToSunBlock<StatusResponse<bool>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<OutOfBandVerificationResponse>> VerifyOutOfBandCode(VerifyOutOfBandCodeRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/VerifyOutOfBandCode";
			var response = PostToSunBlock<StatusResponse<OutOfBandVerificationResponse>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse<bool>> EnableOnlineAccess(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/EnableOnlineAccess";
			var response = PostToSunBlock<StatusResponse<bool>>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<GetChangePasswordInformationResponse> GetChangePasswordInformation(GetChangePasswordInformationRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetChangePasswordInformation";
			var response = PostToSunBlock<GetChangePasswordInformationResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<MobileStatusResponse<bool>> UpdatePassword(MobileDeviceVerificationRequest<UpdatePasswordRequest> request, object view, object navigationController)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v2/UpdatePassword";
			var response = PostToSunBlock<MobileStatusResponse<bool>>(url, request, SessionSettings.Instance.SunBlockToken, view, false, OutOfBandTransactionTypes.Profile, navigationController);

			return response;
		}

		public Task<EnrollFingerprintAuthorizationResponse> EnrollFingerprintAuthorization(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/EnrollFingerprintAuthorization";
			var response = PostToSunBlock<EnrollFingerprintAuthorizationResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<AuthenticateHostViewModel> AuthenticateHost(MobileAuthenticateHostRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v3/AuthenticateHost";
			var response = PostToSunBlock<AuthenticateHostViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<AuthenticateFingerprintViewModel> AuthenticateFingerprint(AuthenticateFingerprintRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v4/AuthenticateFingerprint";
			var response = PostToSunBlock<AuthenticateFingerprintViewModel>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> Logout(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/LogOut";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view, true);

			return response;
		}

		public Task<NotificationRegistrationData> RegisterForNotification(PSNRegistrationData request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/RegisterForNotification";
			var response = PostToSunBlock<NotificationRegistrationData>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> SessionIsActive(AnalyzeRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SessionIsActive";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view, true);

			return response;
		}		

        public Task<BiometricInformationResponse> GetBiometricInformation(BiometricInformationRequest request, object view)
        {
            string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetBiometricInformation";
            var response = PostToSunBlock<BiometricInformationResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

            return response;
        }

		public Task<GetStartupSettingsResponse> GetStartupSettings(GetStartupSettingsRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockAnalyzeUrl + "v1/GetStartupSettings";
			var response = PostToSunBlock<GetStartupSettingsResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<GetAccountVerificationOptionsResponse> GetAccountVerificationOptions(object request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/GetAccountVerificationOptions";
			var response = PostToSunBlock<GetAccountVerificationOptionsResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> DelayPasswordNotification(DelayPasswordNotificationRequest request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/DelayPasswordNotification";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

		public Task<StatusResponse> SetFlags(List<FlagOption> request, object view)
		{
			string url = AppSettings.SunBlockUrl + AppSettings.SunBlockServiceUrl + "v1/SetFlags";
			var response = PostToSunBlock<StatusResponse>(url, request, SessionSettings.Instance.SunBlockToken, view);

			return response;
		}

        public async Task<bool> IsOnLineDisclosureAccepted(bool isAccepted, string onlineBankingAgreementText, object View)
		{
			bool returnValue = false;

            if (!isAccepted)
			{
				var agreementTitle = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "8E3F2D92-B987-4391-885A-047478BBE99D", "General Disclosure");
				var agreementText = onlineBankingAgreementText.Replace("\\n", "\n");
				var acceptText = CultureTextProvider.GetMobileResourceText("BA287EA8-E3D6-4850-BE61-2B48BC9EDFDB", "B28432BE-04AC-4591-A049-AC98B8375C56", "Accept");
				var emailText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "60AEF6C4-4F82-4509-8DD7-3DEA5F20EADD", "Email");
				var declineText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "5D536C6D-AEF6-43EC-86FC-9D276EA7BEDD", "Decline");

				var agreementResponse = await AlertMethods.Alert(View, agreementTitle, agreementText, acceptText, emailText, declineText);

				if (agreementResponse == acceptText)
				{
					returnValue = true;

					await EnableOnlineAccess(null, View);
				}
				else if (agreementResponse == emailText)
				{
					returnValue = false;
					GeneralUtilities.SendEmail(View, null, agreementTitle, agreementText, false, true);
				}
				else
				{
					returnValue = false;
				}
			}
			else
			{
				returnValue = true;
			}			

			return returnValue;
		}

		public async Task EstatementsOptIn(bool eStatementOptInViewed, bool eStatementsEnrolled, string enrollmentAgreementText, object View)
		{
			try
			{				
                if (!eStatementOptInViewed && !eStatementsEnrolled)
				{
					var agreementTitle = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "28C70E8A-A215-4646-9467-0104251D353B", "eStatements Enrollment");
                    var agreementText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "07D63296-7ECD-46D1-8763-ADB859DAE65C", enrollmentAgreementText);
					var acceptBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "4D271CAB-66A9-4A57-96EA-F9B6BFD27DD9", "Accept");
					var declineBtnText = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "5D536C6D-AEF6-43EC-86FC-9D276EA7BEDD", "Decline");

					agreementText = agreementText.Replace("\\n", "\n");
					var agreementResponse = await AlertMethods.Alert(View, agreementTitle, agreementText, acceptBtnText, declineBtnText);

					if (agreementResponse == acceptBtnText)
					{
						var flagOptions = new List<FlagOption>();
						var flagView = new FlagOption { FlagType = FlagTypes.EStatementOptinViewed.ToString(), Value = true };
						var flagEnroll = new FlagOption { FlagType = FlagTypes.EStatementsEnrolled.ToString(), Value = true };
						flagOptions.Add(flagView);
						flagOptions.Add(flagEnroll);

						await SetFlags(flagOptions, View);
					}
					else if (agreementResponse == declineBtnText)
					{
						var flagOptions = new List<FlagOption>();
						var flagView = new FlagOption { FlagType = FlagTypes.EStatementOptinViewed.ToString(), Value = true };
						flagOptions.Add(flagView);

						await SetFlags(flagOptions, View);
					}
				}
			}			
			catch (Exception ex)
			{
				Logging.Logging.Log(ex, "AuthenticationMethods:EstatementsOptIn");
			}
		}

		public string ValidateLoginRequest(string username, string password)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{
				message = CultureTextProvider.GetMobileResourceText("949A3C83-C4A9-45BF-9341-C38AD698E253", "1FC4A412-5E2C-4CE7-B859-59D52F323836", "Member Number and Password must be filled in.");
			}

			return message;
		}

		public bool ValidatePassword(string password, string passwordRegex)
		{
			bool returnValue = false;

			if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(passwordRegex))
			{
				var regex = new Regex(passwordRegex);
				var match = regex.Match(password);

				if (match.Success)
				{
					returnValue = true;
				}
			}

			return returnValue;
		}

		public bool ValidateConfirmPassword(string password, string confirmPassword, string passwordRegex)
		{
			bool returnValue = false;

			if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword) && password == confirmPassword)
			{
				var regex = new Regex(passwordRegex);
				var match = regex.Match(confirmPassword);

				if (match.Success)
				{
					returnValue = true;
				}
			}

			return returnValue;
		}

		public string ValidateAnalyzeRequest(AnalyzeRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.UserId))
			{
				message = "Member Number must be filled in.";
			}

			return message;
		}

		public string ValidateAuthenticateHostRequest(MobileAuthenticateHostRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.Password))
			{
				message = "Password must be filled in.";
			}

			return message;
		}

		public string ValidateAuthenticateHostUnlockRequest(AuthenticateHostUnlockRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.Password))
			{
				message = "Password must be filled in.";
			}

			return message;
		}

		public string ValidateChallengeRequest(ChallengeRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.ChallengeAnswer))
			{
				message = "Challenge answer must be filled in.";
			}

			return message;
		}

		public string ValidateNotEnrolledRequest(NotEnrolledRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.Password))
			{
				message = "Password must be filled in.";
			}

			return message;
		}

		public string ValidateUpdateImagePhraseRequest(UpdateImagePhraseRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.SelectedImage) || string.IsNullOrEmpty(request.SelectedPhrase))
			{
				message = "Security phrase must be filled in and an image selected.";
			}

			return message;
		}

		public string ValidateUpdateChallengeQuestionsRequest(UpdateChallengeQuestionsRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.SelectedAnswer1) ||
				string.IsNullOrEmpty(request.SelectedAnswer2) ||
				string.IsNullOrEmpty(request.SelectedAnswer3) ||
				string.IsNullOrEmpty(request.SelectedQuestion1) ||
				string.IsNullOrEmpty(request.SelectedQuestion2) ||
				string.IsNullOrEmpty(request.SelectedQuestion3) ||
				request.SelectedQuestion1 == "Select Security Question" ||
				request.SelectedQuestion2 == "Select Security Question" ||
				request.SelectedQuestion3 == "Select Security Question")
			{
				message = "All questions and answers must be filled in.";
			}

			return message;
		}

		public string ValidateUnlockRequest(UnlockRequest request)
		{
			var message = string.Empty;

			if (string.IsNullOrEmpty(request.UnlockAnswer1) ||
				string.IsNullOrEmpty(request.UnlockAnswer2) ||
				string.IsNullOrEmpty(request.UnlockAnswer3) ||
				string.IsNullOrEmpty(request.SelectedQuestion1) ||
				string.IsNullOrEmpty(request.SelectedQuestion2) ||
				string.IsNullOrEmpty(request.SelectedQuestion3))
			{
				message = "All questions and answers must be filled in.";
			}

			return message;
		}
	}
}