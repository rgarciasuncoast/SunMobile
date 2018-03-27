using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using SunMobile.Shared.Data;
using SunMobile.Shared.Methods;

namespace SunMobile.Droid.ExternalServices
{
    public class LoanCenterFragment : BaseFragment
    {
        private WebView _webView;
        private Bundle _webViewBundle;

        #region Required Parameters
        public LoanCenterTypes LoanType { get; set; }
        #endregion

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			RetainInstance = true;

			var view = (LinearLayout)inflater.Inflate(Resource.Layout.webviewfragment, container, false);

			_webView = view.FindViewById<WebView>(Resource.Id.webView1);
			_webView.SetWebViewClient(new WebViewClient());
			_webView.Settings.JavaScriptEnabled = true;

			if (_webViewBundle == null)
			{
				LoadWebPage();
			}
			else
			{
				_webView.RestoreState(_webViewBundle);
			}

			return view;
		}

        private async void LoadWebPage()
        {
            var methods = new ExternalServicesMethods();

            ShowActivityIndicator();

            var url = await methods.GetLoanCenterUrl(LoanType, Activity);

            HideActivityIndicator();

            _webView.LoadUrl(url);
        }

		public override void OnPause()
		{
			base.OnPause();

			_webViewBundle = new Bundle();
			_webView.SaveState(_webViewBundle);
		}
	}
}