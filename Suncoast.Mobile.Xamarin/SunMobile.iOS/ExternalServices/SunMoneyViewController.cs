using System;
using Foundation;
using SunBlock.DataTransferObjects.Geezeo;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.iOS.ExternalServices
{
    public partial class SunMoneyViewController : BaseViewController
	{
		public SunMoneyViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			IsMemberEnrolledInSunMoney();
		}

		public async void IsMemberEnrolledInSunMoney()
		{
			try 
			{
				var request = new IsMemberEnrolledInSunMoneyRequest
				{
					MemberId = GeneralUtilities.GetMemberIdAsInt()
				};

				ShowActivityIndicator();

				var methods = new ExternalServicesMethods();
				var response = await methods.IsMemberEnrolledInSunMoney(request, null);

				HideActivityIndicator();

				if (response != null)
				{
					bool startSunMoney = true;

					if (!response.IsMemberEnrolledInSunMoney)				
					{
						var agreementText = response.SunMoneyAgreement.Replace("\\n", "\n");
						var agreementResponse = await AlertMethods.Alert(View, "SunMoney Agreement", agreementText, "Accept", "Email", "Cancel");

						if (agreementResponse == "Accept")
						{
							startSunMoney = true;

							var setMemberSunMoneyEnrollmentRequest = new SetMemberSunMoneyEnrollmentRequest 
							{
								IsEnrolled = true,
								MemberId = GeneralUtilities.GetMemberIdAsInt()
							};

							ShowActivityIndicator();

							await methods.SetMemberSunMoneyEnrollment(setMemberSunMoneyEnrollmentRequest, null);

							HideActivityIndicator();
						}
						else if (agreementResponse == "Email")
						{
							startSunMoney = false;
							GeneralUtilities.SendEmail(this, null, "SunMoney Agreement", agreementText, false, true);
						}
						else
						{
							startSunMoney = false;
						}
					}

					if (startSunMoney)
					{
						var retrieveGeezeoSingleSignOnRequest = new RetrieveGeezeoSingleSignOnRequest
						{
							IsMobile = true,
							IssuerUrl = "https://sunnet.suncoastfcu.org",
							MemberId = SessionSettings.Instance.UserId
						};

						ShowActivityIndicator();

						var retrieveGeezeoSingleSignOnResponse = await methods.RetrieveGeezeoSingleSignOn(retrieveGeezeoSingleSignOnRequest, null);

						HideActivityIndicator();

						if (retrieveGeezeoSingleSignOnResponse != null && !string.IsNullOrEmpty(retrieveGeezeoSingleSignOnResponse.SingleSignOnResponse))
						{
							webView.LoadData(retrieveGeezeoSingleSignOnResponse.SingleSignOnResponse, "text/html", "UTF-8", new NSUrl("https://suncoastcreditunion.com"));
						}
					}
					else
					{
						View.UserInteractionEnabled = false;
						View.Alpha = 0.3f;
					}
				} 
			}
			catch(Exception ex)
			{
				Logging.Log(ex, "SunMoneyViewController:IsMemberEnrolledInSunMoney");
			}
		}
	}
}