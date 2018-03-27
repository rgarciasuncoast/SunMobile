using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace SunMobile.Droid.Common
{
	public class BaseWebViewFragment : BaseFragment
	{
		public string Title { get; set; }
		public string Url { get; set; }
		private WebView _webView;
		private Bundle _webViewBundle;

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			RetainInstance = true;

			if (!string.IsNullOrEmpty(Title))
			{
				((MainActivity)Activity).SetActionBarTitle(Title);
			}

			var view = (LinearLayout)inflater.Inflate(Resource.Layout.webviewfragment, container, false);

			_webView = view.FindViewById<WebView>(Resource.Id.webView1);
			_webView.SetWebViewClient(new WebViewClient());
			_webView.Settings.JavaScriptEnabled = true;

			if (_webViewBundle == null)
			{
				_webView.LoadUrl(Url);
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
	}
}