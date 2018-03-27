using System;
using UIKit;
using Foundation;

namespace SunMobile.iOS.Common
{
	public partial class WebViewController : UIViewController
	{
		public string HeaderTitle { get; set; }
		public string Url { get; set; }

		public WebViewController(IntPtr handle) : base(handle)
		{			
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (!string.IsNullOrEmpty(HeaderTitle))
			{
				Title = HeaderTitle;
			}

			webView.LoadRequest(new NSUrlRequest(new NSUrl(Url)));
		}
	}
}