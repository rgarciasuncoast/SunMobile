using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Lang;
using SunBlock.DataTransferObjects.ExternalServices;
using SunMobile.Shared.Culture;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Methods;
using SunMobile.Shared.Utilities.General;

namespace SunMobile.Droid.ExternalServices
{
	public class LoanCenterFragment : BaseFragment
	{
		private WebView _webView;
		private Bundle _webViewBundle;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			RetainInstance = true;

            ((MainActivity)Activity).SetActionBarTitle(CultureTextProvider.GetMobileResourceText("82235447-9956-4905-A2DF-019D73AB4827", "A5140E17-C01B-47A5-9C70-09EC8D188827", "Loan Center"));

			var view = (LinearLayout)inflater.Inflate(Resource.Layout.webviewfragment, container, false);

			_webView = view.FindViewById<WebView>(Resource.Id.webView1);
			_webView.SetWebViewClient(new WebViewClient());
			_webView.Settings.JavaScriptEnabled = true;

			if (_webViewBundle == null)
			{
				StartLoanCenter();
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

		public async void StartLoanCenter()
		{
			try
			{
				var retrieveLoanAppSsoRequest = new LoanApplicationSsoRequest
				{
					MemberId = GeneralUtilities.GetMemberIdAsInt()
				};

				ShowActivityIndicator();

				var methods = new ExternalServicesMethods();
				var retrieveLoanAppSsoResponse = await methods.RetrieveLoanApplicationSingleSignOn(retrieveLoanAppSsoRequest, Activity);

				HideActivityIndicator();

				if (retrieveLoanAppSsoResponse?.LoanAppUrl != null)
				{
					_webView.LoadUrl(retrieveLoanAppSsoResponse.LoanAppUrl);
				}
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "LoanCenterFragment:StartLoanCenter");
			}
		}
	}
}