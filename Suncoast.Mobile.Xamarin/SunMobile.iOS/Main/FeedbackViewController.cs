using System;
using SunMobile.iOS.Common;
using SunMobile.Shared.Logging;
using SunMobile.Shared.Utilities.Settings;

namespace SunMobile.iOS.Main
{
	public partial class FeedbackViewController : BaseViewController
	{
		public event Action<string> Completed = delegate { };

		public FeedbackViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			btnSignMeUp.TouchUpInside += (sender, e) => SignMeUp();
			btnRemindMeLater.TouchUpInside += (sender, e) => RemindMeLater();
			btnNoThanks.TouchUpInside += (sender, e) => NoThanks();
		}

		private void SignMeUp()
		{
			try
			{
				RetainedSettings.Instance.ShowFeedback = false;
				var webViewController = AppDelegate.StoryBoard.InstantiateViewController("WebViewController") as WebViewController;
				webViewController.Url = SessionSettings.Instance.GetStartupSettings["FeedbackUrl"];
				webViewController.Title = "Suncoast Feedback";
				NavigationController.PushViewController(webViewController, true);
			}
			catch (Exception ex)
			{
				Logging.Log(ex, "FeedbackFragment:SignMeUp");
			}
		}

		private void RemindMeLater()
		{
			Completed("success");
		}

		private void NoThanks()
		{
			RetainedSettings.Instance.ShowFeedback = false;
			Completed("success");
		}
	}
}