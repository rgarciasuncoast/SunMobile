using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Lang;
using SunBlock.DataTransferObjects.Geezeo;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.Droid.ExternalServices
{
	public class SunMoneyFragment : BaseFragment
	{
		private WebView _webView;
		private Bundle _webViewBundle;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			RetainInstance = true;

			((MainActivity)Activity).SetActionBarTitle("SunMoney");

			var view = (LinearLayout)inflater.Inflate(Resource.Layout.webviewfragment, container, false);

			_webView = view.FindViewById<WebView>(Resource.Id.webView1);
			_webView.SetWebViewClient(new WebViewClient());
			_webView.Settings.JavaScriptEnabled = true;

			if (_webViewBundle == null)
			{
				IsMemberEnrolledInSunMoney();
			}
			else
			{
				_webView.RestoreState(_webViewBundle);
			}

			return view;
		}

		public override void OnPause()
		{
			base.OnPause();

			_webViewBundle = new Bundle();
			_webView.SaveState(_webViewBundle);
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
				var response = await methods.IsMemberEnrolledInSunMoney(request, Activity);

				HideActivityIndicator();

				if (response != null)
				{
					bool startSunMoney = true;

					if (!response.IsMemberEnrolledInSunMoney)
					{
						var agreementText = response.SunMoneyAgreement.Replace("\\n", "\n");
						var agreementResponse = await AlertMethods.Alert(Activity, "SunMoney Agreement", agreementText, "Accept", "Email", "Cancel");

						if (agreementResponse == "Accept")
						{
							startSunMoney = true;

							var setMemberSunMoneyEnrollmentRequest = new SetMemberSunMoneyEnrollmentRequest
							{
								IsEnrolled = true,
								MemberId = GeneralUtilities.GetMemberIdAsInt()
							};

							ShowActivityIndicator();

							await methods.SetMemberSunMoneyEnrollment(setMemberSunMoneyEnrollmentRequest, Activity);

							HideActivityIndicator();
						}
						else if (agreementResponse == "Email")
						{
							startSunMoney = false;
							GeneralUtilities.DisableView((ViewGroup)View, true);
							GeneralUtilities.SendEmail(Activity, null, "SunMoney Agreement", agreementText);
						}
						else
						{
							startSunMoney = false;
							GeneralUtilities.DisableView((ViewGroup)View, true);
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

						var retrieveGeezeoSingleSignOnResponse = await methods.RetrieveGeezeoSingleSignOn(retrieveGeezeoSingleSignOnRequest, Activity);

						HideActivityIndicator();

						if (retrieveGeezeoSingleSignOnResponse != null && !string.IsNullOrEmpty(retrieveGeezeoSingleSignOnResponse.SingleSignOnResponse))
						{
							_webView.LoadData(retrieveGeezeoSingleSignOnResponse.SingleSignOnResponse, "text/html", "UTF-8");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "SunMoneyViewController:IsMemberEnrolledInSunMoney");
			}
		}
	}
}