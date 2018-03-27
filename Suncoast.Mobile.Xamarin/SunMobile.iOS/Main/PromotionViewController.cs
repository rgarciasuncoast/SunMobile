using System;
using Foundation;
using SunMobile.iOS.Common;
using UIKit;

namespace SunMobile.iOS.Main
{
	public partial class PromotionViewController : BaseViewController
	{
        public string HeaderText { get; set; }
        public string Url { get; set; }
        public string YesButtonText { get; set; }
        public string NoButtonText { get; set; }
        public Action<bool> Finished { get; set; }

		public PromotionViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
            base.ViewDidLoad();

            if (!string.IsNullOrEmpty(HeaderText))
            {
                Title = HeaderText;
            }

            if (!string.IsNullOrEmpty(YesButtonText))
            {
                btnYes.SetTitle(YesButtonText, UIControlState.Normal);
            }

            if (!string.IsNullOrEmpty(NoButtonText))
            {
                btnNo.SetTitle(NoButtonText, UIControlState.Normal);
            }

            btnNo.TouchUpInside += (sender, e) => Finished(false);
            btnYes.TouchUpInside += (sender, e) => Finished(true);

            if (!string.IsNullOrEmpty(Url))
            {
                webViewMain.LoadRequest(new NSUrlRequest(new NSUrl(Url)));
            }
		}
	}
}