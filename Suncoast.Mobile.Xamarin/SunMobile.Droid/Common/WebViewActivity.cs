using Android.App;
using Android.OS;
using Android.Webkit;
using Android.Widget;
using SunMobile.Droid.Common;

namespace SunMobile.Droid.Accounts
{
	[Activity(Label = "WebViewActivity", Theme = "@style/CustomHoloLightTheme")]
	public class WebViewActivity : BaseActivity
	{
		private TextView txtTitle;
		private ImageButton btnCloseWindow;
		private WebView webView;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetupView();
		}

		public override void SetupView()
		{
			SetContentView(Resource.Layout.WebActivityView);

			var title = Intent.GetStringExtra("Title");
			var url = Intent.GetStringExtra("Url");

			txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);

			if (!string.IsNullOrEmpty(title))
			{
				txtTitle.Text = title;
			}

			btnCloseWindow = FindViewById<ImageButton>(Resource.Id.btnCloseWindow);
			btnCloseWindow.Click += (sender, e) => Finish();

			webView = FindViewById<WebView>(Resource.Id.webView);
			webView.SetWebViewClient(new WebViewClient());
			webView.Settings.JavaScriptEnabled = true;

			if (!string.IsNullOrEmpty(url))
			{
				webView.LoadUrl(url);
			}
		}
	}
}	